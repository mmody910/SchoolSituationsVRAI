using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;

using Nobi.UiRoundedCorners;
public class scenarioDetailsController : MonoBehaviour
{

    public TextMeshProUGUI title;
    public TextMeshProUGUI shortDesc;
    public TextMeshProUGUI longDesc;
    public Image image;
    public GameObject downloadBtn, openBtn;
    string titleValue, shortDescValue, longDescValue;
    scenario scenarioData;
    public bool UI_New = false;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G))
            openScene();
    }
    public void changeScenarioDetailsLanguage()
    {
        string language = PlayerPrefs.GetString("YatekLang");
        if (language == "en-US")
        {
            titleValue = scenarioData.en_title;
            longDescValue = scenarioData.en_longDescription;
            if (!UI_New)
            {
                shortDescValue = scenarioData.en_smallDescription;
            }
        }
        else
        {
            titleValue = scenarioData.nb_title;
            longDescValue = scenarioData.nb_longDescription;
            if (!UI_New)
            {
                shortDescValue = scenarioData.nb_smallDescription;
            }
        }
        title.text = titleValue;
        shortDesc.text = shortDescValue;
        longDesc.text = longDescValue;
    }

    public void updateData(scenario s)
    {
        var serializedPromts = Newtonsoft.Json.JsonConvert.SerializeObject(s);
        print(serializedPromts);
        scenarioData = s;

        print(s.prompts.Count);
        string language = PlayerPrefs.GetString("YatekLang");
        if (language == "en-US")
        {
            titleValue = scenarioData.en_title;
            longDescValue = scenarioData.en_longDescription;
             }
        else
        {
            titleValue = scenarioData.nb_title;
            longDescValue = scenarioData.nb_longDescription;
           }

        title.text = titleValue;
        //print(longDescValue);
        longDesc.text = longDescValue;
        CheckFile();
    }
    void CheckFile()
    {
        GameObject.Find("Download/Play").transform.GetChild(0).gameObject.SetActive(true);
       // print(scenarioData.sceneName + scenarioData.buildNumber.ToString());
        string path = Path.Combine(Application.persistentDataPath+"/bundles", scenarioData.sceneName+scenarioData.buildNumber.ToString());
        //print(path);
        if (!File.Exists(path))
        {
            downloadBtn.SetActive(true);
            openBtn.SetActive(false);
        }
        else
        {
            downloadBtn.SetActive(false);
            openBtn.SetActive(true);
        }
        stopDownload();
    }
    
    public void  downloadAssetBundle(GameObject loader)
    {
        StartCoroutine(DownloadFile(loader));
    }
    public void loadScene()
    {
       
        
    }
    UnityWebRequestAsyncOperation operation;
   IEnumerator DownloadFile(GameObject loader)
    {
        loader.GetComponent<Image>().fillAmount = 0f;
        UnityWebRequest downloader = UnityWebRequest.Get(scenarioData.assetBundleUrl);
        print(scenarioData.sceneName + scenarioData.buildNumber);
        string path = Path.Combine(Application.persistentDataPath+"/bundles", scenarioData.sceneName+scenarioData.buildNumber);



        //write every downlaoded byte;
        //uwr.downloadHandler = new DownloadHandlerFile(path);

        operation = downloader.SendWebRequest();
            while (!operation.isDone)
            {


                loader.SetActive(true);
                loader.GetComponent<Image>().fillAmount = downloader.downloadProgress;

              

               

                yield return null;
            }




            if (downloader.result != UnityWebRequest.Result.Success) {

            loader.SetActive(false) ;
            Debug.Log(downloader.error);
        }
        else { 
                Debug.Log("File successfully downloaded and saved to " + path);
            
            File.WriteAllBytes(path, downloader.downloadHandler.data);
            loader.SetActive(false);
            downloadBtn.SetActive(false);
            openBtn.SetActive(true);
        }

    }
    public void stopDownload()
    {
        if(operation!=null)
        operation.webRequest.Abort();
    }

    public void deleteAssetBundle()
    {
        string path = Path.Combine(Application.persistentDataPath+"/bundles", scenarioData.sceneName+scenarioData.buildNumber);

        File.Delete(path);
        if (!File.Exists(path))
        {
            downloadBtn.SetActive(true);
            openBtn.SetActive(false);
        }
    }
    
    public void openScene()
    {
        splashScreenData splashData = new splashScreenData();
        splashData.splashImageA = scenarioData.splashImageA;
        splashData.splashImageB = scenarioData.splashImageB;
        splashData.splashImageC = scenarioData.splashImageC;
        splashData.splashImageD = scenarioData.splashImageD;
        splashData.splashBackground = scenarioData.splashBackground;
        splashData.splashColor = scenarioData.splashColor;
        splashData.sceneName = scenarioData.sceneName;
        splashData.bundleNumber = scenarioData.buildNumber;
        string language = PlayerPrefs.GetString("YatekLang");
        if (language == "en-US")
        {
            splashData.title = scenarioData.en_title;
            splashData.shortDescription = scenarioData.en_smallDescription;
        }
        else
        {
            splashData.title = scenarioData.nb_title;
            splashData.shortDescription = scenarioData.nb_smallDescription;

        }
           
        var splashScreendata=Newtonsoft.Json.JsonConvert.SerializeObject(splashData);
        PlayerPrefs.SetString("YatekSplashScreenData", splashScreendata);
        string context = "";
        foreach (Requirment r in scenarioData.requirements)
        {
            if (r.type != "file")
            {
                context += r.toString();
          //      print(r.toString());
            }
            else
            {
                var listOfURLs = Newtonsoft.Json.JsonConvert.SerializeObject(r.answer);
                if (r.answer.Count==0)
                {
                    PlayerPrefs.SetString("YatekFileURL", "");

                }
                else
                {
                    PlayerPrefs.SetString("YatekFileURL", listOfURLs);

                }
                
            }
        }


        PlayerPrefs.SetString("YatekContext", context);
        var serializedPromts = Newtonsoft.Json.JsonConvert.SerializeObject(scenarioData.prompts);

        print(serializedPromts);
        PlayerPrefs.SetString("YatekPrompts", serializedPromts);
        PlayerPrefs.SetString("YatekScenarioId", scenarioData.id);
        

        //AssetBundle levels = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, scenarioData.sceneName));

        //Or levels = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath,"gameLevels"));
        //AssetBundle levels = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, scenarioData.sceneName));
        //     SceneManager.LoadScene(scenarioData.sceneName, LoadSceneMode.Single);

        SceneManager.LoadScene("SplashScreen_Generic", LoadSceneMode.Single);
    }
    public void updateSpacing()
    {
        //print(transform.childCount);
        if (transform.childCount == 5)
        {
            //GetComponent<HorizontalLayoutGroup>().spacing = 60;
        }
        else
        {
            //GetComponent<HorizontalLayoutGroup>().spacing = 150;

        }
        ImageWithRoundedCorners[] scenarios = transform.GetComponentsInChildren<ImageWithRoundedCorners>();
        foreach(ImageWithRoundedCorners x in scenarios)
        {

            x.Refresh();
        }
    }

}
