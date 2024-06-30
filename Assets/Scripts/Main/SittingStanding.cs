using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SittingStanding : MonoBehaviour
{
    public Transform sittingPos;
    Vector3 standingPos;
    GameObject player;
    ContinuousMoveProviderBase movementController;
    bool isSitting = false;

    void Start()
    {
        player = GameObject.Find("XR Origin");
        movementController = GameObject.Find("Locomotion System").GetComponent<ContinuousMoveProviderBase>();
    }

    public void changeSittingState()
    {
        if (isSitting)
        {
            stand();
        }
        else
        {
            sit();
        }
    }

    void sit()
    {
        isSitting = true;
        standingPos = player.transform.position;
        player.transform.position = sittingPos.position;

        movementController.moveSpeed = 0;
        movementController.useGravity = false;
    }

    void stand()
    {
        isSitting = false;
        player.transform.position = standingPos;

        movementController.moveSpeed = 2;
        movementController.useGravity = true;

    }
}
