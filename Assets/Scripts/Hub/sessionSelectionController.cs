using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sessionSelectionController : MonoBehaviour
{
    public string sessionTag = "";
    public Button[] levelBtn;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < levelBtn.Length; i++)
        {
            levelBtn[i].interactable = PlayerPrefs.GetInt(sessionTag + i, 0) == 0 ? false : true;
        }
        for (int i = 0; i < levelBtn.Length; i++)
        {
            if(!levelBtn[i].interactable)
            {
                levelBtn[i].interactable = true;
                return;

            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
