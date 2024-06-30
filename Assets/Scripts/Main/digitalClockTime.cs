using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class digitalClockTime : MonoBehaviour
{
    TextMeshProUGUI clockText;

    void Start()
    {
        clockText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        DateTime time = DateTime.Now;
        string hour = leadingZero(time.Hour);
        string minute = leadingZero(time.Minute);
        //string second = leadingZero(time.Second);
        clockText.text = hour + ':' + minute;
    }

    string leadingZero(int t)
    {
        return t.ToString().PadLeft(2, '0');
    }
}
