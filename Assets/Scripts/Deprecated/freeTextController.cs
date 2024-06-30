using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class freeTextController : MonoBehaviour
{
    public string defaultValue="";
    private void OnEnable()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = defaultValue;
    }
}
