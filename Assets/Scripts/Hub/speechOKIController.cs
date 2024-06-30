using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class speechOKIController : MonoBehaviour
{
    public keysController keys;
    string[] sentences;
    public string[] sentencesEng;
    public string[] sentencesNor;
    public AudioSource SynthesisAudioSource;
    string language = "";
    private float startTimeOfPlayAudio;
    [HideInInspector]
    public int index;
    [HideInInspector]
    public bool practiceFlag = false;

    [HideInInspector]
    public bool interactionSpeechFlag = false;
    public TextMeshProUGUI text;
    public string promptEng;
    public string promptNor;
    string prompt;
    int indexOfInteraction = 0;
    float nextTime = 0;
    public RecognizeSpeech sp;
    string userInput = "";
    string prevMessage = "";
    int interval = 1;
    [SerializeField]
    float HubReturnTime = 3.0f;
    // Start is called before the first frame update

    public PauseMenuController pauseController;


    void Awake()
    {

        LoadKeys.Load(keys);
        language = PlayerPrefs.GetString("YatekLang");

        if (language == "en-US")
        {
            sentences = sentencesEng;
        }
        else
        {
            sentences = sentencesNor;
        }
        index = 0;

    }
    async void Start()
    {
/*
        if (PlayerPrefs.GetString("notFirstTime") == "yes")
        {
            pauseController.enabled = true;
        }
*/

        SynthesizeAudioAsync(sentences[0], true, "");

    }

    public async Task GetText(string playerInput, bool generate, string base_response)
    {
        indexOfInteraction++;
        Debug.Log("start generated : " + Time.time);
        string promptInput = "";
        //if (playerInput.Length > 0)
        // Remove knowledge fuzzifier for now, it is causing prompts to break.
        // promptInput = prompt + $"\n{talker[talker.Count() - 1]} : {playerInput}\n" + KnowledgeFuzzifier.GenerateKnowledgeLogEntry(speechManager);
        if (language == "en-US")
        {
            promptInput = promptEng + $"\nPlayer : {playerInput}\nOKi : ";
        }
        else
        {
            promptInput = promptNor + $"\nSpiller : {playerInput}\nOKi : ";

        }
        //else
        //  promptInput = prompt + KnowledgeFuzzifier.GenerateKnowledgeLogEntry()+"\n";
        Debug.Log($"prompt input: {promptInput}");
        if (generate)
        {
            openAiData openAIdata = new openAiData();

            openAIdata.prompt = promptInput;
            openAIdata.stop = new string[2];
            openAIdata.stop[0] = "Oki";
            openAIdata.stop[1] = "->";
            openAIdata.user = PlayerPrefs.GetString("UserId");
            string json = JsonUtility.ToJson(openAIdata);
            string response = "";
            using (UnityWebRequest www = UnityWebRequest.Put("https://api.openai.com/v1/engines/text-davinci-002/completions", json))
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
                    Debug.Log(www.error);
                }
                else
                {
                    openAiResponse openAIresponse = JsonUtility.FromJson<openAiResponse>(www.downloadHandler.text);

                    if (openAIresponse.choices.Length > 0)
                    {

                        response = openAIresponse.choices[0].text;
                        Debug.Log(response);
                        SynthesizeAudioAsync(response, true, "");

                    }
                }

            }
        }

    }


    public async Task SynthesizeAudioAsync(string s, bool generate, string base_response)
    {

        string ssml;

        bool gotAudio = false;

        //google sdk

        ssml = $" < speak >{s}</ speak > ";
        AudioConfiguration audioConfig = new AudioConfiguration("MP3", 0, 1);
        InputData input = new InputData(ssml);
        Voice voi = new Voice(PlayerPrefs.GetString("YatekLang"), PlayerPrefs.GetString("YatekLang") + "-Wavenet-D");

        GoogleSpeechBody body = new GoogleSpeechBody(audioConfig, input, voi);
        string requestBody = JsonUtility.ToJson(body);
        using (UnityWebRequest www = UnityWebRequest.Put("https://texttospeech.googleapis.com/v1/text:synthesize?key=" + LoadKeys.GOOGLE_SPEECH_API_KEY, requestBody))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }

            else
            {
                GoogleSpeechResponse response = JsonUtility.FromJson<GoogleSpeechResponse>(www.downloadHandler.text);

                File.WriteAllBytes(Application.persistentDataPath + "/somefile.mp3", Convert.FromBase64String(response.audioContent));
                gotAudio = true;


            }
        }



        if (gotAudio)
        {
            StartCoroutine(GetAudioClip());
        }



    }
    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + Application.persistentDataPath + "/somefile.mp3", AudioType.MPEG))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {

                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                SynthesisAudioSource.clip = myClip;

                SynthesisAudioSource.Play();

                startTimeOfPlayAudio = Time.time;
                if (practiceFlag)
                {
                    if (indexOfInteraction > 3)
                    {

                        stopPractice();
                    }
                    else
                    {
                        StartCoroutine(finishSpeaking());

                    }
                }
                else if (!practiceFlag)
                {
                    StartCoroutine(finishSpeaking());
                }
            }
        }
    }

    void Update()
    {
        if (practiceFlag)
        {
            if (Time.time >= nextTime)
            {
                if (sp != null && sp.active)
                {
                    userInput = sp.message;


                }

                if (StopedSpeaking(userInput))
                {
                    if (userInput != prevMessage && (SynthesisAudioSource != null && !SynthesisAudioSource.isPlaying) && userInput.Length > 1)
                    {
                        sp.active = false;
                        GetText(userInput, true, "");


                    }
                }


                text.text = userInput;
                prevMessage = userInput;
                nextTime += interval;

            }
        }


        if (interactionSpeechFlag)
        {
            if (Time.time >= nextTime)
            {
                if (sp != null && sp.active)
                {
                    if (StopedSpeaking(sp.message))
                    {
                        text.text = sp.message;
                        stopInteractionSpeech();
                        sp.enabled = false;
                        sp.enabled = true;
                    }
                }

                nextTime += interval;
            }
        }

    }
    private bool StopedSpeaking(string s)
    {
        return s.Contains("!") || s.Contains(".") || s.Contains("?");
    }
    public IEnumerator Practice()
    {
        sp.active = true;
        practiceFlag = true;
        yield return new WaitForSeconds(0.1f);
    }
    public void stopPractice()
    {
        sp.active = false;
        practiceFlag = false;
        StartCoroutine(finishSpeaking());
    }
    public IEnumerator interactionSpeech()
    {
        interactionSpeechFlag = true;
        //    print("interaction Speech");
        yield return new WaitForSeconds(0.1f);
        sp.active = true;
    }
    public void stopInteractionSpeech()
    {
        interactionSpeechFlag = false;
        StartCoroutine(finishSpeaking());
        sp.active = false;
    }

    public IEnumerator finishSpeaking()
    {

        while (true)
        {

            yield return new WaitForSeconds(0.1f);
            if ((SynthesisAudioSource.clip.length - (Time.time - startTimeOfPlayAudio)) <= 0.0f)
                if (!SynthesisAudioSource.isPlaying)
                {

                    yield return new WaitForSeconds(0.5f);
                    if (!practiceFlag)
                    {

                        index++;
                        if (index < sentences.Length)
                        {


                            if (sentences[index] == "interaction")
                            {


                                StartCoroutine(interactionSpeech());


                            }
                            else if (sentences[index] == "practice")
                            {
                                StartCoroutine(Practice());
                            }
                            else
                            {
                                SynthesizeAudioAsync(sentences[index], true, "");

                                if (index == sentences.Length - 1)
                                {
                                    StartCoroutine(GoBackToHub());
                                }
                               /* if (index == 10 && PlayerPrefs.GetString("notFirstTime") != "yes")
                                {

                                    SceneManager.LoadScene(2, LoadSceneMode.Single);

                                }
                               */

                            }

                        }
                    }
                    else
                    {
                        sp.active = true;
                    }
                    break;

                }
        }
    }
    IEnumerator GoBackToHub()
    {
        yield return new WaitForSeconds(HubReturnTime);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

}
