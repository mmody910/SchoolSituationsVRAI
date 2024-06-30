using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILookAt : MonoBehaviour
{   
    [Header("AI Behavior")]
    public GameObject target;


    // Update is called once per frame
    void Update()
    {   
        Vector3 position = new Vector3(target.transform.position.x,transform.position.y,target.transform.position.z);
        transform.LookAt(position);
    }
}
