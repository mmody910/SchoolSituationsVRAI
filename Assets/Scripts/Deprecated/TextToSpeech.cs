using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;

public class TextToSpeech : MonoBehaviour
{
    public string startSpeech;
    public Sentence[] sentences;
    public keysController keys;
    public AudioSource SynthesisAudioSource;

    private string[] currentSentences;
    private int currentSentenceIndex = 0;
    private float startTimeOfPlayAudio;

    void Start()
    {
        LoadKeys.Load(keys);
        PlayTTS(startSpeech);
    }

    public void PlayTTS(string desiredSentence)
    {
        currentSentenceIndex = 0;
        foreach (Sentence Sn in sentences)
        {
            if (Sn.name == desiredSentence)
            {
                currentSentences = Sn.getSentences();
                SynthesizeAudioAsync(currentSentences[0], true, "");
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

                StartCoroutine(finishSpeaking());
            }
        }
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

                    currentSentenceIndex++;
                    if (currentSentenceIndex < currentSentences.Length)
                    {
                            SynthesizeAudioAsync(currentSentences[currentSentenceIndex], true, "");

                    }
                    break;

                }
        }
    }

}


[System.Serializable]
public class Sentence
{
    public string name;
    public string[] en;
    public string[] nr;

    public string[] getSentences()
    {
        string language = PlayerPrefs.GetString("YatekLang");

        if (language == "en-US")
        {
            return en;
        }
        else
        {
            return nr;
        }
    }
}
