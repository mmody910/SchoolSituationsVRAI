using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_CONVO_FADE : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        myAnimator.SetTrigger("SILENCE");
    }

    void OnTriggerExit(Collider other)
    {
        myAnimator.SetTrigger("CONVO");
    }

}
