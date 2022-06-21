using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressUpManager : MonoBehaviour
{
    public GameObject[] equipment;
    private PlayerStatus playerStatus;

    void Start()
    {
        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        for (int i = 0; i < equipment.Length; i++)
		{
            if (playerStatus.equipmentUnlocked[i])
			{
                equipment[i].SetActive(true);
			}
			else
			{
                equipment[i].SetActive(false);
            }
            if (playerStatus.equipedEquipment[i])
			{
                equipment[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}
    }
}
