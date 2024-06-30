/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System;
using System.Text.RegularExpressions;

public static class KnowledgeFuzzifier 
{
    public static List<string> knowledgeLog = new List<string>();
    public static Dictionary<string, Dictionary<string,AwarenessStatus>> awarenessLogs =
        new Dictionary<string, Dictionary<string, AwarenessStatus>>();    
    
    
    public static void UpdateAwarenessStatus(string observed, string observer, string TriggerReason)
    {
        if (TriggerReason.Split()[0].ToLower().Equals("vision"))
        {

            VisionStatus vs;
            if (Enum.TryParse(TriggerReason.Split()[1], out vs))
            {
                if (awarenessLogs.ContainsKey(observer))
                {
                    if (awarenessLogs[observer].ContainsKey(observed))
                    {
                        awarenessLogs[observer][observed] = new AwarenessStatus(vs, awarenessLogs[observer][observed].HearingStatus, awarenessLogs[observer][observed].KnowledgeStatus);
                    }
                    else
                    {
                        awarenessLogs[observer].Add(observed, new AwarenessStatus(vs, HearingStatus.Hears, KnowledgeStatus.KnowsOf));
                    }
                }
                else
                {
                    awarenessLogs.Add(observer, new Dictionary<string, AwarenessStatus>());
                    awarenessLogs[observer].Add(observed, new AwarenessStatus(vs, HearingStatus.Hears, KnowledgeStatus.KnowsOf));
                }
            }



        }
        else if (TriggerReason.Split()[0].ToLower().Equals("hearing"))
        {
            HearingStatus vs;
            if (Enum.TryParse(TriggerReason.Split()[1], out vs))
                if (awarenessLogs.ContainsKey(observer))
                {
                    if (awarenessLogs[observer].ContainsKey(observed))
                    {
                        awarenessLogs[observer][observed] = new AwarenessStatus(awarenessLogs[observer][observed].VisionStatus, vs, awarenessLogs[observer][observed].KnowledgeStatus);
                    }
                    else
                    {
                        awarenessLogs[observer].Add(observed, new AwarenessStatus(awarenessLogs[observer][observed].VisionStatus, vs, awarenessLogs[observer][observed].KnowledgeStatus));
                    }
                }
                else
                {
                    awarenessLogs.Add(observer, new Dictionary<string, AwarenessStatus>());
                    awarenessLogs[observer].Add(observed, new AwarenessStatus(awarenessLogs[observer][observed].VisionStatus, vs, awarenessLogs[observer][observed].KnowledgeStatus));
                }
        }
        else if (TriggerReason.Split()[0].ToLower().Equals("knowledge"))
        {
            KnowledgeStatus vs;
            if(Enum.TryParse(TriggerReason.Split()[1], out vs))
                awarenessLogs[observer][observed] = new AwarenessStatus(awarenessLogs[observer][observed].VisionStatus, awarenessLogs[observer][observed].HearingStatus, vs);
        }
    }
    public static string GenerateKnowledgeLogEntry(SpeechManager speech)
    {
        foreach (var Observer in awarenessLogs)
        {
            foreach (var Observed in Observer.Value)
            {
                if(speech.speakers.Find((x) => { return x.serverName == Observed.Key; })!=null&& speech.speakers.Find((x) => { return x.serverName == Observer.Key; }) != null) { 
                    knowledgeLog.Add(Observer.Key +" "+ separateCamelCase(Observed.Value.ToString()) +" "+ Observed.Key);
                }
            }
        }
        string m = "";
        foreach (var Message in knowledgeLog)
        {
            m += "->"+Message + "\n";
        }
        knowledgeLog.Clear();
        return m;
    }
    public static string separateCamelCase(string v)
    {
        return Regex.Replace(Regex.Replace(v, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2").ToLower();

    }

}


public enum VisionStatus
{
    CanNotSee, ImmedeatlyRecognizes, Notices, IsDirectlyLookingAt
}
public enum HearingStatus
{
    Hears, IsNotHearing
}

public enum KnowledgeStatus
{
    DoesntKnowOf,KnowsOf
}

public struct AwarenessStatus
{
    public VisionStatus VisionStatus;
    public HearingStatus HearingStatus;
    public KnowledgeStatus KnowledgeStatus;

    public AwarenessStatus(VisionStatus visionStatus, HearingStatus hearingStatus, KnowledgeStatus knowledgeStatus)
    {
        VisionStatus = visionStatus;
        HearingStatus = hearingStatus;
        KnowledgeStatus = knowledgeStatus;
    }

    public override bool Equals(object obj)
    {
        return obj is AwarenessStatus other &&
               VisionStatus == other.VisionStatus &&
               HearingStatus == other.HearingStatus &&
               KnowledgeStatus == other.KnowledgeStatus;
    }

    public override int GetHashCode()
    {
        int hashCode = 486466477;
        hashCode = hashCode * -1521134295 + VisionStatus.GetHashCode();
        hashCode = hashCode * -1521134295 + HearingStatus.GetHashCode();
        hashCode = hashCode * -1521134295 + KnowledgeStatus.GetHashCode();
        return hashCode;
    }

    public void Deconstruct(out VisionStatus visionStatus, out HearingStatus hearingStatus, out KnowledgeStatus knowledgeStatus)
    {
        visionStatus = VisionStatus;
        hearingStatus = HearingStatus;
        knowledgeStatus = KnowledgeStatus;
    }

    public static implicit operator (VisionStatus VisionStatus, HearingStatus HearingStatus, KnowledgeStatus KnowledgeStatus)(AwarenessStatus value)
    {
        return (value.VisionStatus, value.HearingStatus, value.KnowledgeStatus);
    }

    public static implicit operator AwarenessStatus((VisionStatus VisionStatus, HearingStatus HearingStatus, KnowledgeStatus KnowledgeStatus) value)
    {
        return new AwarenessStatus(value.VisionStatus, value.HearingStatus, value.KnowledgeStatus);
    }
    public override string ToString()
    {
        return VisionStatus.ToString(); //+" and " +HearingStatus.ToString() + " and " + KnowledgeStatus.ToString()+" ";
    }
}
*/