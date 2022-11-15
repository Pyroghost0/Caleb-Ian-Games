/* Coded by Caleb Kahn
 * Qualms
 * Slider text in pause menu for auto aim strength option
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AutoAimStrengthText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private PlayerMovement player;
    public Slider slider;
    private float defaultSetting;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        defaultSetting = slider.value;
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString();
        player.autoAimPower = slider.value * 100f;
    }
    public void ResetValue()
	{
        slider.value = defaultSetting;
        text.text = slider.value.ToString();
        player.autoAimPower = slider.value * 100f;
    }
}
