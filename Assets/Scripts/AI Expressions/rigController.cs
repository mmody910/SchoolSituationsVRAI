using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class rigController : MonoBehaviour
{
    List<Rig> rigs;
    private void Start()
    {
        Transform speakersObject = transform.Find("characters");
        rigs = new List<UnityEngine.Animations.Rigging.Rig>(speakersObject.gameObject.GetComponentsInChildren<UnityEngine.Animations.Rigging.Rig>());
    }
    public void setRig(float value)
    {
        StartCoroutine(setting(value));
    }
    IEnumerator setting(float value)
    {
        yield return new WaitForSeconds(1.0f);

        RandomLookController[] RandomLookControllers = GetComponentsInChildren<RandomLookController>();
        bool flag;
        if (value == 1)
        {
            flag = true;
        }
        else
        {

            flag = false;
        }
        foreach (RandomLookController x in RandomLookControllers)
        {
            x.enabled = flag;
        }
    }
}
