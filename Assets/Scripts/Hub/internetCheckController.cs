using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class internetCheckController : MonoBehaviour
{
    private bool connected=true;
    Animator player;
    public GameObject[] interactableObject;
    private void Awake()
    {
        check();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
 
    }

    public void checkByUser()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        StartCoroutine(checkInternetConnection((isConnected) => {

            connected = isConnected;
            // handle connection status here
            if (isConnected)
            {
                if (!player.GetCurrentAnimatorStateInfo(0).IsName("EMPTY"))
                {
                    enableObjects();

                    player.SetTrigger("play");
                    Time.timeScale = 1;
                    if (RecognizeSpeech.instance != null)
                        RecognizeSpeech.instance.configure();
                }
            }
            else if (!isConnected)
            {
                if (!player.GetCurrentAnimatorStateInfo(0).IsName("internetChecker"))
                {

                    disableObjects();

                    player.SetTrigger("internetChecker");
                }
                Time.timeScale = 0;
            }

        }));

    }
    public void check()
    {
        StartCoroutine(checkInternetConnection((isConnected) => {
            //we have to change connection alert to avoid coflict with ui rate ui
            connected = isConnected;
            // handle connection status here
            if (!isConnected)
            {
                if (!player.GetCurrentAnimatorStateInfo(0).IsName("internetChecker"))
                {

                    disableObjects();

                    player.SetTrigger("internetChecker");
                }
                Time.timeScale = 0;
            }

        }));

      
    }
    void disableObjects()
    {
        for (int i = 0; i < interactableObject.Length; i++)
        {
            if (interactableObject[i] != null)
                interactableObject[i].SetActive(false);
        }
    }
    void enableObjects()
    {
        for(int i=0;i< interactableObject.Length; i++)
        {
            if(interactableObject[i]!=null)
            interactableObject[i].SetActive(true);
        }
    }
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }
    public float waitTime = 2f;
    float timer;
    void Update()
    {
        
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            if (connected) { 
            check();
            timer = 0f;
            }
        }
    
}
  
}
