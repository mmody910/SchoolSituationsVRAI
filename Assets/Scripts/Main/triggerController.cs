using UnityEngine;
using UnityEngine.Events;
using  System.Collections;

public class triggerController : MonoBehaviour
{
    public string otherName= "XR Origin";
    private void Start()
    {
        
    }
    public UnityEvent enterTriggerfunction;
    public UnityEvent exitTriggerfunction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name== otherName) {

            StartCoroutine(Entering());
    }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == otherName)
        {

            StartCoroutine(Exiting());
        }
    }



    IEnumerator Entering()
    {
        yield return new WaitForSeconds(0.1f);

        if (enterTriggerfunction != null)
            enterTriggerfunction.Invoke();
    }

    IEnumerator Exiting()
    {
        yield return new WaitForSeconds(1f);

        if (exitTriggerfunction != null)
            exitTriggerfunction.Invoke();
    }


}
