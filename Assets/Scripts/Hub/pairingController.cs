using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using TMPro;
public class pairingController : MonoBehaviour
{
    public firebaseController fbController;
    public GameObject notifyUser;
    public TextMeshProUGUI code;
    public GameObject loader;
    public GameObject pairButon;
    public GameObject LoginCanvas;
    public TextMeshProUGUI UserName;
    string id = "";
    string deviceUniqueIdentifier = "";
    string AuthToken = "";
    bool Production = false;
    private void OnEnable()
    {
        notifyUser.SetActive(false);
        LoginCanvas.SetActive(false);
        loader.SetActive(true);
        pair();
    }
    private void Awake()
    {
        deviceUniqueIdentifier = (SystemInfo.deviceUniqueIdentifier);
    }
    public void generateOTP()
    {
        loader.SetActive(true);
        notifyUser.SetActive(false);
        generateOTPFun(deviceUniqueIdentifier);
    }
    public async Task generateOTPFun(string id)
    {

        string apiUrl;
        if (Production)
        {
            apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/vrAuthGenerateOTP";
        }
        else
        {
            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/vrAuthGenerateOTP";
        }

        /*
         pairingData newRecord = new pairingData();
         newRecord.serialNo = id;

         string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

         print(json);

        */
        WWWForm form = new WWWForm();
        form.AddField("serialNo", id);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                //show alert with you have to pair from the mobile app first
                Debug.Log(www.error);

                Debug.Log(www.downloadHandler.text);
                loader.SetActive(false);
            }
            else
            {
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);
                pairingReponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<pairingReponse>(www.downloadHandler.text);

                code.text = data.otp;
                PlayerPrefs.SetString("OTPSimli", data.otp);

                loader.SetActive(false);
                pairButon.SetActive(true);
                notifyUser.SetActive(true);
            }
        }
    }

    public void LoginWithAuthToken()
    {
        loader.SetActive(true);
        notifyUser.SetActive(false);
        LoginCanvas.SetActive(false);
        StartCoroutine(fbController.loginFunWithCustomeToken(AuthToken));
    }
    /*
    public async Task pairWithSerialFun(string id)
    {
        string apiUrl;
        if (Production)
        {
            apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/getAuthToken";
        }
        else
        {

            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/getAuthToken";
        }

        // We should pass in the serial number all UPPER
        id = id.ToUpper();

        pairingData newRecord = new pairingData();
        newRecord.serialNo = id;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

        WWWForm form = new WWWForm();
        form.AddField("serialNo", id);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                //Serial Number Error
                Debug.Log(www.error);

                Debug.Log(www.downloadHandler.text);

                notifyUser.SetActive(true);
                LoginCanvas.SetActive(false);
                LoginLoadingCircle.SetActive(false);

                Debug.LogError(id);
            }
            else
            {
                //  print(www.downloadHandler.text);
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);
                pairinWithSerialReponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<pairinWithSerialReponse>(www.downloadHandler.text);
                print(data.userName);
                print(data.authToken);
                print(apiUrl);
                LoginCanvas.SetActive(true);
                LoginLoadingCircle.SetActive(false);
                UserName.text = data.userName;
                AuthToken = data.authToken;
                //LoginWithAuthToken(data.authToken);
            }
        }
    }
    */
    public void pair()
    {
        try
        {
#if UNITY_ANDROID
            //   Permission.RequestUserPermission("android.permission.READ_PRIVILEGED_PHONE_STATE");

            // print(Permission.HasUserAuthorizedPermission("android.permission.READ_PRIVILEGED_PHONE_STATE"));

            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
            print("Simli_android_id : " + id);




#endif
        }
        catch { }

        //id = "0F7057608EAF3CAE";
        pairFun(id);
    }
    public void pairWithToken()
    {
        loader.SetActive(true);
        notifyUser.SetActive(false);

        pairWithTokenFun(deviceUniqueIdentifier);
    }
    public async Task pairWithTokenFun(string id)
    {
        print(id);
        //   print(id);
        string apiUrl;
        if (Production)
        {
            apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/vrAuth";
        }
        else
        {

            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/vrAuth";

        }

        pairingData newRecord = new pairingData();
        newRecord.serialNo = id;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

        WWWForm form = new WWWForm();
        form.AddField("serialNo", id);
        print(id);
        print(apiUrl);
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                //show alert with you have to pair from the mobile app first
                Debug.Log(www.error);

                Debug.Log(www.downloadHandler.text);
                notifyUser.SetActive(true);

                loader.SetActive(false);
                pairButon.SetActive(true);
            }
            else
            {
                print(www.downloadHandler.text);
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);
                pairingReponse2 data = Newtonsoft.Json.JsonConvert.DeserializeObject<pairingReponse2>(www.downloadHandler.text);
                print(data.customToken);
                print(apiUrl);

                StartCoroutine(fbController.loginFunWithCustomeToken(data.customToken));


            }
        }
    }

    public async Task pairFun(string id)
    {
        id = id.ToUpper();
        //   print(id);
        string apiUrl;
        if (Production)
        {
            apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/getAuthToken";
        }
        else
        {

            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/getAuthToken";

        }

        pairingData newRecord = new pairingData();
        newRecord.serialNo = id;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

        WWWForm form = new WWWForm();
        form.AddField("serialNo", id);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                //Android ID not on Simli Admin Panel

                LoginCanvas.SetActive(false);

                loader.SetActive(true);
                notifyUser.SetActive(true);
                pairButon.SetActive(false);
                generateOTP();
            }
            else
            {
                print(www.downloadHandler.text);
                //data = JsonUtility.FromJson<scenariosData>(www.downloadHandler.text);
                pairinWithSerialReponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<pairinWithSerialReponse>(www.downloadHandler.text);
                print(data.authToken);
                print(apiUrl);
                UserName.text = data.userName;
                AuthToken = data.authToken;

                loader.SetActive(false);
                notifyUser.SetActive(false);
                pairButon.SetActive(false);

                LoginCanvas.SetActive(true);


                //StartCoroutine(fbController.loginFunWithCustomeToken(data.authToken));
            }
        }
    }

    public void unpair()

    {
        //PlayerPrefs.SetString("notFirstTime", "NO");
        PlayerPrefs.SetInt("loggedIn", 2);
        // PlayerPrefs.SetString("notFirstTime", "no");

        GameObject.FindGameObjectWithTag("firebase").GetComponent<firebaseController>().logoutFun();


        pairButon.SetActive(true);
        loader.SetActive(false);

        /*
#if UNITY_ANDROID
        //   Permission.RequestUserPermission("android.permission.READ_PRIVILEGED_PHONE_STATE");

        // print(Permission.HasUserAuthorizedPermission("android.permission.READ_PRIVILEGED_PHONE_STATE"));

        AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
        print("android_id : " + id);


       

#endif
        UnpairFun(id);
        */
    }
    public async Task UnpairFun(string id)
    {
        string apiUrl;
        if (Production)
        {
            apiUrl = "https://us-central1-vr-mvp.cloudfunctions.net/vrSignout";
        }
        else
        {
            apiUrl = "https://us-central1-vr-mvp-staging.cloudfunctions.net/vrSignout";
        }
        pairingData newRecord = new pairingData();
        newRecord.serialNo = id;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(newRecord);

        //  print(json);
        WWWForm form = new WWWForm();
        form.AddField("serialNo", id);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            //www.method = "POST";
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                pairButon.SetActive(true);
                loader.SetActive(false);
            }
        }
    }

}
[System.Serializable]
public class pairingData
{
    public string serialNo;
}

[System.Serializable]
public class pairingReponse
{
    public string customToken;
    public string otp;
}

[System.Serializable]
public class pairingReponse2
{
    public string customToken;
    public bool status;
}

[System.Serializable]
public class pairinWithSerialReponse
{
    public string authToken;
    public string userName;
}
