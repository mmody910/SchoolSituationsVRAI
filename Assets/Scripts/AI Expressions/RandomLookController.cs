using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
[RequireComponent(typeof(Animator))]
public class RandomLookController : MonoBehaviour
{
    Transform characters;
    Transform player;
    List<Transform> points=new List<Transform>();
    Animator anim;
    List<MultiAimConstraint>head_neck=new List<MultiAimConstraint>();
    Rig rig;
    RigBuilder rigBuilder;

    Coroutine b;
    public float minTimePlayer = 2.0f, maxTimePlayer = 6.0f;
    public float minTimeRand = 5.0f, maxTimeRand = 10.0f;
    WeightedTransformArray sources1;
    WeightedTransformArray sources2;
    WeightedTransformArray sources3;

    IEnumerator rebuild()
    {
        transform.parent.gameObject.GetComponent<RigBuilder>().enabled = false;
        yield return new WaitForSeconds(.2f);
        transform.parent.gameObject.GetComponent<RigBuilder>().enabled = true;



    }
    private void Awake()
    {
        rigBuilder = GetComponentInParent<RigBuilder>();
        rig = GetComponent<Rig>();
        anim = GetComponent<Animator>();
        characters = transform.parent.parent;
        player = GameObject.Find("XR Origin").transform;
        if (head_neck.Count == 0)
        {
            foreach (Transform x in transform)
            {
                head_neck.Add(x.GetComponent<MultiAimConstraint>());
            }
        }
        if (points.Count == 0)
        {
            if (characters.childCount > 1)
            {
                foreach (Transform x in characters)
                {
                    points.Add(x);
                }
            }
            points.Add(player);
        }
        if (head_neck[0].data.sourceObjects[0].transform == null)
        {
            WeightedTransformArray a = new WeightedTransformArray();
            WeightedTransform t = new WeightedTransform();
            t.transform = player;
            t.weight = 1;
            a.Add(t);
            foreach (Transform character in characters)
            {
                t.transform = character;
                t.weight = 0;
                a.Add(t);
            }

            foreach (MultiAimConstraint x in head_neck)
            {
                x.data.sourceObjects = a;//.transform = target;

            }
            StartCoroutine(rebuild());
            sources1 = head_neck[0].data.sourceObjects;
            sources2 = head_neck[1].data.sourceObjects;
            sources3 = head_neck[2].data.sourceObjects;

        }

    }
    // Start is called before the first frame update
    void OnEnable()
    {
        b=StartCoroutine(randomLookGroup());
        
       

    }
    IEnumerator resetRig()
    {
        if (rig.weight != 0)
        {
            float elapsed = 0.0f;
            while (elapsed < 0.5)
            {
                rig.weight = Mathf.Lerp(rig.weight, 0, elapsed / 0.5f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rig.weight = 0;
        }
    }

        private void OnDisable()
    {
        StopCoroutine(b);
        StartCoroutine(resetRig());
        
    
    }
    IEnumerator randomLookGroup()
    {
        if (rig.weight != 1) { 
       float  elapsed = 0.0f;
        while (elapsed < 0.5)
        {
            rig.weight = Mathf.Lerp(rig.weight, 1, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rig.weight = 1;
        }

        while (true)
        {
             StartCoroutine(assignTarget2(points.Count - 1));

            //assignTarget(points[Random.Range(0,points.Count-1)]);
           // yield return new WaitForSeconds(1.5f);
            //StartCoroutine(rigFocus());
            yield return new WaitForSeconds(Random.Range(minTimePlayer, maxTimePlayer));


            StartCoroutine(assignTarget2(Random.Range(0, points.Count - 1)));

            //assignTarget(points[Random.Range(0,points.Count-1)]);
           // yield return new WaitForSeconds(1.5f);
            //StartCoroutine(rigFocus());
            yield return new WaitForSeconds(Random.Range(minTimeRand, maxTimeRand));

            //StartCoroutine(rigIgnore());
            //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));



          //rigBuilder.Build();



        }
    }
   
    
    


    IEnumerator assignTarget2(int rand)
    {
        Transform target = points[rand];
        if (target.name == transform.parent.name||target==player)
        {
            target = player;
            rand = -1;
        }
      
        float elapsed = 0.0f;

        int i = 0;

        if (sources1.GetWeight(rand + 1) != 1)
        {


            elapsed = 0.0f;
            int j = 0;
            elapsed = 0.0f;
            while (elapsed < 0.2f)
            {
                print("rand : "+j);
                j++;
                sources1.SetWeight(rand+1, Mathf.Lerp(0, 1, elapsed / 0.2f));
                sources2.SetWeight(rand+1, Mathf.Lerp(0, 1, elapsed / 0.2f));
                sources3.SetWeight(rand+1, Mathf.Lerp(0, 1, elapsed / 0.2f));

                head_neck[0].data.sourceObjects = sources1;
                head_neck[1].data.sourceObjects = sources2;
                head_neck[2].data.sourceObjects = sources3;
                elapsed += Time.deltaTime;
                yield return null;
            }
           
        

        sources1.SetWeight(rand + 1, 1);
            sources2.SetWeight(rand + 1, 1);
            sources3.SetWeight(rand + 1, 1);
         
            head_neck[0].data.sourceObjects = sources1;
            head_neck[1].data.sourceObjects = sources2;
            head_neck[2].data.sourceObjects = sources3;
        
            }

        i = 0;
        foreach (var y in head_neck[0].data.sourceObjects)
            {
                if (y.transform != target && y.weight != 0)
                {
                int j = 0;
                    elapsed = 0.0f;
                    while (elapsed < 0.5)
                    {
                    print(j);
                    j++;
                        sources1.SetWeight(i, Mathf.Lerp(1, 0, elapsed / 0.5f));
                        sources2.SetWeight(i, Mathf.Lerp(1, 0, elapsed / 0.5f));
                        sources3.SetWeight(i, Mathf.Lerp(1, 0, elapsed / 0.5f));

                        head_neck[0].data.sourceObjects = sources1;
                        head_neck[1].data.sourceObjects = sources2;
                        head_neck[2].data.sourceObjects = sources3;
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    sources1.SetWeight(i, 0);
                    sources2.SetWeight(i, 0);
                    sources3.SetWeight(i, 0);
                    head_neck[0].data.sourceObjects = sources1;
                    head_neck[1].data.sourceObjects = sources2;
                    head_neck[2].data.sourceObjects = sources3;

                }
            i++;
        }
        
        
    }

   

}


