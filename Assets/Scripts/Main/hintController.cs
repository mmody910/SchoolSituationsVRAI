using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hintController : MonoBehaviour
{

    public GameObject[] tasks;
    public Animator[] animators;
    public void changeStatus(int index)
    {
        tasks[index - 1].SetActive(true);
        if (!isDone) { 
            end();
        }
    }
    bool isDone = false;
    void end()
    {
        foreach (GameObject task in tasks)
        {
            if (!task.activeInHierarchy)
            {
                return;
            }
        }
        isDone = true;
        foreach(Animator anim in animators)
        {
            anim.SetTrigger("Ending");

        }
    }

  
}

