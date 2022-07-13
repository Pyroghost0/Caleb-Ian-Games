using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public short bones = 0;
    public short health = 1000;
    public short maxHealth = 1000;
    public short maxSkeletons = 3;
    public short maxSkeletonUpgradeAmount = 100;
    public short numSkeletons = 0;

    private SelectManager SelectManager;

    // Start is called before the first frame update
    void Start()
    {

        SelectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeMaxSkeletons()
    {
        UpdateBones((short)-maxSkeletonUpgradeAmount);
        maxSkeletons++;
        maxSkeletonUpgradeAmount += 100;
    }

    public void Hit(short damage)
    {
        if (health > 0)
        {
            //health -= (short)(damage / defence);
            if (health <= 0)
            {
                //StartCoroutine(Death());
            }
        }
    }
    public void UpdateBones(short bonesDifference)
    {
        bones += bonesDifference;
        SelectManager.boneValue.text = bones.ToString();
    }
}
