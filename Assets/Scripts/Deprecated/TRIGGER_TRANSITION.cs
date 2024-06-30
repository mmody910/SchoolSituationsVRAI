using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class TRIGGER_TRANSITION : MonoBehaviour
{
    public Animator myAnimator;
    public GameObject user;
    public GameObject XROrigin;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        user.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        XROrigin.GetComponent<MovementWithKeyboard>().enabled = false;
        XROrigin.transform.position = new Vector3(-1.5f, XROrigin.transform.position.y, XROrigin.transform.position.z);
        myAnimator.SetTrigger("startSession");
    }

}
