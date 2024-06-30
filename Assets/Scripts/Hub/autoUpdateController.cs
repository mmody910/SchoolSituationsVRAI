using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.Android;
public class autoUpdateController : MonoBehaviour
{
    public List<bundleData> updateBundles = new List<bundleData>();
    public TextMeshProUGUI progress;
    public Image progressImage;
    public GameObject main;
    public serverComunicatorController server;
    private int original;
    key loadedData;

    private void Awake()
    {
        loadedData = GameObject.Find("StoredkeysController").GetComponent<StoredkeysController>().loadedData;


    }
    UnityWebRequestAsyncOperation operation;
    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        original = Screen.sleepTimeout;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        main = GameObject.FindGameObjectWithTag("MainMenu");
        
        if (loadedData.keys["notFirstTime"] == "yes" && SceneManager.GetActiveScene().name.Contains("Loading"))
        {
            GameObject.FindGameObjectWithTag("AutoUpdate").transform.GetChild(0).gameObject.SetActive(false);
        }
        StartCoroutine(DownloadFile());
    }

    IEnumerator DownloadFile()
    {
        if(progressImage != null)
            progressImage.GetComponent<Image>().fillAmount = 0f;
        int i = 0;
        foreach (bundleData x in updateBundles)
        {

            UnityWebRequest downloader = UnityWebRequest.Get(x.url);

            string path = Path.Combine(Application.persistentDataPath + "/bundles", x.newName);
            string oldPath = Path.Combine(Application.persistentDataPath + "/bundles", x.oldName);



            //write every downlaoded byte;
            //uwr.downloadHandler = new DownloadHandlerFile(path);

            operation = downloader.SendWebRequest();
            while (!operation.isDone)
            {
                GameObject update= null;
                float temp=(((float)i / (float)updateBundles.Count) * 100.0f);
                if (update==null&&SceneManager.GetActiveScene().name.Contains("Loading"))
                {
                    update = GameObject.FindGameObjectWithTag("AutoUpdate");
                    update.transform.GetChild(0).gameObject.SetActive(true);
                    progressImage = update.transform.Find("AUTOUPDATE/Canvas/LoadingOki/DottedCircle/FillingCircle").GetComponent<Image>();
                    progress = progressImage.transform.Find("Percentage").GetComponent<TextMeshProUGUI>();
                }
                if (update != null)
                {
                    progressImage.fillAmount =(temp/100.0f)+ downloader.downloadProgress * (float)(1.0f / (float)updateBundles.Count);
                    progress.text =  ((int)(downloader.downloadProgress * (float)(1.0f / (float)updateBundles.Count) * 100.0f)+(int)temp ).ToString()+"%";
                }
                
                yield return null;
            }




            if (downloader.result != UnityWebRequest.Result.Success)
            {

                Debug.Log(downloader.error);
            }
            else
            {
                Debug.Log("File successfully downloaded and saved to " + path);

                File.Delete(oldPath);

                File.WriteAllBytes(path, downloader.downloadHandler.data);
              }

            i++;

        }

        GameObject.FindGameObjectWithTag("AutoUpdate").SetActive(false);
        main.transform.GetChild(0).gameObject.SetActive(true);
        server.enabled = false;
        server.enabled = true;
        Screen.sleepTimeout = original;
    }

}

[System.Serializable]
public class bundleData
{
    public string oldName;
    public string newName;
    public string url;
    public bundleData(string o,string n,string u)
    {
        oldName = o;
        newName = n;
        url = u;
    }
}