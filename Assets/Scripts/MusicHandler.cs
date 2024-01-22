using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicHandler : MonoBehaviour
{

    public float fadetime;
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
               gameObject.GetComponent<AudioSource>().volume -= Time.deltaTime/fadetime;
        }
    }
}

