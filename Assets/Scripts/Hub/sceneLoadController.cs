using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoadController : MonoBehaviour
{
    public void openFlowers()
    {
        GameObject.Find("newOnboardingStuff").GetComponent<dynamicEventController>().startCall();

    }
    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
