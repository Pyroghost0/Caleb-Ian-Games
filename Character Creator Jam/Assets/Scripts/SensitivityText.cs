using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SensitivityText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private PlayerMovement player;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString();
        player.mouseVirticalSensitivity = slider.value;
        player.mouseHorizontalSensitivity = slider.value * 4f;
    }
}
