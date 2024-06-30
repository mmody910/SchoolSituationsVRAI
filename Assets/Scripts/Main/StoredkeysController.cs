using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class StoredkeysController : MonoBehaviour
{
    public key loadedData;
    string jsonFilePath;
    public void finishLocomotion()
    {
        PlayerPrefs.SetInt("FinishedLocomotion", 1);
    }
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_ANDROID &&!UNITY_EDITOR 
        string id;
        //   Permission.RequestUserPermission("android.permission.READ_PRIVILEGED_PHONE_STATE");

        // print(Permission.HasUserAuthorizedPermission("android.permission.READ_PRIVILEGED_PHONE_STATE"));

        AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
        print("Simli_android_id : " + id);




#endif
        PlayerPrefs.SetInt("FinishedLocomotion", 0);
        loadedData = new key();
        DontDestroyOnLoad(this.gameObject);
        string keysData;
        jsonFilePath = DataPath();
        CheckFileExistance(jsonFilePath, true);
        keysData = File.ReadAllText(jsonFilePath);
        

        loadedData = Newtonsoft.Json.JsonConvert.DeserializeObject<key>(keysData);
        if (loadedData.keys["notFirstTime"] !="yes")
        {
            loadScene("MINDFULNESS");

        }

    }
    public void setKey(string key, string value)
    {
        loadedData.keys[key] = value;
        
            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(loadedData);

            File.CreateText(jsonFilePath).Dispose();
            using (TextWriter writer = new StreamWriter(jsonFilePath, false))
            {
                writer.WriteLine(dataString);
                writer.Close();
            }

            // File.WriteAllText(filePath, dataString);


        
    }
    public void loadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
    }
    static string storedDataFileName = "data.json";
    static string DataPath()
    {
            return Path.Combine(Application.persistentDataPath, storedDataFileName);
       
    }
    static void CheckFileExistance(string filePath, bool isReading = false)
    {
        if (!File.Exists(filePath))
        {
           // File.Create(filePath);
            key newData = new key();
            newData.keys["notFirstTime"] = "No";
            Debug.Log(newData.keys.Count);
            string dataString= Newtonsoft.Json.JsonConvert.SerializeObject(newData);

            File.CreateText(filePath).Dispose();
            using (TextWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(dataString);
                writer.Close();
            }

           // File.WriteAllText(filePath, dataString);


        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class key
{
    public Dictionary<string, string> keys;
    public key()
    {
        keys = new Dictionary<string, string>();


    }
   
}