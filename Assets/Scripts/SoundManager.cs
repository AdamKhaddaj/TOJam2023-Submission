using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public List<AudioClip> sounds = new List<AudioClip>();

    public AudioSource source;

    public float fadetime;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySoundEffect(int s)
    {
        source.PlayOneShot(sounds[s]);
    }

}
