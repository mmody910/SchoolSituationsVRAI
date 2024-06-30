using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
public class splashScreenController : MonoBehaviour
{
    public LoadTextureFromURL[] images;
    public TextMeshProUGUI title;
    public TextMeshProUGUI shortDescription;
    public Camera cam;
    splashScreenData splashdata;
    bool generatedPrompts = false;
    bool Production = false;
    public Animator fader;
    private void Awake()
    {
        //PlayerPrefs.SetString("YatekLang", "en-US");
        if (assetbundleContainer.instance.asset != null) {
            assetbundleContainer.instance.asset.Unload(true);
        }



        splashdata = Newtonsoft.Json.JsonConvert.DeserializeObject<splashScreenData>(PlayerPrefs.GetString("YatekSplashScreenData"));

        images[0].load(splashdata.splashImageA);
        images[1].load(splashdata.splashImageB);
        images[2].load(splashdata.splashImageC);
        images[3].load(splashdata.splashImageD);
        images[4].load(splashdata.splashBackground);
        Color newCol;
        ColorUtility.TryParseHtmlString(splashdata.splashColor, out newCol);
        cam.backgroundColor = newCol;
        images[4].gameObject.GetComponent<Renderer>().material.SetColor("_Color", newCol);
        title.text = splashdata.title;
        shortDescription.text = splashdata.shortDescription;
      
    }
    AssetBundle myLoadedAssetBundle;
    IEnumerator Start()
    {
        generatedPrompts = false;
        StartCoroutine(getToken());
        // print(Application.persistentDataPath);


        //AssetBundle levels = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, scenarioData.sceneName));
        //var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.persistentDataPath, "presentation_1"));
        // print(splashdata.sceneName + splashdata.bundleNumber);
        print(splashdata.bundleNumber);
        print(splashdata.sceneName);

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.persistentDataPath + "/bundles", splashdata.sceneName+splashdata.bundleNumber));
        yield return bundleLoadRequest;

        //myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        assetbundleContainer.instance.asset= bundleLoadRequest.assetBundle;

        if (assetbundleContainer.instance.asset == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }
        InvokeRepeating("loadscene", 1f,1f);
        //StartCoroutine(LoadAsyncScene("presentation_1"));
    }

    private IEnumerator getToken()
    {

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        Task<string> t = user.TokenAsync(true);
        while (!t.IsCompleted)
            yield return new WaitForEndOfFrame();

        generatePrompt(t.Result, PlayerPrefs.GetString("YatekLang"), PlayerPrefs.GetString("YatekScenarioId"));

      //  getAllScenariosData(t.Result);


    }
    public async Task generatePrompt(string token,string lang,string scenarioId)
    {

        lang = lang.Substring(0, 2);
      /*
        print(lang);
        print(scenarioId);
        print(token);
      */
        string apiUrl;
        if(Production)
        {
        apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/generatePrompt";
        }
        else {
            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/generatePrompt";
        }



        print(apiUrl);
        print(Application.persistentDataPath);
        WWWForm form = new WWWForm();
        form.AddField("language", lang);
        form.AddField("scenarioId", scenarioId);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                generatedPrompts = true;
                //show alert with you have to pair from the mobile app first
                Debug.LogError(www.error);

                Debug.LogError(www.downloadHandler.text);
             
            }
            else
            {
                print(www.downloadHandler.text);
                AIPromptResponse data = JsonUtility.FromJson<AIPromptResponse>(www.downloadHandler.text);
                //AIPromptResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<AIPromptResponse>(www.downloadHandler.text);
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);
                // AIPromptResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<AIPromptResponse>(www.downloadHandler.text);

                Dictionary<string, Prompt> promptDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Prompt>>(PlayerPrefs.GetString("YatekPrompts"));
                foreach (AIPromptIntro x in data.generatedPromptsWithIntro)
                {
                    print(lang);
                    print(x.characterUid);
                    print(promptDic.Count);
                    if (lang == "en")
                    {
                        promptDic[x.characterUid].prompt.en = x.generatedPrompt;
                        promptDic[x.characterUid].intro.en = x.intro;
                        print(promptDic[x.characterUid].prompt.en);
                        print(promptDic[x.characterUid].intro.en);
                    }
                    else
                    {
                        promptDic[x.characterUid].prompt.nb = x.generatedPrompt;
                        promptDic[x.characterUid].intro.nb = x.intro;
                        print(promptDic[x.characterUid].prompt.nb);

                    }
                }
                generatedPrompts = true;
                print(www.downloadHandler.text);
                PlayerPrefs.SetString("YatekPrompts",Newtonsoft.Json.JsonConvert.SerializeObject(promptDic));
                print(PlayerPrefs.GetString("YatekPrompts"));

            }
        }
    }

    void loadscene()
    {
        if (generatedPrompts)
        {
            fader.enabled = true;
            StartCoroutine(LoadAsyncScene(splashdata.sceneName));
        
        }
    }



    IEnumerator LoadAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

}
[System.Serializable]
public class AIPromptResponse
{
    public AIPrompt[] generatedPrompt;
    public AIPromptIntro[] generatedPromptsWithIntro;
}

[System.Serializable]
public class AIPrompt
{
    public string characterUid;
    public string generatedPrompt;
    
}
[System.Serializable]
public class AIPromptIntro
{
    public string characterUid;
    public string generatedPrompt;
    public string intro;
}
