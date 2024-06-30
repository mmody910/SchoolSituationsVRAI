using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;

[Serializable]
class patientRecord
{
    public long timestamp;
    public int practiceTime;
    public string scenarioId;
    public Dictionary<string,int> rates;
}

public class saveDataForSessionController : MonoBehaviour
{
    [HideInInspector]
    public string scenarioId = "";
    public bool noFixedTime = false;
    public float sessionTime = 300f;
    firebaseController fbcontroller;
    public RecognizeSpeech recognizeSpeech;
    public GameObject[] interactionManager;
    Animator anim;
    public string sceneName;
    public CharacterController character;
   
    private void OnEnable()
    {

        scenarioId = PlayerPrefs.GetString("YatekScenarioId");
        st = calculateTimeInMilliSecond();
       
         anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        if (!noFixedTime) { 
        Invoke("finishSession", sessionTime);
        }
    }
    public void finishSession()
    {

        GameObject.FindGameObjectWithTag("Player").GetComponent<PauseMenuController>().enabled = false;

        ft = calculateTimeInMilliSecond();
        anim.SetTrigger("endSession");
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = false;

        Invoke("hideSceneObjects", 0.7f);
        recognizeSpeech.enabled = false;


    }
   
    public void changeObjectsStatus()
    {

        if (character != null&& character.enabled)
        {
            Invoke("changeCharacterController", 0f);
            Invoke("hideSceneObjects", 0.25f);

        }
        else if (character != null && !character.enabled)
        {

            Invoke("hideSceneObjects", 0.75f);
            Invoke("changeCharacterController", 0.9f);
        }else { 
            if (interactionManager[0].activeSelf)
            {

                Invoke("hideSceneObjects", 0.25f);
            }
            else
            {

                Invoke("hideSceneObjects", 0.75f);
            }
        }


    }
    void hideSceneObjects()
    {
        for (int i = 0; i < interactionManager.Length; i++)
            interactionManager[i].SetActive(!interactionManager[i].activeSelf);

    }
    void changeCharacterController()
    {
        if (character != null)
        {
            character.enabled = !character.enabled;

        }

    }

    private void Update()
    {
        
    }
    public void loadScene(string SceneName)
    {

        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene(SceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;

     
    }

    public long st;
    public long ft; 
    public long calculateTimeInMilliSecond()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
    
  
}
