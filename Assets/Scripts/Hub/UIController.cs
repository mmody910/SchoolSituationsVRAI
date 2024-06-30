using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    public bool englishSelected = false;
    [InspectorName("first index for English and the second for the Norwegian")]
    public string[] man;
    [InspectorName("first index for English and the second for the Norwegian")]
    public string[] woman;
    [InspectorName("first index for English and the second for the Norwegian")]
    public string[] manGoogle;
    [InspectorName("first index for English and the second for the Norwegian")]
    public string[] womanGoogle;
    public GameObject enChecker, nbChecker;
    public Transform experienceParent;
    public Transform scenarioDetails;
    public LocalizationController localizationController;
   /*
    public List<TextMeshProUGUI> texts;
    public List<string> textsEnglish;
    public List<string> textsNorwegian;
    */
    public void goBack()
    {

        Transform detailsParent = GameObject.FindGameObjectWithTag("WorldDetails").transform;
        foreach (Transform c in detailsParent)
        {
            c.gameObject.SetActive(false);
        }
        /*Button playButton = detailsParent.GetComponentInChildren<Button>();
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(new UnityAction(InstansiateScenariosElevator));
      */


    }
    private void Awake()
    {
       // PlayerPrefs.SetString("YatekLang", "");
        // for english en-AU
        //for norwegian nb-NO
        if (PlayerPrefs.GetString("YatekLang") != "en-US" && PlayerPrefs.GetString("YatekLang") != "nb-NO")
        {
            englishSelected = false;
            PlayerPrefs.SetString("YatekLang", "nb-NO");
        }else if (PlayerPrefs.GetString("YatekLang") == "en-US")
        {

            englishSelected = true;
        }
        selectLang(PlayerPrefs.GetString("YatekLang"));
        updateToggle();
    }
    public void selectLang(GameObject langs)
    {
        langs.SetActive(!langs.activeSelf);
    }
    private void OnEnable()
    {



    }
    public void selectLang(string value)
    {
        PlayerPrefs.SetString("YatekLang", value);
        if(value== "en-US")
        {
            PlayerPrefs.SetString("YatekLangMan", man[0]);
            PlayerPrefs.SetString("YatekLangWoman", woman[0]);

            PlayerPrefs.SetString("YatekLangManGoogle", manGoogle[0]);
            PlayerPrefs.SetString("YatekLangWomanGoogle", womanGoogle[0]);
            englishSelected = true;
        }
        else
        {

            PlayerPrefs.SetString("YatekLangMan", man[1]);
            PlayerPrefs.SetString("YatekLangWoman", woman[1]);

            PlayerPrefs.SetString("YatekLangManGoogle", manGoogle[1]);
            PlayerPrefs.SetString("YatekLangWomanGoogle", womanGoogle[1]);
            englishSelected = false;
        }

        updateToggle();
        changeLanguageUI();

    }
    void changeLanguageUI()
    {
        string value=PlayerPrefs.GetString("YatekLang");
        int i = 0;


        localizationController.updateTextLanguage();
        foreach (experienceController x in experienceParent.GetComponentsInChildren<experienceController>())
        {
            x.changeExperianceLanguage();
        }
        //scenarioDetails.gameObject.GetComponent<scenarioDetailsController>().changeScenarioDetailsLanguage();

    }
    public SpriteRenderer langSprite;
    public Sprite engSprite,norSprite;
    
    void updateToggle()
    {
        if (englishSelected)
        {
            enChecker.GetComponent<Image>().color = new Color32(255, 255, 225, 255); 
            nbChecker.GetComponent<Image>().color = new Color32(152, 152, 152, 255);
            langSprite.sprite = engSprite;
        }
        else
        {
            nbChecker.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            enChecker.GetComponent<Image>().color = new Color32(152, 152, 152, 255);
            langSprite.sprite = norSprite;
            print(englishSelected);

        }
       
    }
}
