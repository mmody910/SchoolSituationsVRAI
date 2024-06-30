using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstTimeController : MonoBehaviour
{
    private void Start()
    {

        

    }
    public void setFirstTime()
    {
        GameObject.Find("StoredkeysController").GetComponent<StoredkeysController>().setKey("notFirstTime", "yes");
       // PlayerPrefs.SetString("notFirstTime", "yes");
        //print(PlayerPrefs.GetString("notFirstTime"));
    }
}
