using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.Networking;
// using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Linq;
using System.IO;
using System;
using System.Net;
using System.Threading;

public class SpeechManager : MonoBehaviour
{
    public List<Character> speakers = new List<Character>();
    AIManager AIManagerInstance;
    GameObject player;
    
    public float time_since_last_speech = 0;
    private void Start()
    {
        time_since_last_speech = 0;
    }
    void setSpeakers()
    {
        player = GameObject.Find("XR Origin");

        AIManagerInstance = GetComponent<AIManager>();
        Transform speakersObject = transform.Find("characters");

        foreach (Transform speaker in speakersObject)
        {
            speakers.Add(new Character(speaker.gameObject, ""));

        }
        speakers.Add(new Character(player, ""));
        //   AssignCharacters();
    }

    public void AssignCharacters()
    {
        setSpeakers();
        int i = 0;
        foreach (Character c in speakers)
        {
            if (PlayerPrefs.GetString("YatekLang") == "en-US")
            {
                c.serverName = AIManagerInstance.talkerEng[i];
            }
            else
            {
                c.serverName = AIManagerInstance.talkerNor[i];

            }
            i++;
        }
    }
    
    public async Task PlayNextSpeaker()
    {
        print("Speaker Count: "+ AIManagerInstance.speakerQueue.Count.ToString());
        bool terminate = false;
        GroupInteractionManager gim = GetComponent<GroupInteractionManager>();
        // get all speakers audio sources to check if any of them are speaking

        while (AIManagerInstance.speakerQueue.Count > 0 &&! terminate)
        {
            // if (gim.message.Trim() != "" && gim.message != gim.)
            // {
            //     Debug.Log("Cancelled");
            //     break;
            // }

            string next = AIManagerInstance.speakerQueue.Dequeue();
            if (AIManagerInstance.speakerQueue.Count == 0)
            {
                gim.time_since_player_spoke = 0;
                StartCoroutine(gim.SpeechSH());
            }
            print(next);
            if (speakers.Find((x) => { return x.serverName == next; }) == null) {


                print("null       ++++"   );
                continue;
            
            }
            // if (gim.message.Trim() != "" && gim.message != gim.SynthMessage)
            // {
            //     break;
            // }

            // check if the audio source has been assigned and therefore
            // is currently speaking
            if (gim.audioSource!=null)
            {
                print(gim.audioSource);
                // if so cancel the task
                //        print("Cancel");
                break;
            }
    

            else
            {
                print("elseeee");

                SynthesizeSpeech nextSpeaker = speakers.Find((x) => { return x.serverName == next; }).charGO.GetComponent<SynthesizeSpeech>();
                string statement = nextSpeaker.statementQueue.Peek();
                gim.audioSource = nextSpeaker.GetComponent<CharacterInteractionManager>().audioSource;
                //Debug.Log("Start Synth"+ Time.time.ToString());
                time_since_last_speech = 0;
                AIManagerInstance.SentenceSpoken(next + ":" + "\"" + statement + "\"\n");
                await nextSpeaker.SynthesizeAudioAsyncFromQueue(true, "");
                time_since_last_speech = 0;
                //Debug.Log("Finish Synth" + Time.time.ToString());
                gim.audioSource = null;
            }
            
        }
    }
}
[System.Serializable]
public class Character
{
    public GameObject charGO;
    public string serverName;
    public bool find(string name)
    {
        if (serverName == name)
        {
            return true;
        }
        return false;
    }
    public Character(GameObject item1, string item2)
    {
        charGO = item1;
        serverName = item2;
    }

    public override bool Equals(object obj)
    {
        return obj is Character other &&
               EqualityComparer<GameObject>.Default.Equals(charGO, other.charGO) &&
               serverName == other.serverName;
    }

    public override int GetHashCode()
    {
        int hashCode = -1030903623;
        hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(charGO);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(serverName);
        return hashCode;
    }

    public void Deconstruct(out GameObject item1, out string item2)
    {
        item1 = charGO;
        item2 = serverName;
    }

    public static implicit operator (GameObject, string)(Character value)
    {
        return (value.charGO, value.serverName);
    }

    public static implicit operator Character((GameObject, string) value)
    {
        return new Character(value.Item1, value.Item2);
    }
}