using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RANDOM : StateMachineBehaviour {

   public string m_parametersName = "RandomState";
    public int[] m_stateIDArray = { 0, 1, 2, 3, 4, 5, 6, 7 };

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //Official comment translation: Call OnStateEnter before calling OnStateEnter on any state in this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_stateIDArray.Length <= 0)
        {
            animator.SetInteger(m_parametersName, 0);
        }
        else
        {
            int index = Random.Range(1, m_stateIDArray.Length);
            animator.SetInteger(m_parametersName, m_stateIDArray[index]);
        }
    }
}
