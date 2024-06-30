using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class scenarioController : MonoBehaviour
{
    public TextMeshProUGUI title;
    scenario scenarioData;
    private void OnEnable()
    {

        GameObject.Find("Download/Play").transform.GetChild(0).gameObject.SetActive(false);
    }
    public void setData(scenario s)
    {
        string language = PlayerPrefs.GetString("YatekLang");
        scenarioData = s;
        title.text = language== "en-US"? s.en_title: s.nb_title;

    }
    public void setDataImage(scenario s)
    {
        scenarioData = s;
        gameObject.GetComponent<LoadTextureFromURL>().load(s.imageUrl);
    }
    public void updateScenarioDetails()
    {
        GameObject scenarioDetails = GameObject.FindGameObjectWithTag("scenarioContainer").transform.GetChild(0).gameObject;
        scenarioDetails.SetActive(true);
        scenarioDetails.GetComponent<scenarioDetailsController>().updateData(scenarioData);
    }
    public void updateScenarioDetailsImageOnly()
    {
        Debug.Log(GameObject.FindGameObjectWithTag("scenarioContainer"));
        GameObject.FindGameObjectWithTag("scenarioContainer").GetComponent<scenarioDetailsController>().updateData(scenarioData);   
    }
    public scenario getScenarioData()
    {
        return scenarioData;
    }

}
