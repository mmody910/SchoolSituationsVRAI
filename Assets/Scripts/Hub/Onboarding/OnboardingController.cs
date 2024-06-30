using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingController : MonoBehaviour
{
    public Animator goalAnimator;
    public Animator okiAnimator;
    public Animator controllerAnimator;
  //  public OKIController okiController;
    //public HubMenuController hubMenuController;
    private int step;
    private void Awake()
    {
        step = 0;
    }
    private void OnEnable()
    {
        nextStep(step);
    }

    private void Update()
    {
        
    }

    public void nextStep(int goalID)
    {
       //print("goal " + goalID);
        step = goalID;
        //goalAnimator.SetInteger("Goals", step);
        //okiAnimator.SetInteger("Goals", step);
        //controllerAnimator.SetInteger("Goals", step);
        goalAnimator.SetTrigger(goalID.ToString());
        okiAnimator.SetTrigger(goalID.ToString());
        controllerAnimator.SetTrigger(goalID.ToString());
      //  if(goalID!=0)
        //okiController.stopInteraction();

    }
}
