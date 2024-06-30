using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UIElements;

[Serializable]
class openAiExpressionData
{
    public string prompt;
    public float temperature;
    public int max_tokens;
    public float top_p;
    public float frequency_penalty;
    public float presence_penalty;
    //public string[] stop;
    public string user;
    public openAiExpressionData()
    {
        temperature = 0.5f;
        max_tokens = 50;
        top_p = 1;
        frequency_penalty = 0;
        presence_penalty = 0f;
      /*  stop = new String[2];
        stop[0] = "Listener";
        stop[1] = "Speaker";
 */
        }

}
[Serializable]
class chatOpenAIData
{

    public string model;
    public Dictionary<string,string>[] messages;
    public float temperature;
    public int max_tokens;
    public float top_p;
    public string[] stop;
    public float presence_penalty;
    public float frequency_penalty;
    public string user;
    public chatOpenAIData()
    {
        temperature = 0.7f;
        max_tokens = 75;
        top_p = 1;
        frequency_penalty = 0;
        presence_penalty = 0.6f;
        stop = new string[4];
        stop[0] = "";
        stop[1] = "";
        stop[2] = "";
        stop[3] = "";
        model = "gpt-3.5-turbo";
        messages = new Dictionary<string, string>[2];
        messages[0] = new Dictionary<string, string>
        {
            { "role", "system" },
            { "content", "" }
        };
        messages[1] = new Dictionary<string, string>
        {
            {"role","user"},
            {"content",""}
        };
    }

}
[Serializable]
class openAiData
{
    public string prompt;
    public float temperature;
    public int max_tokens;
    public float top_p;
    public float frequency_penalty;
    public float presence_penalty;
    public string[] stop;
    public string model;
    public int logprobs;
    public string user;
    public openAiData()
    {
        temperature = 0.7f;
        max_tokens = 75;
        top_p = 1;
        frequency_penalty = 0;
        presence_penalty = 0.6f;
        stop = new string[2];
        stop[0] = "Candidate";
        stop[1] = "Interviewer";

    }

}
[Serializable]
public class choice
{
    public string text;
    public int index;

}

[Serializable]
class chatOpenAIResponse
{
    public string id;
    public int created;
    public chatChoice[] choices;
    public string finish_reason;
//    public Dictionary<string,int> usage;

}

[Serializable]
public class chatChoice
{
    public int index;
    public Dictionary<string, string> message;
}
[Serializable]
class openAiResponse
{
    public string id;
    public string created;
    public string model;
    public choice[] choices;


}


[Serializable]
class openAiExpressionResponse
{
    public string id;
    public string created;
    public string model;
    public choice[] choices;


}