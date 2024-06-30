using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockLevel : MonoBehaviour
{
    public scenario scenarioData;
    public GameObject lockIcon;
    private string[] lockedScenarios = { "present_in_front_of_a_class", "houseparty", "coffee_machine_talk_with_colleagues", "presentation" };
    // Start is called before the first frame update
    public void setData(scenario scenarioData)
    {
      
        // Check if not finished locomotion onboarding
        if (PlayerPrefs.GetInt("FinishedLocomotion", 0) == 0)
        {
            foreach (string s in lockedScenarios)
            {
                if (s == scenarioData.sceneName)
                {
                    lockButton();
                }
            }
        }
    }
    void OnEnable()
    {
        scenarioData = gameObject.GetComponent<scenarioController>().getScenarioData();

        // Check if not finished locomotion onboarding
        if (PlayerPrefs.GetInt("FinishedLocomotion", 0) == 0)
        {
            foreach (string s in lockedScenarios)
            {
                if (s == scenarioData.sceneName)
                {
                    lockButton();
                }
            }
        }
    }

    void lockButton()
    {
        GetComponent<Button>().interactable = false ;
        lockIcon.SetActive(true);
        Debug.Log("Locked!");
    }
}
