using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disable_character : MonoBehaviour
{
    public AudioSource characterSynthesizeSpeech;

    void OnTriggerExit(Collider other)
    {
        characterSynthesizeSpeech.volume = 0f;
    }
}
