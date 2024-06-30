using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class GrantPermissionController : MonoBehaviour
{
    string message = "";
    private bool micPermissionGranted = false;

    void Awake()
    {
        givePermission();
    }

   void givePermission()
    {

#if PLATFORM_ANDROID
        // Request to use the microphone, cf.
        // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
        message = "";
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#elif PLATFORM_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
#else
            micPermissionGranted = true;
            message = "";
#endif
        foreach (var device in Microphone.devices)
        {
            Debug.Log("DeviceName: " + device);
        }
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "";
        }
        else
        {

            givePermission();
        }
#elif PLATFORM_IOS
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "";
        }
#endif

    }
}
