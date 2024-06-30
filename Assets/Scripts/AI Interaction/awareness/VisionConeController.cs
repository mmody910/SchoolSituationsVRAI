/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class VisionConeController : MonoBehaviour
{
    public ObservableCollection<GameObject> visibleObjects = new ObservableCollection<GameObject>();
    public KnowledgeScore clarity;
    private static Dictionary<GameObject, int> entranceTimes = new Dictionary<GameObject, int>();
    public int baseNoticeProbability = 50;
    public Transform eyeLine;
    public SpeechManager speechManager;
    private void FixedUpdate()
    {
        if (speechManager == null)
            return;
        List<GameObject> unseenobjects = new List<GameObject>();
        foreach (var kv in entranceTimes)
        {
            if ((int)Time.unscaledTime - kv.Value > 5)
                unseenobjects.Add(kv.Key);
        }
        foreach (GameObject gb in unseenobjects)
        {
            entranceTimes.Remove(gb);
            var found = speechManager.speakers.Find((x) => (x.charGO) == gb);
            var found2 = speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject);
            if (found != null && found2 != null)
                KnowledgeFuzzifier.UpdateAwarenessStatus(found.serverName, found2.serverName, "Vision CanNotSee");
        }

        if (eyeLine == null)
            eyeLine = transform.parent.parent;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (speechManager == null||other.gameObject == eyeLine.gameObject)
            return;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, out hit, float.MaxValue, LayerMask.GetMask("VisionTarget")) && hit.collider.gameObject == other.gameObject)
        {
            if (!entranceTimes.ContainsKey(other.gameObject))
                entranceTimes.Add(other.gameObject, (int)Time.unscaledTime);
            if (Random.Range(0, 101) > baseNoticeProbability + (int)clarity * 20)
            {
                visibleObjects.Add((other.gameObject));
                KnowledgeFuzzifier.UpdateAwarenessStatus(speechManager.speakers.Find((x) => (x.charGO) == other.gameObject).serverName,
                speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject).serverName, "Vision ImmedeatlyRecognizes");
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (speechManager == null || other.gameObject == eyeLine.gameObject)
            return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, out hit, float.MaxValue, LayerMask.GetMask("VisionTarget")) && hit.collider.gameObject == other.gameObject)
        {
            if (visibleObjects.Contains(other.gameObject))
            {
                entranceTimes[other.gameObject] = (int)Time.unscaledTime;
                return;
            }
            if (clarity != KnowledgeScore.Clear)
            {
                if (!entranceTimes.ContainsKey(other.gameObject))
                {
                    entranceTimes.Add(other.gameObject ,(int)Time.unscaledTime);
                    KnowledgeFuzzifier.UpdateAwarenessStatus(speechManager.speakers.Find((x) => (x.charGO) == other.gameObject).serverName,
                speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject).serverName, "Vision Notices");
                }
                if (Random.Range(0, 101) + ((int)Time.unscaledTime - entranceTimes[other.gameObject]) * 5 > baseNoticeProbability + (int)clarity * 20)
                {
                    visibleObjects.Add((other.gameObject));
                    KnowledgeFuzzifier.UpdateAwarenessStatus(speechManager.speakers.Find((x) => (x.charGO) == other.gameObject).serverName,
                speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject).serverName, "Vision Notices");
                }
            }
        }
        if (Physics.Raycast(eyeLine.position, eyeLine.forward, out hit, float.MaxValue, LayerMask.GetMask("VisionTarget")))
        {
            if (hit.collider.gameObject == other.gameObject)
            {
                KnowledgeFuzzifier.UpdateAwarenessStatus(speechManager.speakers.Find((x) => (x.charGO) == other.gameObject).serverName,
                speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject).serverName, "Vision IsDirectlyLookingAt");
                if(!visibleObjects.Contains(other.gameObject))
                    visibleObjects.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (speechManager == null || other.gameObject == eyeLine.gameObject)
            return;
        if (visibleObjects.Contains((other.gameObject)))
        { 
            visibleObjects.Remove((other.gameObject));
            KnowledgeFuzzifier.UpdateAwarenessStatus(speechManager.speakers.Find((x) => (x.charGO) == other.gameObject).serverName,
                speechManager.speakers.Find((x) => (x.charGO) == eyeLine.gameObject).serverName, "Vision CanNotSee");
        }
    }
}
public enum KnowledgeScore
{
    Clear, Maybe, Vague
}
*/