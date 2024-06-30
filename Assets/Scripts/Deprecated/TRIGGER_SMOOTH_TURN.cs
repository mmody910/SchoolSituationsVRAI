using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_SMOOTH_TURN : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        myAnimator.SetTrigger("FADE_IN");
    }

    void OnTriggerExit(Collider other)
    {
        myAnimator.SetTrigger("FADE_OUT");
    }

}
