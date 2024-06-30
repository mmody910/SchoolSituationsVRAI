using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockRealTime : MonoBehaviour
{
    public GameObject HoursHand;
    public GameObject MinutesHand;
    public GameObject SecondsHand;

    void Update()
    {
        DateTime currentTime = DateTime.Now;

        float hoursDegree = (currentTime.Hour / 12f) * 360f;
        HoursHand.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, hoursDegree));

        float minutesDegree = (currentTime.Minute / 60f) * 360f;
        MinutesHand.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, minutesDegree));

        float secondsDegree = (currentTime.Second / 60f) * 360f;
        SecondsHand.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, secondsDegree));

        //Debug.Log(currentTime);
    }
}
