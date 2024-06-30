using UnityEngine;
using TMPro;

public class LocalizeText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    [TextArea] public string EnglishText;
    [TextArea] public string NorwegianText;

   
    public void setText(string language)
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();

        //print(textMeshPro.name);
        if (language == "en-US")
        {
            textMeshPro.text = EnglishText;
        }
        else if (language == "nb-NO")
        {
            textMeshPro.text = NorwegianText;
        }
    }
}
