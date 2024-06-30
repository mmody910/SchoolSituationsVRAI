using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllersInteractionScript : MonoBehaviour
{
   public Animator anim;
   private Collider other;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();

        
    }

    private void OnTriggerEnter(Collider other)
    {
        anim.SetLayerWeight(1, 1.0f);
        
    }
}
