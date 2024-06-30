using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class dynamicEventController : MonoBehaviour
{
    public UnityEvent startEvents,endEvents;

    public void startCall()
    {
        if (startEvents != null)
            startEvents.Invoke();
    }
    public void endCall()
    {
        if (startEvents != null)
            startEvents.Invoke();
    }
}
