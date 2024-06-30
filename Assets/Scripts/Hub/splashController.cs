using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
public class splashController : MonoBehaviour
{
    [TextArea(3, 10)]
    string titleText;
    [TextArea(3, 10)]
    public string titleTextEng;
    [TextArea(3, 10)]
    public string titleTextNor;
    public Color textColor;
    public TextMesh title;
    public Color color;
    public Material background;
    public Sprite texture;
    public SpriteRenderer image;
    public GameObject mainObject;
    public Vector3 mainObjectScale;
    public Transform parentOfMainObject;
    public CharacterController character;
    public GameObject scene;
    //public string onboardingTag;
    public string sceneName;

    GameObject player;
    public void loadScene(string SceneName)
    {

        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene(SceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        /*
         //print(PlayerPrefs.GetInt(onboardingTag));
          PlayerPrefs.SetInt(onboardingTag, 1);
         //print(PlayerPrefs.GetInt(onboardingTag));
          */
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        yield return null;

    }
    private void Awake()
    {

        string language = PlayerPrefs.GetString("YatekLang");
        if (language == "en-US")
        {
            titleText = titleTextEng;
        }
        else
        {

            titleText = titleTextNor;
        }
        /*  if(onboardingTag!="")
          if (PlayerPrefs.GetInt(onboardingTag, 0) == 1)
          {
              loadScene(sceneName);
          }
        */
        //to generate OKI and add it to the spalsh screens in the onboarding
        if (mainObject != null && parentOfMainObject != null)
        {

            mainObject = Instantiate(mainObject, new Vector3(0.4f, -2.5f, -1.62f), Quaternion.Euler(new Vector3(0f, 180f, 0)), parentOfMainObject);
            mainObject.transform.localScale = mainObjectScale;
            mainObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0));
            mainObject.transform.localPosition = new Vector3(0f, -2.5f, -1.62f);
        }
        title.text = titleText;
        title.color = textColor;
        background.color = color;
        image.sprite = texture;




        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<PauseMenuController>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Invoke("hideSplash", 9.8f);

    }
    void hideSplash()
    {
        scene.SetActive(true);
        this.gameObject.SetActive(false);
        if (character != null)
            character.enabled = true;
        player.GetComponent<PauseMenuController>().enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
