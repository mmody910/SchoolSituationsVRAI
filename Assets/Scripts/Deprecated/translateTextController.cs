using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class translateTextController : MonoBehaviour
{
    [TextArea(3, 10)]
    string text;
    [TextArea(3, 10)]
    public string textEng;
    [TextArea(3, 10)]
    public string textNor;


    // Start is called before the first frame update
    void Awake()
    {
        string language = PlayerPrefs.GetString("YatekLang");
        if (language == "en-US")
        {
            text = textEng;
        }
        else
        {

            text = textNor;
        }

        this.GetComponent<Text>().text = text;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
