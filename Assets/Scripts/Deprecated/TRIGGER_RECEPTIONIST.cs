using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_RECEPTIONIST : MonoBehaviour
{
    public Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        myAnimator.SetTrigger("LOOK_UP");
    }


    void OnTriggerExit(Collider other)
    {
        myAnimator.SetTrigger("LOOK_DOWN");
    }

}
