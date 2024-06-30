using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_OPEN_CURTAIN : MonoBehaviour
{
    public Animator myAnimator;
    public GameObject curtainGlow;

    void OnTriggerEnter(Collider other)
    {
        myAnimator.SetTrigger("OPEN_CURTAIN");
        
        //disable curtain glow upon leaving the stage
        if(curtainGlow.activeSelf)
        {
            curtainGlow.SetActive(false);
        }
    }


    void OnTriggerExit(Collider other)
    {
        myAnimator.SetTrigger("CLOSE_CURTAIN");

        //enable curtain glow after entering the stage
        curtainGlow.SetActive(true);
    }
}
