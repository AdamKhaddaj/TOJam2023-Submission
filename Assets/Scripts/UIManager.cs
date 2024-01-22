using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gm;
    public Canvas UI;

    public bool pannelslidein, pannelslideout, fortuneslidein, fortuneslideout;

    public float slideSpeed, fortuneSlideSpeed;

    private void Awake()
    {
        pannelslidein = false;
        pannelslideout = false;
        fortuneslidein = false;
        fortuneslideout = false;
    }

    void Update()
    {
        //FOOD UI
        UI.transform.Find("Food").Find("FoodText").GetComponent<TMPro.TextMeshProUGUI>().text = gm.food.ToString();

        if (gm.food > 50)
        {
            SetFoodFalse();
            UI.transform.Find("Food").Find("FullFood").gameObject.SetActive(true);
        }
        else if (gm.food > 0)
        {
            SetFoodFalse();
            UI.transform.Find("Food").Find("HalfFood").gameObject.SetActive(true);
        }
        else
        {
            SetFoodFalse();
            UI.transform.Find("Food").Find("EmptyFood").gameObject.SetActive(true);
        }

        //FUEL UI
        UI.transform.Find("Fuel").Find("FuelText").GetComponent<TMPro.TextMeshProUGUI>().text = gm.fuel.ToString();

        if (gm.fuel > 75)
        {
            SetFuelFalse();
            UI.transform.Find("Fuel").Find("FullFuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 50)
        {
            SetFuelFalse();
            UI.transform.Find("Fuel").Find("34Fuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 25)
        {
            SetFuelFalse();
            UI.transform.Find("Fuel").Find("HalfFuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 0)
        {
            SetFuelFalse();
            UI.transform.Find("Fuel").Find("14Fuel").gameObject.SetActive(true);
        }
        else
        {
            SetFuelFalse();
            UI.transform.Find("Fuel").Find("EmptyFuel").gameObject.SetActive(true);
        }

        if (gm.fortuneavailable && gm.travelling)
        {
            UI.transform.Find("FateBall").GetComponent<Button>().interactable = true;
        }
        else
        {
            UI.transform.Find("FateBall").GetComponent<Button>().interactable = false;
        }

        //PANNEL SLIDING UI

        if (pannelslidein)
        {
            if(UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition.x <= -535)
            {
                pannelslidein = false;
                UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition = new Vector2(-535, UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition.y);
            }
            else
            {
                float newPosition = UI.transform.Find("EventPannel").GetComponent<RectTransform>().position.x - slideSpeed * Time.deltaTime;
                UI.transform.Find("EventPannel").GetComponent<RectTransform>().position = new Vector2(newPosition, UI.transform.Find("EventPannel").position.y);
            }
        }

        if(pannelslideout)
        {
            if (UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition.x >= 546)
            {
                pannelslideout = false;
            }
            else
            {
                float newPosition = UI.transform.Find("EventPannel").GetComponent<RectTransform>().position.x + slideSpeed * Time.deltaTime;
                UI.transform.Find("EventPannel").GetComponent<RectTransform>().position = new Vector2(newPosition, UI.transform.Find("EventPannel").position.y);
            }
        }

        if (fortuneslidein)
        {
            if (!UI.transform.Find("FortuneTeller").gameObject.active)
            {
                UI.transform.Find("FortuneTeller").gameObject.SetActive(true);
            }

            //do background transparency and fortune teller transparency

            if(UI.transform.Find("FortuneTeller").GetComponent<Image>().color.a < 0.7f)
            {
                Color newcolor = UI.transform.Find("FortuneTeller").GetComponent<Image>().color;
                newcolor.a = UI.transform.Find("FortuneTeller").GetComponent<Image>().color.a + 0.02f;
                UI.transform.Find("FortuneTeller").GetComponent<Image>().color = newcolor;
            }

            foreach(Transform child in UI.transform.Find("FortuneTeller"))
            {
                if (child.GetComponent<Image>().color.a < 1f)
                {
                    Color newcolor = child.GetComponent<Image>().color;
                    newcolor.a = child.GetComponent<Image>().color.a + 0.02f;
                    child.GetComponent<Image>().color = newcolor;
                }
            }
        }
        if (fortuneslideout)
        {
            //do background transparency and fortune teller transparency

            if (UI.transform.Find("FortuneTeller").GetComponent<Image>().color.a > 0.0f)
            {
                Color newcolor = UI.transform.Find("FortuneTeller").GetComponent<Image>().color;
                newcolor.a = UI.transform.Find("FortuneTeller").GetComponent<Image>().color.a - 0.02f;
                UI.transform.Find("FortuneTeller").GetComponent<Image>().color = newcolor;
            }

            foreach (Transform child in UI.transform.Find("FortuneTeller"))
            {
                if (child.GetComponent<Image>().color.a > 0f)
                {
                    Color newcolor = child.GetComponent<Image>().color;
                    newcolor.a = child.GetComponent<Image>().color.a - 0.02f;
                    child.GetComponent<Image>().color = newcolor;
                }
                else
                {
                    fortuneslideout = false;
                    UI.transform.Find("FortuneTeller").gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetFoodFalse()
    {
        UI.transform.Find("Food").Find("FullFood").gameObject.SetActive(false);
        UI.transform.Find("Food").Find("HalfFood").gameObject.SetActive(false);
        UI.transform.Find("Food").Find("EmptyFood").gameObject.SetActive(false);
    }

    private void SetFuelFalse()
    {
        UI.transform.Find("Fuel").Find("FullFuel").gameObject.SetActive(false);
        UI.transform.Find("Fuel").Find("34Fuel").gameObject.SetActive(false);
        UI.transform.Find("Fuel").Find("HalfFuel").gameObject.SetActive(false);
        UI.transform.Find("Fuel").Find("14Fuel").gameObject.SetActive(false);
        UI.transform.Find("Fuel").Find("EmptyFuel").gameObject.SetActive(false);
    }


}
