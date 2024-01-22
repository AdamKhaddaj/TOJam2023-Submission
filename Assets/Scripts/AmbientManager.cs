using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{

    public static AmbientManager instance;

    public List<AudioClip> sounds = new List<AudioClip>();

    public AudioSource source;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySoundEffect(int s)
    {
        source.PlayOneShot(sounds[s]);
    }

}
