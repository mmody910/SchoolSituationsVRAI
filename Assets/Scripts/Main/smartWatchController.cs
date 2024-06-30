using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class smartWatchController : MonoBehaviour
{
    public Animator smartWatchAnimator;
    public GameObject clockTime;
    [HideInInspector] public UnityEvent currentEnterTriggerEvent;
    [HideInInspector] public UnityEvent currentExitTriggerEvent;
    [HideInInspector] public UnityEvent currentButtonEvent;
    private InputDevice rightDevice, leftDevice;
    private bool LT_lastState = false;
    private bool inTriggerArea = false;
    private bool inMenu = false;
    private XRInteractorLineVisual[] xrInteractorLineVisuals;


    public playerController PlayerController;
    void Start()
    {
        xrInteractorLineVisuals = gameObject.GetComponentsInChildren<XRInteractorLineVisual>();
        setLineVisuals(false);

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
        if (PlayerController != null)
        PlayerController.enabled = true;
    }

    void Update()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool LT_buttonState); //Left Trigger Button

        if ((LT_lastState == false && LT_buttonState == true) || Input.GetKeyDown(KeyCode.T))
        {
            ButtonPress();
        }

        LT_lastState = LT_buttonState;
    }

    public void UpdateEvents(UnityEvent enterTriggerEvent, UnityEvent exitTriggerEvent, UnityEvent buttonPressEvent)
    {
        currentEnterTriggerEvent = enterTriggerEvent;
        currentExitTriggerEvent = exitTriggerEvent;
        currentButtonEvent = buttonPressEvent;
    }

    public void EnterTrigger()
    {
        inTriggerArea = true;
        clockTime.SetActive(false);
        smartWatchAnimator.SetTrigger("GoIn");
        leftDevice.SendHapticImpulse(0u, 0.7f, 1f);

        if (currentEnterTriggerEvent != null)
        {
            currentEnterTriggerEvent.Invoke();
        }
    }
    public void ExitTrigger()
    {
        inTriggerArea = false;
        clockTime.SetActive(true);
        smartWatchAnimator.SetTrigger("GoOut");

        if (currentExitTriggerEvent != null)
        {
            currentExitTriggerEvent.Invoke();
        }
    }
    public void ButtonPress()
    {
        if (inTriggerArea && currentButtonEvent != null)
        {
            leftDevice.SendHapticImpulse(0u, 0.2f, 0.2f);
            currentButtonEvent.Invoke();
        }
    }
    public void TriggerMenu()
    {
        leftDevice.SendHapticImpulse(0u, 0.2f, 0.2f);
        inMenu = !inMenu;
        if (inMenu)
        {
            setLineVisuals(true);
        }
        else
        {
            setLineVisuals(false);
        }

        if (!inTriggerArea)
        {
            clockTime.SetActive(!clockTime.activeSelf); //Set active if not active and vice-versa
        }

        smartWatchAnimator.SetTrigger("LoadOkiMenu");
    }

    void setLineVisuals(bool isActive)
    {
        foreach (XRInteractorLineVisual x in xrInteractorLineVisuals)
        {
            x.enabled = isActive;
        }
    }

}
