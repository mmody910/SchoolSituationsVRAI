using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingIntroController : MonoBehaviour
{
    public Animator oki;
    public Animator controllers;
    // Start is called before the first frame update
    void Start()
    {
        oki = GetComponent<OnboardingController>().okiAnimator;
        controllers = GetComponent<OnboardingController>().controllerAnimator;  
    }

    // Update is called once per frame
    void Update()
    {
        if (oki.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("1"))
        {
            controllers.SetBool("Started", true);
            this.enabled = false;
        }

    }
}
