using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Linq; 
using CrazyMinnow.SALSA;


public class InteractionManager : MonoBehaviour
{ /*
    int interval = 1;
    float nextTime =0;
    string prevMessage = "";
    string message = "-";
    bool stopped = false;
    bool isSynthesizing = false;

    bool delayAnimationIsWorking = false;
    public AudioSource audioSource;
    private string[] ListeningClips ={"BoredTrigger","SurprisedTrigger", "ConfrontationalTrigger","AttentiveTrigger"};
    private string[] shortWords = { "ok.", "Hmm", "aha", "Right", "Yeah" };
    int index = 0;

    Animator anim;
    SynthesizeSpeech synthesizeSpeech;
    public RecognizeSpeech recognizeSpeech;
    public RecognizeSpeechOnboarding recognizeSpeechOnboarding;
    GeneratedText generatedText;
    // Emoter emoter;

    // Start is called before the first frame update


    void Start()
    {
        anim = GetComponent<Animator>();
        synthesizeSpeech = GetComponent<SynthesizeSpeech>();
        generatedText = GetComponent<GeneratedText>();
       // emoter = GetComponent<Emoter>();
        



    }
    void disableDelayAnimationIsWorking()
    {
        delayAnimationIsWorking = false;
    }
    bool intro = true;
    void Update()
    {   
        // Read player input every second.
        if (Time.time >= nextTime)
        {
            if (recognizeSpeech != null) { 
            message = recognizeSpeech.getMessage();
            isSynthesizing = !recognizeSpeech.active;
            }
            if (recognizeSpeechOnboarding != null)
            {
                message = recognizeSpeechOnboarding.getMessage();
                isSynthesizing = !recognizeSpeechOnboarding.active;
            }
            stopped = StopedSpeaking(message);



            if (intro)
            {
                Interact(message, true,intro);

                intro = false;
            }else if (message!=prevMessage && !audioSource.isPlaying && message.Length>1)
       {
               
                // Player has stopped speaking
                if (stopped) 
            {
                    if (!isSynthesizing)
                    {
                        isSynthesizing = true;
                        // Interact(shortWords[Random.RandomRange(0, shortWords.Length - 1)], false);

                    }
                    
                    Interact(message, true);





                }
                else
                {
                    // if (!delayAnimationIsWorking)
                    // {
                    //     delayAnimationIsWorking = true;
                    //     //generate random animation
                    //     int ran = Random.RandomRange(1, 30)%4;
                    //     if (ran == 0)
                    //     {
                    //         ran++;
                    //     }
                    //     string rand = ran.ToString();
                    //     anim.SetTrigger(rand);
                    //     Invoke("disableDelayAnimationIsWorking", 2.5f);
                    //     //anim.SetTrigger(ListeningClips[Random.Range(0,ListeningClips.Length)]);
                    // }

                }

            }

       prevMessage=message;
       nextTime += interval;
          }
        } 
    
    bool AnimatorIsPlaying(){
     return anim.GetCurrentAnimatorStateInfo(0).length >
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
  }
    private bool StopedSpeaking(string s)
    {
        return s.Contains("!") || s.Contains(".") || s.Contains("?");
    }

    private bool IsQuestion(string s)
    {
        return s.Contains("?");
    }

     public async Task Interact(string playerInput, bool generate=true,bool intro=false)
{
    
    if(!intro){

    await generatedText.GetText(playerInput, generate);
    string s = generatedText.GetResponse().ToString();
    string [] promptLines = new string[] {""};
    if (recognizeSpeech!=null){
        message = recognizeSpeech.getMessage();
    }
    else{
        message = recognizeSpeechOnboarding.getMessage();
    }
    print("message:" +message);
    print("playerinput: " +playerInput);

    // Move this to main interaction loop as it won't get called here.
    // if(synthesizeSpeech.SynthesisAudioSource.isPlaying){
    // print("this shit got triggered");
    // synthesizeSpeech.SynthesisAudioSource.loop=false;
    // synthesizeSpeech.SynthesisAudioSource.Stop();

    if(message==playerInput){
     await synthesizeSpeech.SynthesizeAudioAsync(s,generate);
    }
    else{
        print("detected interuption");
        string prompt = generatedText.GetPrompt();
        string [] responses = prompt.Split(':');
        

        foreach(string response in responses)
        {
            print(response);
        }
        print(responses);
        string newPrompt = string.Join(":",responses.Take(responses.Length-1)); 
        print("new prompt");
        print(newPrompt);
        generatedText.SetPrompt(newPrompt);
        }

    }
    else{
        await synthesizeSpeech.SynthesizeAudioAsync(playerInput,generate);
    }

    }
    */
}
