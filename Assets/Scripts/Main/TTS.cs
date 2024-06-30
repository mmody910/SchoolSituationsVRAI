using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

public class TTS : MonoBehaviour
{
    public string startSpeech;
    public Speech[] speeches;
    public Speech currentSpeech;
    public string Voice = "D";
    public keysController keys;
    public AudioSource SynthesisAudioSource;
    private RecognizeSpeech recognizeSpeech;

    bool practice = false;
    float interval = 0.1f;
    float nextTime = 0;
    public string prevMessage = "";
    public string message = "";
    bool stopped;
    private float startTimeOfPlayAudio;
    public bool english=true;
    void Start()
    {
        recognizeSpeech = GetComponent<RecognizeSpeech>();
        if (english) { 
        PlayerPrefs.SetString("YatekLang","en-US");
        }
        else
        {
            PlayerPrefs.SetString("YatekLang", "nb-NO");

        }
        LoadKeys.Load(keys);
        PlayTTS(startSpeech);
    }

    private bool StoppedSpeaking(string s)
    {
        if (s.Length > 0)
        {
            return s[s.Length - 1] == '.' || s[s.Length - 1] == '?' || s[s.Length - 1] == '!';
        }
        else
        {
            return true;
        }
    }
    private void Update()
    {
        if (practice) {
            if (Time.time >= nextTime)
            {
                if (recognizeSpeech != null && recognizeSpeech.active)
                {
                    message = recognizeSpeech.getMessage();

                }

                // check if we have a message
                if (message.Length > 0&& StoppedSpeaking(message))
                {
                    print(message.Length);
                    print(StoppedSpeaking(message));

                    practice = false;
                    StartCoroutine(finishSpeaking());
                }

                nextTime = Time.time + interval;
            }
        }
    }
    public void PlayTTS(string desiredSpeech)
    {
        foreach (Speech S in speeches)
        {
            if (S.name == desiredSpeech)
            {

                currentSpeech = null;
                currentSpeech = new Speech();
                currentSpeech = S;
               // currentSpeech.resetIndex();
                SynthesizeAudioAsync(currentSpeech.getLine(), true, "");
                break;
            }
        }
    }

    public async Task SynthesizeAudioAsync(string s, bool generate, string base_response)
    {
        // Check if string is empty
        if (s == "")
        {
            StartCoroutine(finishSpeaking());
            return;
        }
        else if (int.TryParse(s, out int digit))
        {
            StartCoroutine(finishSpeaking(digit));
            return;
        }
        else if (s=="PRACTICE")
        {
            practice = true;
            recognizeSpeech.enabled = true;
            recognizeSpeech.active= true;
            return;
        }
        //print("sentence : " + s);

        string ssml;

        bool gotAudio = false;

        //google sdk

        ssml = $" < speak >{s}</ speak > ";
        AudioConfiguration audioConfig = new AudioConfiguration("MP3", 0, 1);
        InputData input = new InputData(ssml);
        Voice voi = new Voice(PlayerPrefs.GetString("YatekLang"), PlayerPrefs.GetString("YatekLang") + "-Wavenet-" + Voice.ToUpper());

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

                StartCoroutine(finishSpeaking());
            }
        }
    }

    public IEnumerator finishSpeaking(float secondsToWait = 0.0f)
    {

        while (true)
        {
            if (secondsToWait != 0.0)
            {
                yield return new WaitForSeconds(secondsToWait);
                currentSpeech.invokeFinishEvent();
                break;

            }
            else { 

            yield return new WaitForSeconds(0.1f);
            if ((SynthesisAudioSource.clip.length - (Time.time - startTimeOfPlayAudio)) <= 0.0f)
                if (!SynthesisAudioSource.isPlaying)
                {
                        
                    yield return new WaitForSeconds(0.2f);

                    if (currentSpeech.isFinished())
                    {
                        currentSpeech.invokeFinishEvent();
                    }
                    else
                    {
                        yield return new WaitForSeconds(secondsToWait);
                        SynthesizeAudioAsync(currentSpeech.getLine(), true, "");
                    }
                   
                    break;

                }
        }
        }
    }

}


[System.Serializable]
public class Speech
{
    public Speech()
    {
    currentIndex = 0;

}
    public string name;
    public string[] en;
    public string[] nr;
    public UnityEvent finishEvent;
    private int currentIndex = 0;

    public string getLine()
    {
        string language = PlayerPrefs.GetString("YatekLang");
        string line = "";
        if (currentIndex<en.Length) { 
        if (language == "en-US")
        {
            line = en[currentIndex];
        }
        else
        {
            line = nr[currentIndex];
        }

            currentIndex++;
        }
        else
        {
            invokeFinishEvent();

        }
        return line;
    }
    public void resetIndex()
    {
        currentIndex = 0;
    }
    public bool isFinished()
    {

        string language = PlayerPrefs.GetString("YatekLang");

        if (language == "en-US" && currentIndex >= en.Length)
        {
            currentIndex = 0;
            return true;
        }
        else if ( currentIndex >= nr.Length)
        {
            currentIndex = 0;
            return true;
        }

        return false;
    }

    public void invokeFinishEvent()
    {
        if (finishEvent != null)
        {
            finishEvent.Invoke();
        }
    }
}