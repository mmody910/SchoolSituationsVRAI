using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using CrazyMinnow.SALSA;

public class CharacterInteractionManager : MonoBehaviour
{
    public AudioSource audioSource;
    Animator anim;
    SynthesizeSpeech synthesizeSpeech;
    bool delayAnimationIsWorking = false;


    void Start()
    {

        audioSource = this.transform.GetComponentInChildren<AudioSource>();
        //audioSource = this.transform.Find(gameObject.name + "_Audio_Source").gameObject.GetComponent<AudioSource>();

        anim = GetComponent<Animator>();
        synthesizeSpeech = GetComponent<SynthesizeSpeech>();
        synthesizeSpeech.SynthesisAudioSource = audioSource;
    }
    public void animationDelay()
    {
        if (!delayAnimationIsWorking)
        {
            delayAnimationIsWorking = true;
            //generate random animation
            int ran = Random.RandomRange(1, 30)%4;
            if (ran == 0)
            {
                ran++;
            }
            string rand = ran.ToString();
            anim.SetTrigger(rand);
            Invoke("disableDelayAnimationIsWorking", 2.5f);
            //anim.SetTrigger(ListeningClips[Random.Range(0,ListeningClips.Length)]);
        }   
    }
    void disableDelayAnimationIsWorking()
    {
        delayAnimationIsWorking = false;
    }
}