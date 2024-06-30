using UnityEngine;
using System.Collections;

public class CLICK : MonoBehaviour
{
    bool isHighlighted = false;
    GameObject baseObject;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
    }

    void OnMouseDown()
    {
        if (isHighlighted == false)
        {
            animator.SetTrigger("OPEN");
        }
    }
}