using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class promptsController : MonoBehaviour
{
    public groupController[] groups;
    public bool testData = false;
    public string test;
    void Awake()
    {
        Dictionary<string, Prompt> promptDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Prompt>>(PlayerPrefs.GetString("YatekPrompts"));
        if (testData)
        {
            promptDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Prompt>>(test);
        }
        //Dictionary<string, Prompt> promptDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Prompt>>(test);
       /* foreach (KeyValuePair<string, Prompt> kvp in promptDic)
        {
            print(kvp.Key);
            print(kvp.Value);
        }
       */
        foreach (groupController group in groups)
        {
            if (group.id.Length > 0)
            {
                if (promptDic.ContainsKey(group.id))
                {
                    print(group.id);
                    group.GetComponent<AIManager>().fetchPrompt(promptDic[group.id]);
                }
            }
        }
         
    }

}
