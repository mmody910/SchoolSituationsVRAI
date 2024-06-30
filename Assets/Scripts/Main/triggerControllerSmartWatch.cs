using UnityEngine;
using UnityEngine.Events;

public class triggerControllerSmartWatch : MonoBehaviour
{
    public UnityEvent enterTriggerfunction;
    public UnityEvent exitTriggerfunction;
    public UnityEvent buttonPressfunction;
    smartWatchController smartWatchController;

    void Start()
    {
        smartWatchController = GameObject.Find("XR Origin").GetComponent<smartWatchController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "XR Origin")
        {
            smartWatchController.UpdateEvents(enterTriggerfunction, exitTriggerfunction, buttonPressfunction);
            smartWatchController.EnterTrigger();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "XR Origin")
        {
            smartWatchController.ExitTrigger();
            smartWatchController.UpdateEvents(null, null, null);
        }
    }
}
