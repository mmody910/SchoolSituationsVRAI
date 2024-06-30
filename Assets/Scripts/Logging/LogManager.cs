/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class LogManager : MonoBehaviour
{
    public bool DebugMode = false;
    public bool ClearOnPlay = false;
    public LogFile[] logFiles;

    void Awake()
    {
        // Clear Logs.txt On Play
        if(ClearOnPlay)
        {
            foreach (LogFile log in logFiles)
            {
                ClearLog(log);
            }
        }
    }

    void Update()
    {
        // Testing
        //FindObjectOfType<LogManager>().CustomLog("test2", "Update", "test");
    }

    public void CustomLog(string fileName, string header, string message)
    {
        // If DebugMode not enabled don't contine logging to save performance
        if (!DebugMode) return;

        bool fileNotFound = true;

        foreach(LogFile lf in logFiles)
        {
            if(lf.FileName == fileName)
            {
                fileNotFound = false;
                lf.Add(header, message);
                WriteLog(lf);
                break;
            }
        }

        if(fileNotFound)
        {
            Debug.Log($"LogManager: File '{fileName}' Not Found!");
            return;
        }
    }

    public void WriteLog(LogFile log)
    {
        string filePath = UnityEditor.AssetDatabase.GetAssetPath(log.SaveFolder) + "/" + log.FileName + ".txt";
        StreamWriter sw = new StreamWriter(filePath, true);

        sw.WriteLine("-------------------------------------------");
        sw.WriteLine("[" + System.DateTime.Now + "]");

        // Write Headers
        foreach (KeyValuePair<string, List<string>> kvp in log.Info)
        {
            sw.WriteLine(kvp.Key+":");

            // Write Messages under each Header
            foreach (string s in kvp.Value)
            {
                sw.WriteLine("\t" + s);
            }

            sw.WriteLine("");
        }

        sw.Close();
    }

    public void ClearLog(LogFile log)
    {
        string filePath = UnityEditor.AssetDatabase.GetAssetPath(log.SaveFolder) + "/" + log.FileName + ".txt";
        StreamWriter sw = new StreamWriter(filePath, false);

        sw.WriteLine($"[{log.FileName}]\n");

        sw.Close();
    }
}

[System.Serializable]
public class LogFile
{
    public string FileName;
    public UnityEngine.Object SaveFolder;
    public Dictionary<string, List<string>> Info = new Dictionary<string, List<string>>(); //Headers and Messages

    public void Add(string header, string message)
    {
        if (Info != null && Info.ContainsKey(header))
        {
            if(!Info[header].Contains(message))
            {
                Info[header].Add(message);
            }
        }
        else
        {
            Info.Add(header, new List<string>{message});
        }
    }

    public void ClearValues()
    {
        foreach (KeyValuePair<string, List<string>> kvp in Info)
        {
            Info[kvp.Key].Clear();
        }
    }
}
*/