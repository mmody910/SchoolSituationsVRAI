using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

using UnityEngine.XR.Interaction.Toolkit;
public class PauseMenuController : MonoBehaviour
{
    private Animator sceneTransitionAnimator;
    private InputDevice rightDevice, leftDevice;
    public SnapTurnProviderBase turningController;
    public ContinuousMoveProviderBase movementController;
    smartWatchController smartWatchController;
    private void OnEnable()
    {
        turningController.turnAmount = 36;
        movementController.moveSpeed = 2;
    }
    void Start()
    {
        sceneTransitionAnimator = gameObject.GetComponent<Animator>();
        smartWatchController = GetComponentInChildren<smartWatchController>();
        //smartWatchController = GameObject.Find("XR Origin").GetComponent<smartWatchController>();
        print(smartWatchController.gameObject.name);
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (var item in devices)
        {
            if (item.name.Contains("Right"))
            {
                rightDevice = item;
                continue;
            }
            else if (item.name.Contains("Left"))
            {
                leftDevice = item;
                continue;
            }
        }
    }
    bool menuOpened = false;

    void Update()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPress);

        if (!menuOpened && (menuButtonPress || Input.GetKeyDown(KeyCode.P)))
        {

            Pause();
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            Resume();
        }else if (Input.GetKeyDown(KeyCode.K))
        {
            Restart();
        }
    }

    public void Pause()
    {
        menuOpened = true;
      
        sceneTransitionAnimator.SetTrigger("pause");
        smartWatchController.TriggerMenu();
        turningController.turnAmount = 0;
        movementController.moveSpeed = 0;
        GameObject.FindGameObjectWithTag("sessionController").GetComponent<saveDataForSessionController>().changeObjectsStatus();
    }

    public void Resume()
    {
        menuOpened = false;
      
        sceneTransitionAnimator.SetTrigger("resume");
        smartWatchController.TriggerMenu();
        turningController.turnAmount = 36;
        movementController.moveSpeed = 2;
        GameObject.FindGameObjectWithTag("sessionController").GetComponent<saveDataForSessionController>().changeObjectsStatus();
    }

    public void Restart()
    {
        if (assetbundleContainer.instance.asset != null) { 
        SceneManager.LoadScene("SplashScreen_Generic", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
