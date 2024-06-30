using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCylinder : MonoBehaviour
{
   public Animator anim;
    public Collider other;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();

        
    }

    public void OnTriggerEnter(Collider other)
    {
        anim.SetInteger("Goals", 4);
    }
}
