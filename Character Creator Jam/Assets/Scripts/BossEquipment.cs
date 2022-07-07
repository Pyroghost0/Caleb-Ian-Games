using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEquipment : MonoBehaviour
{
    public bool isMale = false;
    public GameObject[] maleObjects;
    public GameObject[] femaleObjects;
    public GameObject[] selfEquipment;
    public int equipedSet; //0 = mech, 1 = deer, 2 = baker, 3 = potus

    private PlayerManager playerManager;
    private PlayerStatus playerStatus;
    private int offset;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitFindPlayer());
    }
	private IEnumerator WaitFindPlayer()
	{
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        yield return new WaitUntil(() => playerManager.player != null);
        playerStatus = playerManager.player.GetComponent<PlayerStatus>();
        if (playerStatus.isMale)
        {
            isMale = true;
            for (int i = 0; i < maleObjects.Length; i++)
            {
                femaleObjects[i].SetActive(false);
                maleObjects[i].SetActive(true);
            }
        }
        else
        {
            isMale = false;
            for (int i = 0; i < femaleObjects.Length; i++)
            {
                femaleObjects[i].SetActive(true);
                maleObjects[i].SetActive(false);
            }
        }
        offset = isMale ? 12 : 0;

        if (playerStatus.equipedEquipment[0] == false && playerStatus.equipedEquipment[1] == false && playerStatus.equipedEquipment[2] == false)
        {
            equipedSet = 0;
            selfEquipment[0 + offset].SetActive(true);
            selfEquipment[1 + offset].SetActive(true);
            selfEquipment[2 + offset].SetActive(true);

        }
        else if (playerStatus.equipedEquipment[3] == false && playerStatus.equipedEquipment[4] == false && playerStatus.equipedEquipment[5] == false)
        {
            equipedSet = 1;
            selfEquipment[3 + offset].SetActive(true);
            selfEquipment[4 + offset].SetActive(true);
            selfEquipment[5 + offset].SetActive(true);
        }
        else if (playerStatus.equipedEquipment[6] == false && playerStatus.equipedEquipment[7] == false && playerStatus.equipedEquipment[8] == false)
        {
            equipedSet = 2;
            selfEquipment[6 + offset].SetActive(true);
            selfEquipment[7 + offset].SetActive(true);
            selfEquipment[8 + offset].SetActive(true);
        }
        else
        {
            equipedSet = 3;
            selfEquipment[9 + offset].SetActive(true);
            selfEquipment[10 + offset].SetActive(true);
            selfEquipment[11 + offset].SetActive(true);
            maleObjects[0].SetActive(false);
            femaleObjects[0].SetActive(false);
        }
    }
}
