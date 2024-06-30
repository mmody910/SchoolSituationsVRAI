using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class headAndNeckController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        react();
    }
    void react()
    {
            StartCoroutine(lookAtUser());
        Invoke("noReaction", Random.Range(7f, 12.0f));
    }
   
    void noReaction()
    {
        StartCoroutine(lookAt());
        Invoke("react", Random.Range(2f, 5f));

    }


    IEnumerator lookAt()
    {
        for (float i = 1; i >= 0; i -= 0.1f)
        {
            yield return new WaitForSeconds(0.0001f);
            GetComponent<Rig>().weight = i;
        }
    }
    IEnumerator lookAtUser()
    {
        for (float i = 0; i <= 1; i += 0.1f)
        {
            yield return new WaitForSeconds(0.0001f);
            // print(obj.gameObject.name+"    "+ i);
            GetComponent<Rig>().weight = i;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
