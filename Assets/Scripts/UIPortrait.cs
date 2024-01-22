using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //y = 809 when up, y = 279 when down
    public bool slidingdown, slidingup;
    public float slideSpeed;

    public GameManager gm;
    public Companion displaycompanion;
    private int displaycompanionindex;
    private Color origtextcolor;

    private void Start()
    {
        slidingdown = false;
        slidingup = false;
        displaycompanion = gm.companions[0];
        origtextcolor = transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slidingdown = true;
        slidingup = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        slidingup = true;
        slidingdown = false;
    }

    private void Update()
    {
        if (slidingdown)
        {
            if(gameObject.GetComponent<RectTransform>().anchoredPosition.y >= 279)
            {
                float newPosition = gameObject.GetComponent<RectTransform>().anchoredPosition.y - slideSpeed * Time.deltaTime;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x, newPosition);
            }
            else
            {
                slidingdown = false;
            }
        }
        if (slidingup)
        {
            if (gameObject.GetComponent<RectTransform>().anchoredPosition.y <=  809)
            {
                float newPosition = gameObject.GetComponent<RectTransform>().anchoredPosition.y + slideSpeed * Time.deltaTime;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x, newPosition);
            }
            else
            {
                slidingup = false;
            }
        }

        //update values based on displaycompanion
        if (displaycompanion != null)
        {
            transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = displaycompanion.companionName;
            if (displaycompanion.cursed)
            {
                transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().color = origtextcolor;
            }
            transform.Find("LoyaltyBar").GetComponent<Bar>().SetValue(displaycompanion.loyalty);
            transform.Find("EnergyBar").GetComponent<Bar>().SetValue(displaycompanion.energy);
            transform.Find("SkillBar").GetComponent<Bar>().SetValue(displaycompanion.skill);
        }
        else
        {
            SetAlone();
        }

    }

    public void CompanionReset() //called when a companion dies to avoid index out of bounds errors
    {
        if(gm.companions.Count > 0)
        {
            displaycompanion = gm.companions[0];
            displaycompanionindex = 0;
        }
        else
        {
            displaycompanion = null;
            SetAlone();
        }
    }

    public void ChangeDisplayCompanionRight()
    {
        if(displaycompanionindex != gm.companions.Count-1)
        {
            displaycompanion = gm.companions[displaycompanionindex + 1];
            displaycompanionindex = displaycompanionindex + 1;
        }
        else
        {
            displaycompanion = gm.companions[0];
            displaycompanionindex = 0;
        }
    }
    public void ChangeDisplayCompanionLeft()
    {
        if (displaycompanionindex != 0)
        {
            displaycompanion = gm.companions[displaycompanionindex-1];
            displaycompanionindex = displaycompanionindex-1;
        }
        else
        {
            displaycompanion = gm.companions[gm.companions.Count-1];
            displaycompanionindex = gm.companions.Count-1;
        }
    }

    private void SetAlone()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
            if(child.gameObject.name == "AloneText")
            {
                child.gameObject.SetActive(true);
            }
        }
    }

}
