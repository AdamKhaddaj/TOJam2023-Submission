using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IndicatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int riskIndicator; //0 = low, 1= med, 2= high
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(riskIndicator == 0)
        {
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.curevent.lowIndicator;
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").gameObject.SetActive(true);
        }
        else if(riskIndicator == 1)
        {
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.curevent.medIndicator;
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").gameObject.SetActive(true);
        }
        else
        {
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.curevent.highIndicator;
            GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").GetComponent<TMPro.TextMeshProUGUI>().text = " ";

        GameManager.instance.ui.UI.transform.Find("EventPannel").Find("Indicator").gameObject.SetActive(false);

    }
}
