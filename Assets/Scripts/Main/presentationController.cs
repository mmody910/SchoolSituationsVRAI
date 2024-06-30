using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Firebase.Auth;


public class presentationController : MonoBehaviour
{
    public bool onStage = false; //State of player whether on stage or not
    public Material myMaterial;
    public List<Texture2D> slides;
    private int slidesIndex = 0;
    private InputDevice rightDevice, leftDevice;
    private bool A_lastState = false, B_lastState = false; //Stores button press states from the previous frame
    private bool Production = false; //Switch between production and staging

    void Start()
    {
        
        //Get presentation slides data from server and assign them to array slides
        StartCoroutine(getPresentationSlides()); //Assign presentation slides data to slides array
        
        
        //Player is not on stage yet
        onStage = false;

        //Recognize VR left and right controllers
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (var item in devices)
        {
            if (item.name.Contains("Right"))
            {
                rightDevice = item;
                continue;
            }
            else if (item.name.Contains("Left"))
            {
                leftDevice = item;
                continue;
            }
        }
        
    }
    public void setOnStage(bool value)
    {
        onStage = value;
    }

    // Update is called once per frame
    void Update()
    {
        rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool A_buttonState); //button A
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool B_buttonState); //button B

        //Check if player is on stage to enable slide scrolling 
        //Check slidesIndex boundries
        if (onStage)
        {
            if (((A_lastState == false && A_buttonState == true) || Input.GetKeyDown(KeyCode.E)) && slidesIndex < slides.Count - 1) //Next slide
            {
                slidesIndex++;
                //Change slides material texture
                myMaterial.SetTexture("_BaseMap", slides[slidesIndex]);
                myMaterial.SetTexture("_EmissionMap", slides[slidesIndex]);

            }
            else if (((B_lastState == false && B_buttonState == true) || Input.GetKeyDown(KeyCode.Q)) && 0 < slidesIndex) //Back slide
            {

                slidesIndex--;
                //Change slides material texture
                myMaterial.SetTexture("_BaseMap", slides[slidesIndex]);
                myMaterial.SetTexture("_EmissionMap", slides[slidesIndex]);
            }
        }

        //Assign the buttons to be equal to the current button state before leaving the state
        A_lastState = A_buttonState;
        B_lastState = B_buttonState;
    }



   
    IEnumerator getPresentationSlides()
    {
        List<string> slidesUrls=  Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("YatekFileURL"));
       
       

        //If there are presentation slides on server then clear current list and upadte with new slides
        if (slidesUrls != null)
        {
            slides.Clear();

            //Get Texture from url and Add it to slides list
            for (int i = 0; i < slidesUrls.Count; i++)
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(slidesUrls[i]);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D webTexture = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
                    slides.Add(webTexture);
                }

                if (i == 0)
                {
                    //Initial slide
                    myMaterial.SetTexture("_BaseMap", slides[0]);
                    myMaterial.SetTexture("_EmissionMap", slides[0]);
                }
            }
        }
        else if (slidesUrls == null)
        {
            //Initial slide
            myMaterial.SetTexture("_BaseMap", slides[0]);
            myMaterial.SetTexture("_EmissionMap", slides[0]);

            //TODO Show pop-up on VR to upload presentation on companion app
        }
    }

}
