using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

[System.Serializable]
public class Prompt
{
    public sentence intro;
    public sentence prompt;
    public sentences talkers;
}
[System.Serializable]
public class scenario
{
    public int averageMinutes;
    public int level;
    public Requirment[] requirements;
    public bool isAvailable;
    public string assetBundleUrl;
    public string imageUrl;
    public string en_smallDescription;
    public string nb_smallDescription;
    public string en_longDescription;
    public string nb_longDescription;
    public long updatedAt;
    public string en_category;
    public string nb_category;
    public string en_title;   
    public string nb_title;
    public int buildNumber;
    public string id;
    public string sceneName;
    public string splashImageA;
    public string splashImageB;
    public string splashImageC;
    public string splashImageD;
    public string splashBackground;
    public string splashColor;
    public Dictionary<string,Prompt> prompts;
}


[System.Serializable]
public class Requirment
{
    public string type;
    public sentence question;
    public List<string> answer;
    public string toString()
    {
        string context = "";
        string q = PlayerPrefs.GetString("YatekLang") == "en-US" ? question.en : question.nb;
        context = "Q: " + q + "\n";
        context += "A: " + answer[0] + "\n";
        return context;
    }
}



[System.Serializable]
public class AllQuestions
{
    public Questions[] questions;
}

[System.Serializable]
public class Questions
{
    public string uid;
    public sentence question;
}
[System.Serializable]
public class sentence
{
    public string en;
    public string nb;
}
[System.Serializable]
public class sentences
{
    public List<string> en;
    public List<string> nb;
}
[System.Serializable]
public class scenariosData
{
    public UserData userData;
    public List<scenario> scenarios;
    public AllQuestions allQuestions;
    public world worldsData;

}
[System.Serializable]
public class UserData
{
    public string firstName;
}
[System.Serializable]
public class world
{
    public List<WorldData> worldData;
}
[System.Serializable]
public class WorldData
{
    public string category_en;
    public string imageUrl;
    
}
[System.Serializable]
public class splashScreenData
{
    public string sceneName;
    public int bundleNumber;
    public string splashImageA;
    public string splashImageB;
    public string splashImageC;
    public string splashImageD;
    public string splashBackground;
    public string splashColor;
    public string title;
    public string shortDescription;

}
[System.Serializable]
public class bundlesController
{
    public Dictionary<string,int> dic;
    public bundlesController()
    {
        dic = new Dictionary<string, int>();
    }

}


public class serverComunicatorController : MonoBehaviour
{
    bundlesController downloadedBundlesController = new bundlesController();
    //like interview
    public GameObject experince;

    //vertical container
    public GameObject experinceParent;
    
    
    public scenariosData data = null;

    private bool Production = false; //Switch between production and staging
    public bool UI_new = false;
    void fetchDownloadedBundles()
    {
        downloadedBundlesController.dic.Clear();

        if (!Directory.Exists(Application.persistentDataPath + "/bundles"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/bundles");

        }

        string[] fileEntries = Directory.GetFiles(Application.persistentDataPath + "/bundles");
        for (int i = 0; i < fileEntries.Length; i++)
        {
            int ix = fileEntries[i].IndexOf("/bundles");
            if (ix != -1)
            {
                fileEntries[i] = fileEntries[i].Substring(ix + "/bundles/".Length);

                string code=( Regex.Match(fileEntries[i], @"\d+").Value);
                fileEntries[i]= fileEntries[i].Substring(0, fileEntries[i].Length - code.Length);
                
                downloadedBundlesController.dic.Add(fileEntries[i], int.Parse(code));

            }
        }
        
    }
    private void OnEnable()
    {
        print("server onenable");
        fetchDownloadedBundles();


        //PlayerPrefs.SetString("notFirstTime","yes");
        StartCoroutine(getToken());
        foreach (Transform child in experinceParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        /*
        if (PlayerPrefs.GetString("notFirstTime") == "yes")
        {
            // If there is an instance, and it's not me, delete myself.      
        }
        
        */
        
        /*
        else
        {
            updateController.enabled = true;
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }*/
    }
    private void Start()
    {
        DontDestroyOnLoad(updateController.gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private IEnumerator getToken()
    {
        print("step1");
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        print("step2");
        Task<string> t = user.TokenAsync(true);
        print("step3");
        while (!t.IsCompleted) {
            print("step x");
            yield return new WaitForEndOfFrame();
        }

        print("step1");

        print(t.Result);
        getAllScenariosData(t.Result);


    }
    private void Awake()
    {
        if(assetbundleContainer.instance != null)
        if (assetbundleContainer.instance.asset != null)
        {
            assetbundleContainer.instance.asset.Unload(true);
        }
    }
    public async Task getAllScenariosData(string token)
    {
       //print(Application.persistentDataPath);
       //print(token);
        string apiUrl;

        if(Production) apiUrl="https://us-central1-vr-mvp.cloudfunctions.net/getAllScenarios";
        else apiUrl="https://us-central1-vr-mvp-staging.cloudfunctions.net/getAllScenarios";

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, ""))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + token);
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
               ////////Debug.log(    www.error);
            }
            else
            {
                print(www.downloadHandler.text);
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);

                //print(data.scenarios[0].prompts);
                data= Newtonsoft.Json.JsonConvert.DeserializeObject<scenariosData>(www.downloadHandler.text);
                print(data.scenarios[0].prompts);

                PlayerPrefs.SetString("PlayerName", data.userData.firstName);
                print(data.userData.firstName);
                fillExperienceData2D(data);
            } 
        }
            
    }
    public autoUpdateController updateController;
    public GameObject loader;
    /*
    void fillExperinceData(scenariosData data)
    {
       //print("yes");
        var serializedQuestions = Newtonsoft.Json.JsonConvert.SerializeObject(data.allQuestions);
       //print(serializedQuestions);

      
        PlayerPrefs.SetString("YatekQuestions", serializedQuestions);

        Dictionary<string, List<scenario>> x = new Dictionary<string, List<scenario>>();
      
        foreach (var e in data.scenarios)
        {
            if (!x.ContainsKey(e.en_category))
            {
                x.Add(e.en_category, new List<scenario>());
            }
            x[e.en_category].Add(e);
        }
       
        foreach (KeyValuePair<string, List<scenario>> kvp in x)
        {
           GameObject Experince= Instantiate(experince, experinceParent.transform);

            var serializedExperince = Newtonsoft.Json.JsonConvert.SerializeObject(kvp.Value);
           //print(serializedExperince);
            Experince.GetComponent<experienceController>().fillScenarioeData(kvp.Value);
           
        }
       
        GameObject.FindGameObjectWithTag("loaderContainer").transform.GetChild(0).gameObject.SetActive(false);
    }
    */
    //Same as the above but each row has 2 scenarios not 1
    void fillExperienceData2D(scenariosData data)
    {
        var serializedQuestions = Newtonsoft.Json.JsonConvert.SerializeObject(data.allQuestions);



        PlayerPrefs.SetString("YatekQuestions", serializedQuestions);

        Dictionary<string, List<scenario>> x = new Dictionary<string, List<scenario>>();
        int downloadedVersion = 0;
        updateController.updateBundles.Clear();
        foreach (var e in data.scenarios)
        {
            if (downloadedBundlesController.dic.TryGetValue(e.sceneName, out downloadedVersion))
            {
                if (downloadedVersion < e.buildNumber)
                {
                    updateController.updateBundles.Add(new bundleData(e.sceneName + downloadedVersion, e.sceneName + e.buildNumber, e.assetBundleUrl));
                }
            }
            else
            {
                updateController.updateBundles.Add(new bundleData(e.sceneName + "0", e.sceneName + e.buildNumber, e.assetBundleUrl));
            }
            if (!x.ContainsKey(e.en_category))
            {
                x.Add(e.en_category, new List<scenario>());
            }
            x[e.en_category].Add(e);
        }
        if (updateController.updateBundles.Count > 0)
        {
            updateController.gameObject.SetActive(true);
            loader.transform.GetChild(0).gameObject .SetActive(false);
            return;

        }
        print("test test test 0");
        foreach (KeyValuePair<string, List<scenario>> kvp in x)
        {
            
            GameObject Experince = Instantiate(experince, experinceParent.transform);
            Experince.GetComponent<experienceController>().fillScenarioeData(kvp.Value,data.worldsData);

        }
      
        GameObject.FindGameObjectWithTag("loaderContainer").transform.GetChild(0).gameObject.SetActive(false);
    }

}