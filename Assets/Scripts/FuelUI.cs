using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    public GameManager gm;

    void Update()
    {

        transform.Find("FuelText").GetComponent<TMPro.TextMeshProUGUI>().text = gm.fuel.ToString();

        if(gm.fuel > 75)
        {
            SetFalse();
            transform.Find("FullFuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 50)
        {
            SetFalse();
            transform.Find("34Fuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 25)
        {
            SetFalse();
            transform.Find("HalfFuel").gameObject.SetActive(true);
        }
        else if (gm.fuel > 0)
        {
            SetFalse();
            transform.Find("14Fuel").gameObject.SetActive(true);
        }
        else
        {
            SetFalse();
            transform.Find("EmptyFuel").gameObject.SetActive(true);
        }
    }

    private void SetFalse()
    {
        transform.Find("FullFuel").gameObject.SetActive(false);
        transform.Find("34Fuel").gameObject.SetActive(false);
        transform.Find("HalfFuel").gameObject.SetActive(false);
        transform.Find("14Fuel").gameObject.SetActive(false);
        transform.Find("EmptyFuel").gameObject.SetActive(false);

    }
}
