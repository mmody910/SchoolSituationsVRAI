using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Unity.XR.CoreUtils;

public class AIManager : MonoBehaviour, Loggable
{
    [SerializeField]
//    LogManager logManager;
    [HideInInspector]
    public string response = "";

    [SerializeField] string prompts;
    [SerializeField] public string promptsEng;
    [SerializeField] public string promptsNor;
    [SerializeField] string previousPrompt = "";
    [SerializeField] string jobDescription = "";
    [SerializeField] string[] talker;
    [SerializeField] private Dictionary<string, GameObject> characters;
    public string[] talkerEng;
    public string[] talkerNor;
    string openAIDomain = "";
    int index = 0;
    string language = "";
    string context = "";
    public string prompt = "";

    public SpeechManager speechManager;
    public Queue<string> speakerQueue;
    public bool fromRemote = true;
    public string test;

    public string[][] firstHalfResponse;
    string incompleteNormal = "";
    string incompleteNI = "";
    public string[][] secondHalfResponse;

    public string[][] noInputResponse;


    public void fetchPrompt(Prompt p)
    {
        //print(p.prompt.en);
        speechManager = GetComponent<SpeechManager>();
        language = PlayerPrefs.GetString("YatekLang");
        speakerQueue = new Queue<string>();
        characters = new Dictionary<string, GameObject>();
        setPrompt(p);


        openAIDomain = "https://api.openai.com/v1/chat/completions";
        if (language == "en-US")
        {
            openAIDomain = "https://api.openai.com/v1/chat/completions";
            talker = talkerEng;
            prompts = promptsEng;
        }
        else
        {
            openAIDomain = "https://api.openai.com/v1/chat/completions";
            talker = talkerNor;
            prompts = promptsNor;
        }

        if (p.intro.en != "" && p.intro.nb != "")
        {
            if (language == "en-US")
            {
                AssignResponses(ValidateResponses(p.intro.en));
            }
            else
            {
                AssignResponses(ValidateResponses(p.intro.nb));
            }
        }

        Debug.Log(p.intro.en);
        prompt = prompts + "\n";
        updatePrompt();
    }

    void setPrompt(Prompt p)
    {
        talkerEng = p.talkers.en.ToArray();
        talkerNor = p.talkers.nb.ToArray();
        promptsEng = p.prompt.en;
        promptsNor = p.prompt.nb;

        talkerNor[talkerNor.Length - 1] = PlayerPrefs.GetString("PlayerName").Split()[0];
        talkerEng[talkerEng.Length - 1] = PlayerPrefs.GetString("PlayerName").Split()[0];
        speechManager.AssignCharacters();
    }

    public void updatePrompt()
    {
        response = "";
        context = PlayerPrefs.GetString("YatekContext");
        string pattern = @"\b_context\b";
        prompt = prompts;
        prompt = Regex.Replace(prompt, pattern, context);
    }

    public void SentenceSpoken(string sentence)
    {
        prompt += $"\n{sentence}";
    }

    public async Task GetText(string playerInput, bool generate, string base_response, bool predictiveCall = false)
    {
        bool usingPlayerInput = true;
        var firstTalker = "";
        // Debug.log( "start generated : " + Time.time);
        UnityEngine.Random.InitState(Time.frameCount);
        string promptInput = "";
        characters = speechManager.speakers.ToDictionary((x) => x.serverName, (x) => x.charGO);
        //if (playerInput.Length > 0)
        // Remove knowledge fuzzifier for now, it is causing prompts to break.
        // promptInput = prompt + $"\n{talker[talker.Count() - 1]} : {playerInput}\n" + KnowledgeFuzzifier.GenerateKnowledgeLogEntry(speechManager);
        if (playerInput.Length > 0 && !predictiveCall)
        {
            promptInput = prompt + $"\n{talker[talker.Count() - 1]}:\"{playerInput}\"\n";
        }
        else if (predictiveCall && playerInput.Length == 0)
        {
            string completion = "\n";
            if (secondHalfResponse != null)
                foreach (string[] s in secondHalfResponse)
                {
                    completion += s[0].Trim() + ": " + s[1] + "\n";
                }

            if (noInputResponse != null)
                foreach (string[] s in noInputResponse)
                {
                    completion += s[0].Trim() + ": " + s[1] + "\n";
                }

            if (completion.Trim().Length != 0)
                promptInput = prompt.TrimEnd() + "\n" + completion + incompleteNI;
            else
            {
                promptInput = prompt.TrimEnd() + "\n" + $"{talker[UnityEngine.Random.Range(0, talker.Length - 1)]}:";
            }
        }

        else if (predictiveCall)
        {
            if (playerInput.Length > 0)
                promptInput = prompt.TrimEnd() + "\n" + playerInput;
        }
        else
        {
            // chose a talker to start the following interaction
            firstTalker = talker[UnityEngine.Random.Range(0, talker.Length - 1)];
            playerInput = "";
            promptInput = prompt.TrimEnd() + $"\n{firstTalker}:";
            // set flag to know that we are not using player input
            usingPlayerInput = false;
        }

        //else
        //  promptInput = prompt + KnowledgeFuzzifier.GenerateKnowledgeLogEntry()+"\n";
        //// Debug.log( $"prompt input: {promptInput}");
        if (generate)
        {
            chatOpenAIData chatData = new chatOpenAIData();
            if (language == "en-US")
            {
                chatData.messages[0]["content"] =
                    "you are a screen writer who is writing this script with the character name then a colon then what they are saying";
                chatData.messages[1]["content"] = "complete the following script\n";
            }
            else
            {
                chatData.messages[0]["content"] =
                    "du er en manusforfatter som skriver dette manus med karakternavnet deretter kolon deretter hva de sier";
                chatData.messages[1]["content"] = "fullfør følgende manuskript\n";
            }

            chatData.messages[1]["content"] += promptInput;
            chatData.stop = new string[1];
            chatData.stop[0] = talker[talker.Count() - 1];
            // // Debug.log( PlayerPrefs.GetString("UserId"));
            chatData.user = PlayerPrefs.GetString("UserId");
            string json = JsonConvert.SerializeObject(chatData);
            ////print(json);
            //https://api.openai.com/v1/engines/text-davinci-002/completions for Norwegian
            //https://api.openai.com/v1/engines/text-curie-001/completions for English
            using (UnityWebRequest www = UnityWebRequest.Put(openAIDomain, json))
            {
                www.method = "POST";
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + LoadKeys.OPEN_AI_API_KEY.ToString());

                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    //             if (logManager.DebugMode)
                    Debug.LogError(LogFunction("GetText", "failed", www.error, ("playerInput", playerInput),
                        ("predictiveCall", predictiveCall.ToString())));
                    Debug.LogError(www.downloadHandler.text);
                }
                else
                {
                    chatOpenAIResponse openAIresponse =
                        JsonConvert.DeserializeObject<chatOpenAIResponse>(www.downloadHandler.text);

                    if (openAIresponse.choices.Length > 0)
                    {
                        response = openAIresponse.choices[0].message["content"];
                        //print("openai prompt: " + openAIdata.prompt);
                        //print(openAIdata.stop[0]);
                        //print("openai response: " + response);
                        //print("response length: " + response.Length);
                        if (!usingPlayerInput && response.Length > 5)
                        {
                            // if we are not using player input, we need add the talker name to the response
                            response = $"\n{firstTalker}:" + response;
                            // removing the extra talker in the most recent line from the prompt input as it will be added when the speech synthesis is done
                            promptInput = promptInput.Substring(0, promptInput.Length - firstTalker.Length - 1);
                            //print("response after adding talker: " + response);
                            //print("prompt input after removing talker: " + promptInput);
                            usingPlayerInput = true;
                        }
                        else if (predictiveCall)
                        {
                            response = incompleteNormal + response;
                        }
                        else if (playerInput.Length == 0 && !predictiveCall)
                        {
                            response = $"\n{talker[0]}:" + response;
                        }

                        // Debug.log( response);
                        //  //print(gameObject.name);
                    }
                }
            }

            openAiData openAIdataCF = new openAiData();
            openAIdataCF.prompt = $"<|endoftext|>[{response}]\n--\nLabel:";
            openAIdataCF.model = "content-filter-alpha";
            openAIdataCF.max_tokens = 1;
            openAIdataCF.temperature = 0;
            openAIdataCF.top_p = 1;
            openAIdataCF.logprobs = 10;

            openAIdataCF.user = PlayerPrefs.GetString("UserId");
            string jsonCF = JsonUtility.ToJson(openAIdataCF);


            using (UnityWebRequest www = UnityWebRequest.Put("https://api.openai.com/v1/completions", jsonCF))
            {
                www.method = "POST";
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + LoadKeys.OPEN_AI_API_KEY.ToString());

                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    // Debug.log( www.error);
                }
                else
                {
                    openAiResponse openAIresponseCF = JsonUtility.FromJson<openAiResponse>(www.downloadHandler.text);

                    if (openAIresponseCF.choices.Length > 0)
                    {
                        string output_label = openAIresponseCF.choices[0].text;
                        if (output_label == "2")
                        {
                            /*
                            Importantly, you need to check not only the label that was returned by the filter (0, 1, or 2), but sometimes also the logprobs associated with these.
                            If the filter returns 0 or 1, you should accept that as the filter's outcome. If the filter returns 2, you should accept this outcome only if its logprob is greater than -0.355.
                            If the logprob for 2 is beneath -0.355 (for example, -0.4), then you should use as output whichever of 0 or 1 has a logprob closer to 0.
                            */
                            response = base_response;
                        }
                        else
                        {
                            var clean_response = Regex.Replace(response, @"\t|\n|\r", "");
                            //print(clean_response);
                            //  var expression_input = $"\nSpeaker :{playerInput}\n Listener:" + clean_response + " Emotion:";
                            foreach (GameObject v in characters.Values)
                            {
                                // var expController = v.GetComponent<expressionsManager>();
                                var expController = v.GetComponentInChildren<expressionsManager>();
                                ////print(expression_input);
                                if (expController != null)
                                    expController.react(clean_response);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            response = base_response;
        }

        // Debug.log( response);
        // Debug.log( gameObject.GetComponent<AIManager>().enabled);
        // Debug.log( gameObject.name);
        // todo avoid calling trim response more than once if not absolutely needed, replace similar logic in validate response.
        if (gameObject.GetComponent<AIManager>().enabled)
        {
            var r = ValidateResponses((response));
            if ((r == null || r.Length == 0)) //&&logManager.DebugMode)
                Debug.Log(LogFunction("GetText", "Failed, Retrying", response, ("playerInput", playerInput),
                    ("predictive call", predictiveCall.ToString())));

            if (!predictiveCall && playerInput.Length != 0)
            {
                AssignResponses(r);
                ResponseCompletion(r);
                firstHalfResponse = r;
                if (r.Length > 0)
                    prompt = promptInput;
                else
                    GetText(playerInput, true, "");
            }
            else if (playerInput.Length == 0)
            {
                // Debug.log( "NO Input");
                if (r.Length > 0)
                    noInputResponse = r;
                else
                    GetText("", true, "");

                //         ResponseCompletion(noInputResponse);
            }
            else
            {
                // Debug.log( "Predictive Call");
                if (r.Length > 0)
                    secondHalfResponse = r;
                else
                {
                    if (firstTalker.Trim().Length != 0)
                        GetText(playerInput + $"\n{firstTalker}: ", true, "", true);
                    else
                        GetText(
                            playerInput +
                            $"\n{speechManager.speakers[UnityEngine.Random.Range(0, speechManager.speakers.Count - 1)].serverName}: ",
                            true, "", true);
                }
            }

            //            prompt = promptInput + TrimResponse(response);
            if (r.Length > 0)
            {
                //Debug.Log(LogObject());
                string validatedResponse = "\nValidated:";
                foreach (var res in r)
                {
                    validatedResponse += $"\n\t{res[0]}:{res[1]}\n";
                }

                Debug.Log(LogFunction("GetText", "Success", response + validatedResponse, ("playerInput", playerInput),
                    ("predictive call", predictiveCall.ToString())));
            }
        }
    }

    private static char[] stopList = { '.', '!', '?' };

    public string TrimResponse(string res)
    {
        return res.Trim().Substring(0, res.Trim().LastIndexOfAny(stopList) + 1);
    }

    public string[][] ValidateResponses(string response)
    {
        //need new technique for spliting

        List<string[]> usableLines = new List<string[]>();
        response = response.Replace("\\n", "\n");
        string lastLine = "";
        if (response.Contains("\n"))
        {
            lastLine = response.Substring(response.LastIndexOf("\n"));
            bool complete = false;
            foreach (var c in stopList)
                if (lastLine.EndsWith($"{c}"))
                    complete = true;
            if (!complete)
            {
                incompleteNormal = lastLine.Trim();
                if (TrimResponse(incompleteNormal).Length == 0)
                    response = response.Substring(0, response.LastIndexOf("\n"));
                else
                {
                    incompleteNormal = incompleteNormal.Substring(incompleteNormal.LastIndexOfAny(stopList));
                    response = TrimResponse(response);
                }
            }
        }

        response = response.Replace("\"", "");
        //   response = response.Replace("\'", "");
        var pattern = new Regex(@"^(.*?):\s*(.*)$");
        var lines = new List<string>(response.Split('\n'));
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            int nextText = 1;
            if (line.TrimEnd().EndsWith(":") && i != lines.Count - 1)
            {
                while (i + nextText < lines.Count && lines[i + nextText].TrimEnd() == "")
                    nextText++;
                if (i + nextText < lines.Count)
                    line = line + lines[lines.IndexOf((line)) + nextText];
                else
                    continue;
            }

            var match = pattern.Match(line);
            if (match.Success)
            {
                // Extract the speaker and the dialogue from the line
                var speaker = match.Groups[1].Value;
                var dialogue = match.Groups[2].Value;

                if (speaker.Trim().Equals(speechManager.speakers[speechManager.speakers.Count - 1].serverName) ||
                    dialogue.Trim() == "")
                    break;
                // speaker = speaker.Replace(" ", "");
                // Add the speaker and the dialogue to a string array
                var speakerDialogue = new string[] { speaker, dialogue };

                //Debug.Log($"Speaker: {speaker}\nDialogue {dialogue}");
                usableLines.Add(speakerDialogue);
            }
        }
        Debug.Log(usableLines.Count.ToString() + " LINES");
        return usableLines.ToArray();
    }

    public void AssignResponses(string[][] usableLines, bool clearNoInput = false, bool clearOtherHalf = false)
    {
        speakerQueue.Clear();
        foreach (var speaker in speechManager.speakers)
        {
            if (speaker.charGO.GetComponents<XROrigin>() == null)
                speaker.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Clear();
        }

        if (usableLines.Length > 0)
        {
            foreach (string[] r in usableLines)
            {
                ////print($"usable line r[0] : {r[0]}, r[1] : {r[1]}");
                foreach (Character x in speechManager.speakers)
                {
                    if (x.charGO.GetComponent<XROrigin>() == null && x.serverName.Trim() == r[0].Trim())
                    {
                        print(r[1].Trim());
                        //print($"enqueing + {r[0]}");
                        speakerQueue.Enqueue(r[0].Trim());
                        x.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Enqueue(r[1]);
                    }
                }
            }

            if (clearNoInput)
            {
                noInputResponse = null;
            }

            if (clearOtherHalf)
            {
                secondHalfResponse = null;
            }
        }
        else
        {
            // Debug.log( "Empty Response, Retrying");
            GetText("", true, "");
            return;
        }
    }

    public async void ResponseCompletion(string[][] initialResponse)
    {
        string input = "";

        foreach (string[] i in initialResponse)
        {
            input += i[0] + ":" + i[1].Trim() + "\n";
        }

        input += incompleteNormal;
        // Debug.log( "RC");
        await GetText(input, true, "", true);
    }

    public string LogObject()
    {
        // string log = $"[{DateTime.UtcNow}]\n"
        //    +$"\nTime Since Program Launch:\n\t{Time.realtimeSinceStartup}\n\n";
        string log = "";

        log += $"OpenAI Model:\n\t{openAIDomain}\n\n";

        log += $"Language:\n\t{language}\n\n";

        log += $"Context:\n\t{context}\n\n";

        log += "Talkers:\n";
        foreach (string talker in talker)
            log += $"\t{talker}\n";

        if (speakerQueue != null && speakerQueue.Count != 0)
        {
            log += "\nSpeaker Queue:\n";
            Queue<string> copyQueue = new Queue<string>(speakerQueue);
            while (copyQueue.Count > 0)
                log += $"\t{copyQueue.Dequeue()} ->\n";
        }

        if (prompt != null)
            log += $"\nLast Saved OpenAI prompt:\n\t{prompt}\n";

        if (response != null)
            log += $"\nLatest OpenAI response:\n\t{response}\n";

        if (firstHalfResponse != null && firstHalfResponse.Length != 0)
        {
            log += "\nLast Validated OpenAI Response from User Input:\n";
            foreach (string[] response in firstHalfResponse)
            {
                log += $"\t{response[0]}: {response[1]}\n";
            }
        }

        if (incompleteNormal != null && incompleteNormal.Length != 0)
        {
            log += "Incomplete statement from last OpenAI response from User Input:\n\t";
            log += $"{incompleteNormal}\n\n";
        }

        if (secondHalfResponse != null && secondHalfResponse.Length != 0)
        {
            log += "Last Validated Second Half Response from User Input:\n";
            foreach (string[] response in secondHalfResponse)
            {
                log += $"\t{response[0]}: {response[1]}\n";
            }
        }

        if (noInputResponse != null && noInputResponse.Length != 0)
        {
            log += "\nLast Validated OpenAI Response without User Input:\n";
            foreach (string[] response in noInputResponse)
            {
                log += $"\t{response[0]}: {response[1]}\n";
            }
        }

        return log;
    }


    public string LogFunction(string functionName, string status, string output, params (string, string)[] parameters)
    {
        string log = "";
        log += $"{functionName}\n\n";
        log += $"Status:\n\t{status}\n\n";
        log += "Parameteres:\n\n";
        foreach (var param in parameters)
        {
            log += $"\t{param.Item1}:\t {param.Item2}\n\n";
        }

        log += $"Output:\n\t{output}";
        return log;
    }
}