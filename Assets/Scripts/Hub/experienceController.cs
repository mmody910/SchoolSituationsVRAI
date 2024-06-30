using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Microsoft.CognitiveServices.Speech.Speaker;
using UnityEngine.Events;

public class experienceController : MonoBehaviour
{
    public GameObject scenario, scenarioParent;
    public TextMeshProUGUI title,smallDescription;
    public string sessionTag = "";
    List<scenario> scenariosData;
    world worldData;
    public bool NewUI = false;
    private void Start()
    {
       
        PlayerPrefs.SetInt("YatekCurrentExperince", -1);
        title  =GetComponent<TextMeshProUGUI>();
        if (title == null)
            title = GetComponentInChildren<TextMeshProUGUI>();
        
        
    }
   public void goToElevator()
    {
        GameObject.Find("Situations").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("ELEVATOR").GetComponent<Animator>().SetTrigger("ELEVATE");
        GameObject.Find("SITUATION_MENU").GetComponent<Animator>().SetTrigger("MenuWindow");
        GameObject.Find("MAIN_MENU").SetActive(false);
        InstansiateScenariosElevator();

    }
    // Update is called once per frame
   /* public void refresh()
    {
        fillScenarioeData(scenariosData, worldData);
    }
   */
    public void changeExperianceLanguage()
    {   
        string language = PlayerPrefs.GetString("YatekLang");
        // language = "nb-NO";
        
        if (language == "en-US")
        {
            title.text = scenariosData[0].en_category;
            if (smallDescription != null)
                smallDescription.text = scenariosData[0].en_smallDescription;
            sessionTag = scenariosData[0].en_category;
        }
        else
        {
            title.text = scenariosData[0].nb_category;
            if (smallDescription != null)
                smallDescription.text = scenariosData[0].nb_smallDescription;
            sessionTag = scenariosData[0].nb_category;
        }
    }
    
    public void fillScenarioeData(List<scenario> scenarios, world worldsData)
    {
        scenarioParent = GameObject.FindGameObjectWithTag("scenarioContainer");
        if (scenarioParent !=null)
        foreach (Transform child in scenarioParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        scenariosData = scenarios;
        worldData = worldsData;
        scenarios.Sort((x, y) => x.level.CompareTo(y.level));
      
        string language = PlayerPrefs.GetString("YatekLang");
        // language = "nb-NO";
        if (language == "en-US")
        {
            title.text = scenarios[0].en_category;
             sessionTag = scenarios[0].en_category;
        }
        else
        {
            title.text = scenarios[0].nb_category;
            sessionTag = scenarios[0].nb_category;
        }
        /*
            foreach (WorldData x in worldsData.worldData)
            {
                if(x.category_en== scenarios[0].en_category) { 
                this.gameObject.GetComponent<LoadTextureFromURL>().load(x.imageUrl);
                break;
            }
        }
            */
        if (scenarios[0].en_category == "Work")
        {

            this.gameObject.GetComponent<LoadTextureFromURL>().setSprite(work);
        }
        else if (scenarios[0].en_category == "Social")
        {
            this.gameObject.GetComponent<LoadTextureFromURL>().setSprite(social);

        }
        else if (scenarios[0].en_category == "School")
        {
            this.gameObject.GetComponent<LoadTextureFromURL>().setSprite(school);

        }



     }


    public Sprite work, social, school;


    /*
    public void InstantiateScenarios()
    {
        foreach (scenario e in scenariosData)
        {
            GameObject Scenario = Instantiate(scenario, scenarioParent.transform);
            print(Scenario.name);
            Scenario.GetComponent<scenarioController>().setData(e);

        }
    }
    */
    public void InstansiateScenariosElevator()
    {
        scenarioParent = GameObject.FindGameObjectWithTag("scenarioContainer");
        foreach (Transform child in scenarioParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        scenarioParent.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = sessionTag;

        Invoke("fillData", .1f);

    }
    void fillData()
    {
       // print(scenarioParent.transform.childCount);
        foreach (scenario e in scenariosData)
        {

            GameObject Scenario = Instantiate(scenario, scenarioParent.transform);
            Scenario.transform.localScale = Vector3.one;

            Scenario.GetComponent<scenarioController>().setDataImage(e);
            Scenario.GetComponent<scenarioController>().setData(e);
            Scenario.GetComponent<LockLevel>().setData(e);

        }

        scenarioParent.GetComponent<scenarioDetailsController>().updateSpacing();
    }
    void fillData(Transform scenarioParent)
    {
        

    }
    public void FillDetailsParent()
    {
        //Transform detailsParent = GameObject.FindGameObjectWithTag("WorldDetails").transform;
        /*foreach (Transform c in detailsParent)
        {
            c.gameObject.SetActive(true);
        }
        string language = PlayerPrefs.GetString("YatekLang");
        smallDescription = detailsParent.Find("World_DESCRIPTION/Container/Panel/DESCRIPTION").GetComponent<TextMeshProUGUI>();
        Button playButton = detailsParent.GetComponentInChildren<Button>();
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(new UnityAction(InstansiateScenariosElevator));
        
        if (language == "en-US")
        {
            smallDescription.GetComponent<TMPro.TextMeshProUGUI>().text = scenariosData[0].en_smallDescription;
        }
        else
        {
            smallDescription.GetComponent<TMPro.TextMeshProUGUI>().text = scenariosData[0].nb_smallDescription;
        }
        */
    }
    /*
  
    public GameObject leveSelection, textInfo;
    public void interactionState()
    {
        if (!leveSelection.activeSelf)
        {
            if (PlayerPrefs.GetInt("YatekCurrentExperince") != -1)
            {
                gameObject.transform.parent.GetChild(PlayerPrefs.GetInt("YatekCurrentExperince")).gameObject.GetComponent<experienceController>().resetLevelsUI();
        }
            PlayerPrefs.SetInt("YatekCurrentExperince", transform.GetSiblingIndex());
        }
        else
        {
            PlayerPrefs.SetInt("YatekCurrentExperince", -1);

        }
       

        leveSelection.SetActive(!leveSelection.activeSelf);
        textInfo.SetActive(!textInfo.activeSelf);
    }
    public void resetLevelsUI()
    {
        leveSelection.SetActive(false);
        textInfo.SetActive(true);
    }
*/
}
