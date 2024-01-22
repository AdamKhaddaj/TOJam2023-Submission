using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    public GameManager gm;

    void Update()
    {

        transform.Find("FoodText").GetComponent<TMPro.TextMeshProUGUI>().text = gm.food.ToString();

        if (gm.food > 50)
        {
            SetFalse();
            transform.Find("FullFood").gameObject.SetActive(true);
        }
        else if (gm.food > 0)
        {
            SetFalse();
            transform.Find("HalfFood").gameObject.SetActive(true);
        }
        else
        {
            SetFalse();
            transform.Find("EmptyFood").gameObject.SetActive(true);
        }
    }

    private void SetFalse()
    {
        transform.Find("FullFood").gameObject.SetActive(false);
        transform.Find("HalfFood").gameObject.SetActive(false);
        transform.Find("EmptyFood").gameObject.SetActive(false);

    }
}
