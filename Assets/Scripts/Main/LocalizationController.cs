using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    public bool englishSelected = false;
    [InspectorName("first index for English and the second for the Norwegian")]
    public LocalizeText[] localizedTexts;
    private void Awake()
    {
        localizedTexts = (LocalizeText[]) Resources.FindObjectsOfTypeAll(typeof(LocalizeText));

        if (PlayerPrefs.GetString("YatekLang") == "en-US")
        {
            englishSelected = true;
        }
        else if (PlayerPrefs.GetString("YatekLang") == "nb-NO")
        {
            englishSelected = false;
        }
        else
        {
            englishSelected = true;
            PlayerPrefs.SetString("YatekLang", "nb-NO");
        }

        updateTextLanguage();
    }
    public void updateTextLanguage()
    {
        string language = PlayerPrefs.GetString("YatekLang");

        foreach (LocalizeText x in localizedTexts)
        {
            x.setText(language);
        }
    }
}
