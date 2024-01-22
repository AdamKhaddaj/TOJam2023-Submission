using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour {
	public int value;
	public Slider slider;
	public Gradient gradient;
	public Image fill;

    private void Start()
    {
		value = 100;
		SetMaxValue(value);
		SetValue(value);

		if(gameObject.name == "SuccessBar")
		{
			value = GameManager.instance.odds;
			SetValue(value);
		}

    }
    public void SetMaxValue(int value) {
		slider.maxValue = value;
		fill.color = gradient.Evaluate(1f);
	}

	public void SetValue(int value) {
		slider.value = value;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

	public void ChangeValue(int value)
    {
		slider.value += value;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}



