using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using System.Threading;

[RequireComponent(typeof(SpeechManager))]
[RequireComponent(typeof(AIManager))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(triggerController))]
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(rigController))]
public class GroupInteractionManager : MonoBehaviour
{
    float interval = 0.1f;
    float nextTime = 0;
    public string prevMessage = "";
    public string message = "";
    public string SynthMessage;
    bool stopped = false;
    bool apiCallInProgress = false;
    public float time_since_player_spoke = 0;
    private float playerSpeechTimeout;
    private float AISpeechTimeout;
    private float predictiveCallTimeOut;
    [SerializeField] public AudioSource audioSource;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;
    Dictionary<string, CharacterInteractionManager> speakers = new Dictionary<string, CharacterInteractionManager>();

    public RecognizeSpeech recognizeSpeech;

    public string FullMessage;
    public string previousFullMessage;
    [HideInInspector] public SpeechManager speechManager;
    [HideInInspector] public AIManager AIManagerInstance;
    bool intro = true;

    private void Awake()
    {
        FullMessage = "";
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        //recognizeSpeech = GameObject.Find("SpeechObject").GetComponent<RecognizeSpeech>();
        recognizeSpeech = GetComponent<RecognizeSpeech>();
        speechManager = GetComponent<SpeechManager>();
        AIManagerInstance = GetComponent<AIManager>();

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void Start()
    {
        playerSpeechTimeout = 6f;
        AISpeechTimeout = 6f;
        predictiveCallTimeOut = 3f;
        Application.targetFrameRate = 60;
        Invoke("playIntro", 1f);
        //   playIntro();
    }

    public void stopAudio()
    {
        StartCoroutine(reduceAudioVolume());
    }

    IEnumerator reduceAudioVolume()
    {
        if (audioSource != null)
        {
            while (audioSource.volume != 0)
            {
                audioSource.volume -= 0.1f;
                yield return new WaitForSeconds(0.2f);
            }

            audioSource.Stop();
            audioSource.volume = 1f;
        }
    }

    private void OnEnable()
    {
        recognizeSpeech.enabled = true;
        recognizeSpeech.active = true;
        recognizeSpeech.message = "";
        speechManager.enabled = true;
        AIManagerInstance.enabled = true;
        //print("group enabled");
        //print(gameObject.name);
        StartCoroutine(checkIfSpeaking());
        StartCoroutine(Timer());
    }

    private void OnDisable()
    {
        recognizeSpeech.enabled = false;
        recognizeSpeech.message = "";
        recognizeSpeech.active = false;
        speechManager.enabled = false;
        AIManagerInstance.enabled = false;
        //print(gameObject.name);
        StopAllCoroutines();
    }

    void Update()
    {
        if (Time.time >= nextTime)
        {
            if (recognizeSpeech != null && recognizeSpeech.active)
            {
                message = recognizeSpeech.getMessage();
                // check if speeker has spoken since last time
                if (message != prevMessage)
                {
                    time_since_player_spoke = 0;
                }
            }

            // check if we have a message
            if (message.Length > 0)
            {
                stopped = StoppedSpeaking(message);
            }

            if (message != prevMessage && (audioSource == null || !audioSource.isPlaying) && message.Length > 1)
            {
                if (stopped)
                {
                    Debug.Log(message);
                    FullMessage += $" {message.Trim()}";
                    NextSpeaker();
                    time_since_player_spoke = 0;
                }
                else
                {
                    foreach (CharacterInteractionManager c in speakers.Values)
                    {
                        c.animationDelay();
                    }
                }
            }

            prevMessage = message;
            nextTime = Time.time + interval;
            if (audioSource != null && audioSource.isPlaying)
            {
                speechManager.time_since_last_speech = 0;
            }
        }
    }

    IEnumerator Timer()
    {
        while (true)
        {
         //   Debug.Log("AI TIMER "+ speechManager.time_since_last_speech);
           // Debug.Log("PLAYER TIMER " + time_since_player_spoke);
            yield return new WaitForSecondsRealtime(1.0f);
            speechManager.time_since_last_speech += 1.0f;
            time_since_player_spoke += 1.0f;
            yield return null;
        }
    }

    // public CancellationTokenSource activeCall;
    public async void NextSpeaker(bool SkipGetText = false, bool RemainingText = false)
    {
        // activeCall = new CancellationTokenSource();
        // if (audioSource != null && audioSource.isPlaying)
        // {
        //     audioSource.Stop();
        // }
        // Debug.log( "START CALL SEQUENCE");
        apiCallInProgress = true;
        foreach (var speaker in speechManager.speakers)
        {
            if (speaker.charGO.GetComponent<SynthesizeSpeech>() != null)
                speaker.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Clear();
        }

        AIManagerInstance.speakerQueue.Clear();

        var prompt = AIManagerInstance.prompt;
        // check if the message has already been spoken if the message from the user is in the prompt already
        // todo, some cases the message will be contained in the prompt but we still want to use the message, this happens if
        // the user says something like "I want to go to the bathroom" and the prompt is "I want to go to the bathroom, where do you want to go?"
        // so we need to make the following piece of code more robust
        // we can check if the previous few lines of the prompt contain the message, if so we can assume that the message is already in the prompt

        prompt = prompt.ToLower(); // make sure the prompt is all lowercase
        var temp = message.ToLower(); // make sure the message is all lowercase
        // get the last 5 lines of the prompt
        var promptLines = prompt.Split('\n');
        var lastFewLines = "";
        if (promptLines.Length > 5)
        {
            for (int i = promptLines.Length - 5; i < promptLines.Length; i++)
            {
                lastFewLines += promptLines[i];
            }
        }
        else
        {
            lastFewLines = prompt;
        }

        // check if the last few lines of the prompt contain the message
        if (lastFewLines.Contains(temp) || temp.Length == 0 || time_since_player_spoke > 5)
        {
            //print("lastFewLines contains message or message is empty");
            // this means that we want the character to generate a response without any new user input
            // so we send empty string to the AIManager, which will generate a response without player input
            SynthMessage = "";
            speechManager.time_since_last_speech = 0;
            var temp_message = recognizeSpeech.getMessage();
            if (!SkipGetText)
            {
                //               // Debug.log( "Normal");
                await AIManagerInstance.GetText(SynthMessage, true, "");
            }
            else
            {
                if (!RemainingText)
                {
                    if (AIManagerInstance.noInputResponse != null)
                    {
                        //                       // Debug.log( "NO INPUT");
                        AIManagerInstance.AssignResponses(AIManagerInstance.noInputResponse, true, false);
                    }
                    //  else;
                    //      await AIManagerInstance.GetText(SynthMessage, true, "");
                }
                else
                {
                    if (AIManagerInstance.secondHalfResponse != null)
                    {
                        // Debug.log( "PREDICTIVE");
                        AIManagerInstance.AssignResponses(AIManagerInstance.secondHalfResponse, false, true);
                    }
                }
            }

            speechManager.time_since_last_speech = 0;
            var new_message = recognizeSpeech.getMessage();
            //print("messages " + temp_message + " message: " + new_message);
            if (temp_message != new_message && new_message.Length > 0)
            {
                //print("user has spoken while waiting for response");
                foreach (var speaker in speechManager.speakers)
                {
                    if (speaker.charGO.GetComponent<SynthesizeSpeech>() != null)
                        speaker.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Clear();
                }

                AIManagerInstance.speakerQueue.Clear();
                apiCallInProgress = false;
            }
            else
            {
                //print("user has not spoken while waiting for response");
                await speechManager.PlayNextSpeaker();
            }
            //print("messages " + SynthMessage + " message: " + message);
        }
        else
        {
            //print("prompt does not contain message");
            SynthMessage = FullMessage;
            speechManager.time_since_last_speech = 0;
            var temp_message = recognizeSpeech.getMessage();

            if (!SkipGetText)
            {
                // Debug.log( "NORMAL");
                await AIManagerInstance.GetText(SynthMessage, true, "");
            }

            speechManager.time_since_last_speech = 0;
            var new_message = recognizeSpeech.getMessage();
            //print("messages " + temp_message + " message: " + new_message);

            if (temp_message != new_message && new_message.Length > 0)
            {
                //print("user has spoken while waiting for response");
                foreach (var speaker in speechManager.speakers)
                    SynthMessage += message;
                foreach (var speaker in speechManager.speakers)
                {
                    if (speaker.charGO.GetComponent<SynthesizeSpeech>() != null)
                        speaker.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Clear();
                }

                AIManagerInstance.speakerQueue.Clear();
                apiCallInProgress = false;
            }
            else
            {
                apiCallInProgress = true;
                await speechManager.PlayNextSpeaker();
            }
        }


        apiCallInProgress = false;
    }

    async Task playIntro()
    {
        //print("play intro");
        while (AIManagerInstance.speakerQueue.Count == 0)
            await Task.Yield();
        if (AIManagerInstance.speakerQueue.Count != 0)
            await speechManager.PlayNextSpeaker();
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

    private bool IsQuestion(string s)
    {
        return s.Contains("?");
    }

    public void ActivateListeningLayer(bool listening)
    {
        StartCoroutine(listen(1, listening, 1.0f));
    }

    IEnumerator listen(int layerIndex, bool listening, float time)
    {
        List<Animator> armatures = new List<Animator>();
        foreach (var character in speechManager.speakers)
        {
            foreach (var animator in character.charGO.transform.GetComponentsInChildren<Animator>())
                if (animator.gameObject.name.ToLower().Contains("armature"))
                {
                    if (animator.layerCount >= 2)
                        armatures.Add(animator);
                }
        }

        for (float weight = listening ? 0 : time;
             (listening && weight < (time)) || (!listening && weight > 0);
             weight += (listening ? 1 : -1) * Time.smoothDeltaTime)
        {
            foreach (Animator anim in armatures)
            {
                if (anim != null)
                {
                    anim.SetLayerWeight(layerIndex, weight);
                }
            }

            yield return null;
        }
    }

    // inumerator that runs to check if there is more than 5 seconds since player spoke and more than 5 seconds since last speaker spoke
    IEnumerator checkIfSpeaking()
    {
        bool prep = false;

        while (true)
        {
            //Debug.Log( "time since last player spoke " + time_since_player_spoke);
            // Debug.Log( "time since last speaker spoke " + speechManager.time_since_last_speech);
            if (AIManagerInstance.noInputResponse == null)
            {
                if (prep == false)
                {
                    Debug.Log("Prep" + prep.ToString());
                    prep = true;
                    AIManagerInstance.GetText("", true, "");
                }

                yield return new WaitForSeconds(0.5f);
                continue;
            }

            prep = false;
            if ((time_since_player_spoke > playerSpeechTimeout) &&
                (speechManager.time_since_last_speech > AISpeechTimeout) && !apiCallInProgress)
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    continue;
                }
                else
                {
                    Debug.Log("NO INPUTS");
                    speechManager.time_since_last_speech = 0;
                    NextSpeaker(true, false);
                    apiCallInProgress = true;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator SpeechSH()
    {
        if (AIManagerInstance.secondHalfResponse == null)
            yield break;
        yield return new WaitForSeconds(predictiveCallTimeOut);
        if (speechManager.time_since_last_speech > predictiveCallTimeOut)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                while (audioSource.isPlaying)
                {
                    yield return new WaitForFixedUpdate();
                }
            }

            {
                float prevTime = 0;
                while (time_since_player_spoke < playerSpeechTimeout / 2)
                {
                    prevTime = time_since_player_spoke;
                    yield return new WaitForFixedUpdate();
                    if (time_since_player_spoke - prevTime < 0)
                    {
                        AIManagerInstance.secondHalfResponse = null;
                        yield break;
                    }
                }

                var new_message = recognizeSpeech.getMessage();
                //print("messages " + temp_message + " message: " + new_message);
                if (new_message.Length > 0 && new_message != SynthMessage)
                {
                    //print("user has spoken while waiting for response");
                    foreach (var speaker in speechManager.speakers)
                    {
                        if (speaker.charGO.GetComponent<SynthesizeSpeech>() != null)
                            speaker.charGO.GetComponent<SynthesizeSpeech>().statementQueue.Clear();
                    }

                    AIManagerInstance.speakerQueue.Clear();
                    apiCallInProgress = false;
                }
                else
                {
                    Debug.Log("COMPLETION");
                    speechManager.time_since_last_speech = 0;
                    NextSpeaker(true, true);
                    apiCallInProgress = true;
                }
            }
        }
    }
}