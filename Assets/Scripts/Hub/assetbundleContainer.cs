using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assetbundleContainer : MonoBehaviour
{
    public static assetbundleContainer instance;
    [HideInInspector]
    public AssetBundle asset;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
    }
}
