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

    public Attack[] arrowAttacks;

    public SelectManager selectManager;

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
        if (selectManager.healCooldown)
        {
            InvalidNotice notice = Instantiate(selectManager.impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Heal On Cooldown";
            notice.textPosition.anchoredPosition = new Vector2(-340f, 150f);
        }
        else
        {
            selectManager.HealAllCooldown();
            StartCoroutine(HealAllCoroutine());
        }
    }

    IEnumerator HealAllCoroutine()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(.5f);
            health += health < maxHealth - 5 && health > 0 ? (short) 5 : (maxHealth != health ? (short) (maxHealth - health) : (short)0);
            if (!selectManager.selectingObject)
            {
                selectManager.rectHealthBar.sizeDelta = new Vector2(((float)health / maxHealth) * selectManager.rectHealth, selectManager.rectHealthBar.rect.height);
                selectManager.healthValue.text = health + "\n" + maxHealth;
            }
            GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
            GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
            for (int j = 0; j < skeletons.Length; j++)
            {
                Skeleton skeleton = skeletons[j].GetComponent<Skeleton>();
                skeleton.health += skeleton.health < skeleton.maxHealth - 5 && skeleton.health > 0 ? (short)5 : (skeleton.maxHealth != skeleton.health ? (short)(skeleton.maxHealth - skeleton.health) : (short)0);
                if (selectManager.selectingObject && selectManager.selectedTroop == skeleton.transform)
                {
                    selectManager.rectHealthBar.sizeDelta = new Vector2(((float)skeleton.health / skeleton.maxHealth) * selectManager.rectHealth, selectManager.rectHealthBar.rect.height);
                    selectManager.healthValue.text = skeleton.health + "\n" + skeleton.maxHealth;
                }
            }
            for (int j = 0; j < minions.Length; j++)
            {
                Minion minion = minions[j].GetComponent<Minion>();
                minion.health += minion.health < minion.maxHealth - 5 && minion.health > 0 ? (short)5 : (minion.maxHealth != minion.health ? (short)(minion.maxHealth - minion.health) : (short)0);
                if (selectManager.selectingObject && selectManager.selectedTroop == minion.transform)
                {
                    selectManager.rectHealthBar.sizeDelta = new Vector2(((float)minion.health / minion.maxHealth) * selectManager.rectHealth, selectManager.rectHealthBar.rect.height);
                    selectManager.healthValue.text = minion.health + "\n" + minion.maxHealth;
                }
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    public void ArrowAttack()
    {
        if (selectManager.arrowCooldown)
        {
            InvalidNotice notice = Instantiate(selectManager.impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Arrow Attack On Cooldown";
            notice.textPosition.anchoredPosition = new Vector2(-190f, 150f);
        }
        else
        {
            selectManager.ArrowAttackCooldown();
            for (int i = 0; i < arrowAttacks.Length; i++)
            {
                arrowAttacks[i].StartArrowBarrageAttack();
            }
        }
    }
}
