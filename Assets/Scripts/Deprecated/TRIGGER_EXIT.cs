using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGER_EXIT : MonoBehaviour
{
    private saveDataForSessionController sessionController;
    void Start()
    {
        sessionController = GameObject.FindGameObjectWithTag("sessionController").GetComponent<saveDataForSessionController>();
    }

    void OnTriggerEnter(Collider other)
    {
        sessionController.finishSession();
    }
}
