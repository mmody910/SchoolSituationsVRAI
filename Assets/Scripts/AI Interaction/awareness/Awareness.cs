/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;

public class Awareness : MonoBehaviour
{
    [HideInInspector]
    public List<VisionArea> visibleObjects = new List<VisionArea>();
    [HideInInspector]
    public ObservableCollection<GameObject> knownEntities;
    private void Start()
    {
        knownEntities = new ObservableCollection<GameObject>();
        VisionConeController buffer;
        foreach (Transform child in transform.GetChild(0))
        {
            if ((buffer = child.gameObject.GetComponent<VisionConeController>())!=null)
            {
                visibleObjects.Add((buffer.visibleObjects,buffer.clarity));
                buffer.visibleObjects.CollectionChanged += registerEntity;
            }
        }
    }

    private void Update()
    {
      //  Debug.Log(KnowledgeFuzzifier.GenerateKnowledgeLogEntry());
    }
    void registerEntity(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null && e.NewItems.Count != 0)
        {
            if (visibleObjects.Find((x) => { return x.visibleObjects == sender; }).clarity==KnowledgeScore.Clear)
            {
                foreach(var gb in e.NewItems.Cast<GameObject>())
                    if(!knownEntities.Contains(gb))
                        knownEntities.Add(gb);
            }
            else if (visibleObjects.Find((x) => { return x.visibleObjects == sender; }).clarity == KnowledgeScore.Maybe&&(Random.Range(0.0f,1.0f)>=0.5f))
            {
                foreach (var gb in e.NewItems.Cast<GameObject>())
                    if (!knownEntities.Contains(gb))
                        knownEntities.Add(gb);
            }
        }
    }
}



[System.Serializable]
public struct VisionArea
{
    [SerializeField]
    public IEnumerable<GameObject> visibleObjects;
    public KnowledgeScore clarity;

    public VisionArea(IEnumerable<GameObject> item1, KnowledgeScore item2)
    {
        visibleObjects = item1;
        clarity = item2;
    }

    public override bool Equals(object obj)
    {
        return obj is VisionArea other &&
               EqualityComparer<IEnumerable<GameObject>>.Default.Equals(visibleObjects, other.visibleObjects) &&
               clarity == other.clarity;
    }

    public override int GetHashCode()
    {
        int hashCode = -1030903623;
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<GameObject>>.Default.GetHashCode(visibleObjects);
        hashCode = hashCode * -1521134295 + clarity.GetHashCode();
        return hashCode;
    }

    public void Deconstruct(out IEnumerable<GameObject> item1, out KnowledgeScore item2)
    {
        item1 = visibleObjects;
        item2 = clarity;
    }

    public static implicit operator (IEnumerable<GameObject>, KnowledgeScore)(VisionArea value)
    {
        return (value.visibleObjects, value.clarity);
    }

    public static implicit operator VisionArea((IEnumerable<GameObject>, KnowledgeScore) value)
    {
        return new VisionArea(value.Item1, value.Item2);
    }
}
*/