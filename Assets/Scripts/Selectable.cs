using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{
    public bool selected;
    public Color origcolor;
    void Start()
    {
        origcolor = gameObject.GetComponent<Image>().color;
        selected = false;
    }

    public void OnMouseUp()
    {
        if (!selected)
        {
            selected = true;
            gameObject.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            selected = false;
            gameObject.GetComponent<Image>().color = origcolor;
        }
    }
}
