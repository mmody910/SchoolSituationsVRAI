using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class eyeController : MonoBehaviour
{
    expressionsManager expressionsManager;

    void Start()
    {
        expressionsManager = gameObject.GetComponent<expressionsManager>();
        StartCoroutine(eyeRandomLooks());
    }

    IEnumerator eyeRandomLooks()
    {
        string []eyesExpressions = { "EyesLookRight", "EyesLookLeft"};
        
        while(true)
        {
            StartCoroutine(expressionsManager.playExpression(eyesExpressions[Random.Range(0, 1)], sync: true));
            yield return new WaitForSeconds(3f);
        }
    }

}