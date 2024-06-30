using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


public class backController : MonoBehaviour
{
    private InputDevice rightDevice, leftDevice;
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach(var item in devices)
        {
            if (item.name.Contains("Right"))
            {
                rightDevice = item;
                continue;
            }else if (item.name.Contains("Left"))
            {
                leftDevice = item;
                continue;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
     /*   rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueRight);
        leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValueLeft);
        if (secondaryValueLeft || secondaryValueRight)
        {
            StartCoroutine(LoadScene(sceneName));
        }

        */
    }

        IEnumerator LoadScene(string sceneName)
        {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
       
        /*//Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        AsyncOperation asyncOperation2 = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();

        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
        {
            //Output the current progress

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
                {
                //Change the Text to show the Scene is ready
                //Wait to you press the space key to activate the Scene
                // if (Input.GetKeyDown(KeyCode.Space))

                
                //Activate the Scene
                asyncOperation.allowSceneActivation = true;
                }

            yield return null;
        }
        */
        yield return null;

    }
}
