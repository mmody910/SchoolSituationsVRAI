using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faderController : MonoBehaviour
{
    GameObject m_Fader;
    public float time=1f;
    void Awake()
    {

        //Find the fader object
        m_Fader = GameObject.Find("Fader");
        //Check if we found something
        if (m_Fader == null)
            Debug.LogWarning("No Fader object found on camera.");


    }
    public IEnumerator FadeCamera(Transform obj, Transform next)
    {

        //Ensure we have a fader object
        if (m_Fader != null)
        {
            //Fade the Quad object in and wait 0.75 seconds

            //StartCoroutine(FadeIn(0.75f, m_Fader.GetComponent<Renderer>().material));
            StartCoroutine(DoAThingOverTime(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), time, m_Fader.GetComponent<Renderer>().material));
            yield return new WaitForSeconds(1f);

            //Change the camera position
            obj.position = next.position;

            //Fade the Quad object out 
            StartCoroutine(DoAThingOverTime(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), time, m_Fader.GetComponent<Renderer>().material));

            //            StartCoroutine(FadeOut(0.75f, m_Fader.GetComponent<Renderer>().material));
            yield return new WaitForSeconds(1f);
        }
        else
        {
            //No fader, so just swap the camera position
            obj.position = next.position;
        }


    }
    IEnumerator DoAThingOverTime(Color start, Color end, float duration, Material mat)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            mat.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }
        mat.color = end; //without this, the value will end at something like 0.9992367
    }

}
