using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_HUSH : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        myAnimator.SetTrigger("HUSH");
    }

    void OnTriggerExit(Collider other)
    {
        myAnimator.SetTrigger("SPEAK");
    }

}
