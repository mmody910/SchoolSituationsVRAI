


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;
using TMPro;

using UnityEngine.XR;
public class OKIController : MonoBehaviour
{
    public bool speechFlag = false, LocomotionFlag = false,menuFlag=false;
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
    public bool interactionFlag = false;

    [HideInInspector]
    public bool interactionSpeechFlag = false;
    public TextMeshProUGUI text;
    private InputDevice rightDevice, leftDevice;
    public string promptEng;
    public string promptNor;
    string prompt;
    int indexOfInteraction = 0;
    float nextTime = 0;
    public RecognizeSpeech sp;
    string userInput = "";
    string prevMessage = "";
    int interval = 1;
    // Start is called before the first frame update

    public CapsuleCollider target1;
    public CapsuleCollider target2;
    public PauseMenuController pauseController;

    public Button[] btns;
    key loadedData;
    /*
      public CapsuleCollider target3;
      public SphereCollider target4;
      public SphereCollider target5;
      */
    void Awake()
    {
        loadedData = GameObject.Find("StoredkeysController").GetComponent<StoredkeysController>().loadedData;
        LoadKeys.Load(keys);
       // PlayerPrefs.SetString("YatekLang", "en-US");
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
       // PlayerPrefs.SetString("notFirstTime" ,"ydasdes");
       
            pauseController.enabled = true;
       
      

        SynthesizeAudioAsync(sentences[0], true, "");
        detectControllers();
    }
    void detectControllers()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (var item in devices)
        {
            if (item.name.Contains("Right"))
            {
                rightDevice = item;
                continue;
            }
            else if (item.name.Contains("Left"))
            {
                leftDevice = item;
                continue;
            }
        }
    }
    public async Task GetText(string playerInput, bool generate, string base_response)
    {
        indexOfInteraction++;
        Debug.Log("start generated : " + Time.time);
        string promptInput = "";
        //if (playerInput.Length > 0)
        // Remove knowledge fuzzifier for now, it is causing prompts to break.
        // promptInput = prompt + $"\n{talker[talker.Count() - 1]} : {playerInput}\n" + KnowledgeFuzzifier.GenerateKnowledgeLogEntry(speechManager);
        if (language == "en-US") { 
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


    public async Task SynthesizeAudioAsync( string s, bool generate, string base_response)
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
                if (practiceFlag) {
                    if (indexOfInteraction > 3)
                    {

                        stopPractice();
                    }
                    else
                    {
                        StartCoroutine(finishSpeaking());

                    }
                }
                else if(!practiceFlag&&!interactionFlag)
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
                // print(prevMessage);
                //print(userInput);
               
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

        
        if (interactionSpeechFlag&&speechFlag)
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
        
        if (LocomotionFlag)
        {
            
            leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPress);
            if (menuButtonPress) { 
                if(index >= 10) {
                    if (loadedData.keys["notFirstTime"]=="yes")
                    {

                        SceneManager.LoadScene(0, LoadSceneMode.Single);
                    }
                    else
                    {
                        SceneManager.LoadScene(3, LoadSceneMode.Single);
                    }
                }
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
       // print("pra");
        practiceFlag = true;
        yield return new WaitForSeconds(0.1f);
    }
    public void stopPractice()
    {
       // print("stop practice");
        sp.active = false;
        practiceFlag = false;
        StartCoroutine(finishSpeaking());
    }
    public IEnumerator interactionSpeech()
    {
        interactionSpeechFlag = true;
    //    print("interaction Speech");
        yield return new WaitForSeconds(0.1f);
        sp.active=true;
    }
    public void stopInteractionSpeech()
    {
   //     print("stop interaction Speech");
        interactionSpeechFlag = false;
        StartCoroutine(finishSpeaking());
        sp.active = false;
    }
    public IEnumerator interaction()
    {
     //   print("interaction");
        interactionFlag = true;
        yield return new WaitForSeconds(0.1f);
    }
    public void stopInteraction()
    {
       // print("stop interaction");
        interactionFlag = false;
        StartCoroutine(finishSpeaking());
    }
    /*public void practice()
    {
        practiceFlag = true;
        GetComponent<InteractionManager>().enabled = true;
        GetComponent<SynthesizeSpeech>().enabled = true;
        GetComponent<OKIController>().enabled = false;
    }

    public void stopPractice()
    {
        practiceFlag = false;
        GetComponent<InteractionManager>().enabled = false;
        GetComponent<SynthesizeSpeech>().enabled = false;
    }*/
    public IEnumerator finishSpeaking()
    {
       
        while (true)
            {

                yield return new WaitForSeconds(0.1f);
                if ((SynthesisAudioSource.clip.length - (Time.time - startTimeOfPlayAudio)) <= 0.0f)
                    if (!SynthesisAudioSource.isPlaying)
                    {

                        yield return new WaitForSeconds(0.5f);
                    if (!practiceFlag) {

                        index++;
                        if (index < sentences.Length)
                        {

                            if (LocomotionFlag && loadedData.keys["notFirstTime"] == "yes" && index > 18)
                            {
                                continue;
                            }
                            if (LocomotionFlag && loadedData.keys["notFirstTime"]!="yes" && index == 17)
                            {
                                index += 2;
                            }

                            if (sentences[index] == "interaction")
                            {
                                if (LocomotionFlag == true)
                                {
                                    if (index == 4)
                                    {
                                        target1.enabled = true;
                                    }
                                    else if (index == 8)
                                    {
                                        target2.enabled = true;

                                    }
                                   /* else if (index == 10)
                                    {
                                        target3.enabled = true;

                                    }
                                    else if (index == 13)
                                    {

                                        target4.enabled = true;
                                    }
                                    else if (index == 16)
                                    {

                                        target5.enabled = true;
                                    }
                                   */
                                }
                                if (menuFlag == true)
                                {
                                    if (index == 2)
                                    {
                                        btns[0].enabled= true;
                                    }
                                    else if (index == 5)
                                    {
                                        btns[1].enabled = true;

                                    }
                                    else if (index == 7)
                                    {
                                        btns[2].enabled = true;

                                    }
                                    else if (index == 9)
                                    {

                                        btns[3].enabled = true;
                                    }
                                    else if (index == 12)
                                    {

                                        btns[4].enabled = true;
                                    }

                                }
                               // print(sentences[index] + "inside");
                                if (speechFlag)
                                {
                                    StartCoroutine(interactionSpeech());

                                }
                                else
                                {
                                    StartCoroutine(interaction());

                                }

                            }
                            else if (sentences[index] == "practice")
                            {
                                StartCoroutine(Practice());
                            }
                            else
                            {
                             //   print(sentences[index]);
                                SynthesizeAudioAsync(sentences[index], true, "");
                                if (LocomotionFlag)
                                {
                                    if (index == 3 || index == 9)
                                    {
                                        Invoke("vibrateLeft", 1f);

                                    }
                                    else if (index == 7)
                                    {

                                        Invoke("vibrateRight", 2f);

                                    }


                                }
                                else if (speechFlag)
                                {
                                    if (index == 10 && loadedData.keys["notFirstTime"] != "yes")
                                    {

                                        SceneManager.LoadScene(2, LoadSceneMode.Single);

                                    }
                                }

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
    void vibrateRight()
    {
        rightDevice.SendHapticImpulse(0, 0.5f, 1.0f);

    }
    void vibrateLeft()
    {
        leftDevice.SendHapticImpulse(0, 0.5f, 1.0f);

    }

}
