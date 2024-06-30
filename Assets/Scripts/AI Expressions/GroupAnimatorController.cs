using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAnimatorController : MonoBehaviour
{
    public string triggerName;
    public Transform group;
    public List<Animator> audianceAnimators;

    void Start()
    {
        TraverseParentLeafs(group);
    }

    void TraverseParentLeafs(Transform parent)
    {
       Animator[] childs= parent.GetComponentsInChildren<Animator>();
        foreach (Animator child in childs)
        {
            if (child.gameObject.name.Contains("Armature"))
        {
            audianceAnimators.Add(child);
            
        }
      }
    }

    public void Clap()
    {
        foreach (Animator anim in audianceAnimators)
        {
            StartCoroutine(ClapCoroutine(anim));
        }
    }

    IEnumerator ClapCoroutine(Animator anim)
    {
        float currentValue = 0;
        float changeValue = 0.1f;

        anim.SetTrigger(triggerName);

        while (currentValue <= 1)
        {
            currentValue += changeValue;
            anim.SetLayerWeight(1, currentValue);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(4f);

        currentValue = 1;
        while (currentValue >= 0)
        {
            currentValue -= changeValue;
            anim.SetLayerWeight(1, currentValue);
            yield return new WaitForSeconds(0.1f);
        }
        anim.SetLayerWeight(1, 0f);
    }
}
