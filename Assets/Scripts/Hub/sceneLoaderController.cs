using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sceneLoaderController : MonoBehaviour
{
    firebaseController fbcontroller;
    public Text loading;
    public Image loader;
    private void Start()
    {
        fbcontroller = GameObject.FindGameObjectWithTag("firebase").GetComponent<firebaseController>();
       
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
        /* //Begin to load the Scene you specify
         AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
         Resources.UnloadUnusedAssets();
         SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
         //Don't let the Scene activate until you allow it to
         asyncOperation.allowSceneActivation = false;
         //When the load is still in progress, output the Text and progress bar
         while (!asyncOperation.isDone)
         {
             //Output the current progress
             loading.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
             loader.fillAmount=  asyncOperation.progress;

             // Check if the load has finished
             if (asyncOperation.progress >= 0.9f)
             {
                 //Change the Text to show the Scene is ready
                 loading.text = "Scene Loaded";
                 loader.fillAmount = 1;
                 //Wait to you press the space key to activate the Scene
                 // if (Input.GetKeyDown(KeyCode.Space))
                 //Activate the Scene

                    asyncOperation.allowSceneActivation = true;
         }    }
        */
        yield return null;
        
    }
}
