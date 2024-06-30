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
public class HubMenuController : MonoBehaviour
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
    public TextMeshProUGUI text;
    public Button[] btns;
    bool interactionFlag = false;


    void Awake()
    {

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
      /*  if (PlayerPrefs.GetString("notFirstTime") != "yes")
        {

            PlayerPrefs.SetString("notFirstTime", "yes");
        }
      */
        SynthesizeAudioAsync(sentences[0], true, "");
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
        print("finishSpeaking");
        while (true)
        {

            yield return new WaitForSeconds(0.1f);
            if ((SynthesisAudioSource.clip.length - (Time.time - startTimeOfPlayAudio)) <= 0.0f)
                if (!SynthesisAudioSource.isPlaying)
                {

                    yield return new WaitForSeconds(0.5f);
                    index++;
                    if (index < sentences.Length)
                    {
                        if (sentences[index] == "interaction")
                        {
                            if (index == 2)
                            {
                                btns[0].enabled = true;
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
                            StartCoroutine(interaction());

                        }
                        else
                        {
                            SynthesizeAudioAsync(sentences[index], true, "");

                        }

                    }
                    print(index);
                  

                    break;
                }        
        }
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
}
