using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBase : MonoBehaviour
{
    public short bones = 0;
    public short health = 1000;
    public short maxHealth = 1000;
    public short defence = 10;
    public short maxSkeletons = 3;
    public short maxSkeletonUpgradeAmount = 100;
    public short numSkeletons = 0;

    public float timeSurvived = 0f;
    public TextMeshProUGUI resumeText;
    public TextMeshProUGUI pauseText;

    private SelectManager selectManager;

    // Start is called before the first frame update
    void Start()
    {
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            timeSurvived += Time.deltaTime;
        }
    }

    public void UpgradeMaxSkeletons()
    {
        UpdateBones((short)-maxSkeletonUpgradeAmount);
        maxSkeletons++;
        maxSkeletonUpgradeAmount += 50;
        selectManager.boneCostValue0.text = "-" + maxSkeletonUpgradeAmount;
        selectManager.troopCapacityText.text = numSkeletons + "\n" + maxSkeletons;
    }

    public void Hit(short damage)
    {
        if (health > 0)
        {
            health -= (short)(damage / defence);
            if (health <= 0)
            {
                //StartCoroutine(Death());
                health = 0;
                InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
                inputManager.Pause();
                inputManager.allowResume = false;
                resumeText.text = "Retry";
                pauseText.text = "Dead";
            }
            if (!selectManager.selectingObject)
            {
                selectManager.rectHealthBar.sizeDelta = new Vector2(((float)health / maxHealth) * selectManager.rectHealth, selectManager.rectHealthBar.rect.height);
                selectManager.healthValue.text = health + "\n" + maxHealth;
            }
        }
    }
    public void UpdateBones(short bonesDifference)
    {
        bones += bonesDifference;
        selectManager.boneValue.text = bones.ToString();
    }

    public void HealAll()
    {
        Debug.Log("Heals");
    }

    public void ArrowAttack()
    {
        Debug.Log("Arrow Attack");
    }
}
