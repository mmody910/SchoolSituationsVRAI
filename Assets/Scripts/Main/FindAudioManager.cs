using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script is useful for Objects that get instantiated to the scene in runtime, it connects them with the AudioManager
public class FindAudioManager : MonoBehaviour
{
    [HideInInspector] public AudioManager audioManager;
    void Start()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    public void Play(string name)
    {
        audioManager.Play(name);
    }
}
