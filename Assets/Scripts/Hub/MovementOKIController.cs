
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

public class MovementOKIController : MonoBehaviour
{
    // Start is called before the first frame update
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
    public bool interactionFlag = false;
    private InputDevice rightDevice, leftDevice;
    public CapsuleCollider target1;
    public CapsuleCollider target2;

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
        PlayerPrefs.SetInt("FinishedLocomotion", 1);
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

    void Update()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPress);
        if (menuButtonPress)
        {
            if (index >= 10)
            {
                /*
                if (PlayerPrefs.GetString("notFirstTime") == "yes")
                {

                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                }
                else
                {
                    SceneManager.LoadScene(3, LoadSceneMode.Single);
                }
                */
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            
        }
        }
    }



    public IEnumerator interaction()
    {
        interactionFlag = true;
        yield return new WaitForSeconds(0.1f);
    }
    public void stopInteraction()
    {
        interactionFlag = false;
        StartCoroutine(finishSpeaking());
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
                    index++;
                    if (index < sentences.Length)
                    {


                        if (sentences[index] == "interaction")
                        {

                            if (index == 4)
                            {
                                target1.enabled = true;
                            }
                            else if (index == 8)
                            {
                                target2.enabled = true;

                            }

                            StartCoroutine(interaction());
                        }

                        else
                        {
                            SynthesizeAudioAsync(sentences[index], true, "");

                            if (index == 3 || index == 9)
                            {
                                Invoke("vibrateLeft", 1f);
                            }
                            else if (index == 7)
                            {
                                Invoke("vibrateRight", 2f);
                            }
                        }
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
