using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MusicText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private AudioManager audioManager;
    public Slider slider;
    private float defaultSetting;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        audioManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioManager>();
        defaultSetting = slider.value;
    }

    public void UpdateText()
    {
        text.text = slider.value.ToString();
        audioManager.BgmVolumeSetting(slider.value / 100);
    }
    public void ResetValue()
    {
        slider.value = defaultSetting;
        text.text = slider.value.ToString();
        audioManager.BgmVolumeSetting(slider.value / 100);
    }
}
