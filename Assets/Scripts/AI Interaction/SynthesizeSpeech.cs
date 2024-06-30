using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.Networking;
// using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Linq;
using System.IO;
using System;
using System.Net;

public class SynthesizeSpeech : MonoBehaviour
{
    RecognizeSpeech recognizeSpeech;
    GroupInteractionManager gim;

    [HideInInspector] public AudioSource SynthesisAudioSource;
    private SpeechSynthesizer synthesizer;
    string language = "";
    [HideInInspector] public string voiceGoogle = "";
    public bool isRobot = false;
    private float startTimeOfPlayAudio;
    public string voiceNameNorwegian = "";
    public string voiceName = "";
    [HideInInspector] public string intro;
    [HideInInspector] public string introEng;
    [HideInInspector] public string introNor;

    public Queue<string> statementQueue = new Queue<string>();

    private void Awake()
    {
        recognizeSpeech = transform.parent.parent.gameObject.GetComponent<RecognizeSpeech>();
        //  recognizeSpeech = GameObject.Find("SpeechObject").GetComponent<RecognizeSpeech>();
        gim = recognizeSpeech.GetComponent<GroupInteractionManager>();
    }

    void Start()
    {
        /*
        Prompt p = Newtonsoft.Json.JsonConvert.DeserializeObject<Prompt>(PlayerPrefs.GetString("YatekPrompts"));
        //Debug.Log();
        setIntro(p);
        generatedText = GetComponent<GeneratedText>();
          if (language == "en-US")
        {
            intro = introEng;
        }
        else
        {
            intro = introNor;
        }
        */
        language = PlayerPrefs.GetString("YatekLang");

        // language = "nb-NO";
        if (isRobot)
        {
            voiceGoogle = "Wavenet-F";
        }
        else
        {
            if (language == "en-US")
            {
                voiceGoogle = PlayerPrefs.GetString("YatekLang") + "-" + voiceName;
            }
            else
            {
                voiceGoogle = PlayerPrefs.GetString("YatekLang") + "-" + voiceNameNorwegian;

            }
        }
    }

    void setIntro(Prompt p)
    {
        introEng = p.intro.en;
        introNor = p.intro.nb;
    }

    int index = 0;

    public async Task SynthesizeAudioAsyncFromQueue(bool generate, string base_response)
    {
        //
        // print(statementQueue.Count);
        string s = statementQueue.Dequeue();
        // print("dequeing a statement " + s);
        bool gotAudio = false;
        // print(s);
        //google sdk
        // print(s);
        string ssml = $" < speak >{s}</ speak > ";
        //print("start google : " + Time.time);
        AudioConfiguration audioConfig = new AudioConfiguration("MP3", 0, 1);
        InputData input = new InputData(ssml);
        Voice voi = new Voice(PlayerPrefs.GetString("YatekLang"), voiceGoogle);

        GoogleSpeechBody body = new GoogleSpeechBody(audioConfig, input, voi);
        string requestBody = JsonUtility.ToJson(body);
        // Debug.Log(requestBody);
        // Debug.Log(gameObject.name);
        using (UnityWebRequest www = UnityWebRequest.Put(
                   "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + LoadKeys.GOOGLE_SPEECH_API_KEY,
                   requestBody))
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
                try
                {
                    File.WriteAllBytes(Application.persistentDataPath + "/somefile.mp3",
                        Convert.FromBase64String(response.audioContent));
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }

                gotAudio = true;
            }
        }

        if (gotAudio)
        {
            await AsyncGetAudio();
        }
        else
        {
            recognizeSpeech.active = true;
        }
    }

    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www =
               UnityWebRequestMultimedia.GetAudioClip("file:///" + Application.persistentDataPath + "/somefile.mp3",
                   AudioType.MPEG))
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

                StartCoroutine(finishSpeaking());
            }
        }
    }

    async Task AsyncGetAudio()
    {
        if (gim.FullMessage.Trim() +
            (gim.FullMessage.Trim().Contains(gim.message) ? "" : $" {recognizeSpeech.getMessage()}") ==
            gim.SynthMessage.Trim() ||
            !(gim.SynthMessage == gim.previousFullMessage && gim.previousFullMessage.Trim() != ""))
        {
            using (UnityWebRequest www =
                   UnityWebRequestMultimedia.GetAudioClip("file:///" + Application.persistentDataPath + "/somefile.mp3",
                       AudioType.MPEG))
            {
                var web = www.SendWebRequest();
                while (!web.isDone)
                {
                    // Debug.Log("We are waiting for web request to finish");
                    await Task.Yield();
                }

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                    SynthesisAudioSource.clip = myClip;
                    if ((gim.FullMessage.Trim() + (gim.FullMessage.Trim().Contains(gim.message)
                            ? ""
                            : $" {recognizeSpeech.getMessage()}")) == gim.SynthMessage.Trim() ||
                        !(gim.SynthMessage == gim.previousFullMessage && gim.previousFullMessage.Trim() != ""))
                    {
                        SynthesisAudioSource.Play();
                        gim.previousFullMessage = gim.FullMessage;
                        gim.FullMessage = "";
                        recognizeSpeech.active = false;
                        while (SynthesisAudioSource.isPlaying)
                        {
                            await Task.Yield();
                        }

                        StartCoroutine(finishSpeaking());
                    }
                    else
                    {
                        Debug.Log("Second Filter");
                        if (gim.AIManagerInstance.prompt.LastIndexOf(gim.speechManager
                                .speakers[gim.speechManager.speakers.Count - 1].serverName.Trim() + ":") > 0)
                            gim.AIManagerInstance.prompt = gim.AIManagerInstance.prompt.Substring(0,
                                gim.AIManagerInstance.prompt.LastIndexOf(gim.speechManager
                                    .speakers[gim.speechManager.speakers.Count - 1].serverName.Trim() + ":"));
                        gim.NextSpeaker();
                    }
                }
            }
        }
        else
        {
            Debug.Log("First Filter");
            if (gim.AIManagerInstance.prompt.LastIndexOf(gim.speechManager
                    .speakers[gim.speechManager.speakers.Count - 1].serverName.Trim() + ":") > 0)
                gim.AIManagerInstance.prompt = gim.AIManagerInstance.prompt.Substring(0,
                    gim.AIManagerInstance.prompt.LastIndexOf(gim.speechManager
                        .speakers[gim.speechManager.speakers.Count - 1].serverName.Trim() + ":"));
            gim.NextSpeaker();
        }
    }


    /*
    public async Task SynthesizeAudioAsync(string s, bool generate, string base_response)
    {
        print("before Synthesize : "+Time.time);
        // recognizeSpeech.stopRecognition();
        if (recognizeSpeech != null)
            recognizeSpeech.active = false; ;
        if (findSentenceController!= null)
            findSentenceController.active = false; ;
        var config = SpeechConfig.FromSubscription("f0a149eaa4f04640aaaa59114a83ff5f", "northeurope");
        config.SpeechSynthesisLanguage = language; // For example, "de-DE"
        // The voice setting will overwrite the language setting.
        // The voice setting will not overwrite the voice element in input SSML.
        config.SpeechSynthesisVoiceName = voice;
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);
        var stream = AudioOutputStream.CreatePullStream();
        var audioConfig = AudioConfig.FromStreamOutput(stream);
        synthesizer = new SpeechSynthesizer(config, audioConfig);
        // Note: if only language is set, the default voice of that language is chosen.
        var response = s;
       
        if (generate)
        {
        
            await generatedText.GetText(s, generate, base_response);
            
            response = generatedText.GetResponse().ToString();
        }


        string ssml = $"<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\"  xml:lang=\"{language}\"> <voice name=\"{voice}\"> <prosody rate=\"0%\" pitch=\"0%\"> {response} </prosody>  </voice> </speak>";
        // print("ssml : " + ssml);
        // Speaker with style attributes
        // string ssml = $"<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\"  xmlns:mstts=\"https://www.w3.org/2001/mstts\" xml:lang=\"en-US\"> <voice name=\"en-US-JennyNeural\"> <mstts:express-as style=\"chat\"> {response} </mstts:express-as> </voice> </speak>";

        //    var result = await synthesizer.SpeakSsmlAsync(ssml);
        //print("synthesizer Call Time : " + Time.time * 1000);

        // Checks result.
        using (var result = await synthesizer.SpeakSsmlAsync(ssml)) {

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var sampleCount = result.AudioData.Length / 2;
                var audioData = new float[sampleCount];

                for (var i = 0; i < sampleCount; ++i)
                {
                    audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                }

                // The output audio format is 16K 16bit mono
                var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);

                audioClip.SetData(audioData, 0);

                SynthesisAudioSource.clip = audioClip;

                print("before Synthesize : " + Time.time);
                SynthesisAudioSource.Play();
                startTimeOfPlayAudio = Time.time;
            //    print("response synthesizer Call Time : " + Time.time * 1000);
                StartCoroutine(finishSpeaking());
            }



        }
}
    */
    IEnumerator finishSpeaking()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.100f);
            if ((SynthesisAudioSource.clip.length - (Time.time - startTimeOfPlayAudio)) < 1.0f)
                if (!SynthesisAudioSource.isPlaying)
                {
                    if (recognizeSpeech != null)
                    {
                        recognizeSpeech.active = true;
                    }

                    // if (findSentenceController != null)
                    // {
                    //     findSentenceController.active = true;
                    //     if (index == (intro.Length - 1))
                    //     {
                    //         StartCoroutine(findSentenceController.openLoadedScene());
                    //     }
                    // }
                    //recognizeSpeech.startRecognition();
                    break;
                }
        }
    }
}

[System.Serializable]
public class GoogleSpeechResponse
{
    public string audioContent;
}

[System.Serializable]
public class GoogleSpeechBody
{
    public AudioConfiguration audioConfig;
    public InputData input;
    public Voice voice;

    public GoogleSpeechBody(AudioConfiguration a, InputData i, Voice v)
    {
        audioConfig = a;
        input = i;
        voice = v;
    }
}

[System.Serializable]
public class AudioConfiguration
{
    public string audioEncoding;
    public int pitch;
    public int speakingRate;

    public AudioConfiguration(string encode, int p, int s)
    {
        audioEncoding = encode;
        pitch = p;
        speakingRate = s;
    }
}

[System.Serializable]
public class InputData
{
    public string ssml;

    public InputData(string s)
    {
        ssml = s;
    }
}

[System.Serializable]
public class Voice
{
    public string languageCode;
    public string name;

    public Voice(string l, string n)
    {
        languageCode = l;
        name = n;
    }
}