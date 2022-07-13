using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvalidNotice : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RectTransform textPosition;
    public Vector3 distence = new Vector3(20f, -20f, 0f);
    public float noticeTime = 2f;
    private float timer = 0f;
    private Color textColor;

    void Start()
    {
        textColor = text.color;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > noticeTime)
        {
            Destroy(gameObject);
        }
        textPosition.position += distence * Time.deltaTime / noticeTime;
        textColor.a -= Time.deltaTime / noticeTime;
        text.color = textColor;
    }
}
