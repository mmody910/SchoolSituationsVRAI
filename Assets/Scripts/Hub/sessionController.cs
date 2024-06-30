using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sessionController : MonoBehaviour
{
    public Animator animator;
    public bool hideSplash = false;
    private void Awake()
    {
        if (hideSplash)
        {
            animator.SetTrigger("play");

        }
        else
        {
            animator.SetTrigger("start");

        }
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }
    void endSession()
    {
        animator.SetTrigger("endSession");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
