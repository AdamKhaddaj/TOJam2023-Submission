using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{

    public Sprite finalisland;

    public void On()
    {
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }
    public void SetFinalIsland()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = finalisland;
    }
}
