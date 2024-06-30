using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Nobi.UiRoundedCorners;
public class LoadTextureFromURL : MonoBehaviour
{

    public string TextureURL = "";
    public Image image;
    public Renderer renderer;
    public SpriteRenderer sprite;
    // Start is called before the first frame update
    public void setSprite(Sprite sprite)
    {
        image.sprite = sprite;

    }
    public void load(string url)
    {

        TextureURL = url;
        StartCoroutine(DownloadImage(url));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DownloadImage(string MediaUrl)
    {

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture2D;
           
            Sprite webSprite = SpriteFromTexture2D(webTexture);
            if (image != null)
            {
                webTexture.SetPixels(webTexture.GetPixels(0, 0, webTexture.width, webTexture.height));
                webTexture.Apply();
                webSprite = SpriteFromTexture2D(webTexture);
                image.sprite = webSprite;
            }

            if (sprite != null)
            {
                sprite.sprite = webSprite;
            } if (renderer != null) {
                renderer.material.SetTexture("_BaseMap", webTexture);
                renderer.material.SetTexture("_EmissionMap", webTexture);

            }
            
            /*if(gameObject.GetComponent<ImageWithRoundedCorners>()!=null)
            gameObject.GetComponent<ImageWithRoundedCorners>().Refresh();
            */
        }
    }

    Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }


}