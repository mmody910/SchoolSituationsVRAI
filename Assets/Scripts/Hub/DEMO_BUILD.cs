using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Android;
using System.IO;

public class DEMO_BUILD : MonoBehaviour
{
    public bool isDemo = false;
    public string demoEmail, demoPassword;
    public firebaseController firebaseController;
    public UnityEvent eventDemo;
  
    private void Awake()
    {

       







        /* if (!PlayerPrefs.HasKey("notFirstTime"))
         {
             loadScene("MINDFULNESS");
         }*/



    }
    public void loadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
    }
    void Start()
    {

       
        if (isDemo)
        {
            DemoFun();
        }
       
    }

    public void DemoFun()
    {
        
        Debug.LogWarning("DEMO ENABLED");
        firebaseController.loginFun(demoEmail, demoPassword);

        if (eventDemo != null)
        {
            eventDemo.Invoke();
        }

        isDemo = true;
    }
   
}
