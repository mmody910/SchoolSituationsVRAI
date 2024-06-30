using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipStatus : MonoBehaviour
{
   public void flip()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
