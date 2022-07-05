using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressUpManager : MonoBehaviour
{
    public GameObject[] equipment;
    private GameObject player;
    private PlayerStatus playerStatus;
    private PlayerMovement playerMovement;

    public GameObject mainDoor;
    public GameObject fakeWalls;
    public GameObject extraDoors;
    private GameObject playerCanvas;

    private int completion = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCanvas = GameObject.FindGameObjectWithTag("Player Canvas");
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
        if (playerStatus.bossDefeated())
        {
            mainDoor.SetActive(false);
            fakeWalls.SetActive(false);
            extraDoors.SetActive(true);
            //DisplayMessage();
        }
    }
    private void DisplayMessage()
	{
        playerMovement.enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        playerCanvas.SetActive(false);
	}
}
