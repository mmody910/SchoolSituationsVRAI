
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class firebaseController : MonoBehaviour
{
    Timestamp st;
    Timestamp ft;
    FirebaseAuth auth;
    FirebaseUser user;
    FirebaseFirestore db;

    public GameObject openAIALert;

    public GameObject loader;

    public GameObject login;

    public GameObject keyboard;
    public GameObject server;
    public void setStartTime()
    {
        st = Timestamp.GetCurrentTimestamp();
    }
    public void setFinishTime()
    {
        ft = Timestamp.GetCurrentTimestamp();
    }
    private void Awake()
    {
       
        InitializeFirebase();
        DontDestroyOnLoad(this.gameObject);


    }
    public void InitializeFirebase()
    {



        loader = GameObject.FindGameObjectWithTag("MainMenu").transform.GetChild(0).gameObject;

        //login = GameObject.FindGameObjectWithTag("login");


        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        //  StartCoroutine(login("lars@yatek.io","13456"));
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                login.SetActive(true);
                loader.SetActive(false);
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                print(Application.persistentDataPath);

                PlayerPrefs.SetString("UserId", user.UserId);
                if (PlayerPrefs.GetInt("loggedIn") == 1)
                {
                    login.SetActive(false);
                    loader.SetActive(true);
                    server.SetActive(true);
                 }



                
            }
        }
        else
        {
            if (user == null)
            {
                   print("not signed");
               
                login.SetActive(true);
                loader.SetActive(false);
            }
            /* loader.SetActive(false);
             login.SetActive(true);
          */
        }



    }



    IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            logoutFun();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            loginFun("mypatient@yatek.io", "12345678");
        }

    }

    public IEnumerator createUserFunWithemail(string email, string password)
    {
        string msg = "";
        yield return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

    }

    public IEnumerator loginFunWithemail(string email, string password)
    {
        string msg = "";
        yield return auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;

            Invoke("setLoginTag", 0.1f);
            Debug.LogFormat("Firebase user signed-in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

        });

    }

    public void loginFun(string email, string password)
    {
        StartCoroutine(loginFunWithemail(email, password));
    }


    public void logoutFun()
    {
        auth.SignOut();
        login.SetActive(true);
        loader.SetActive(false);
    }
    public IEnumerator loginFunWithCustomeToken(string token)
    {
        //  print("login");
        // print(token);
        string msg = "";
        yield return auth.SignInWithCustomTokenAsync(token).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCustomTokenAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            Invoke("setLoginTag", 0.1f);
            //openAIALert.SetActive(true);
            //Invoke("hideAlert", 15f);

        });

    }

    void setLoginTag()
    {
        print("1");
        PlayerPrefs.SetInt("loggedIn", 1);
        print("1");
        login.SetActive(false);
        print("1");
        server.SetActive(true);
        print("1");
        loader.SetActive(true);
        print("1");
        /*if (PlayerPrefs.GetString("notFirstTime") != "yes")
        {
            loadScene("MINDFULNESS");
        }
        */

    }
    public void loadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
    }
    void hideAlert()
    {
        loader.SetActive(true);
        openAIALert.SetActive(false);
    }
    public string mainScene;
    public IEnumerator setRecord(int rate)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        //  Timestamp st = Timestamp.GetCurrentTimestamp();
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        string path = "session" + time.ToString();
        CollectionReference c = db.Collection("sessions");
        DocumentReference d = c.Document(path);
        CollectionReference users = db.Collection("users");
        DocumentReference u = users.Document(PlayerPrefs.GetString("UserId"));
        Dictionary<string, object> s = new Dictionary<string, object>
{
        { "start", st},
        { "rate", rate },
        { "finish", ft },
        { "user", u }
};

        yield return d.SetAsync(s).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the LA document in the cities collection.");
            StartCoroutine(LoadScene(mainScene));
        }); ;


    }
}
