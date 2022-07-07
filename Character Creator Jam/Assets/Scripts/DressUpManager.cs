using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DressUpManager : MonoBehaviour
{
    public GameObject[] equipment;
    private PlayerManager playerManager;
    private GameObject player;
    private PlayerStatus playerStatus;
    private PlayerMovement playerMovement;

    public GameObject mainDoor;
    public GameObject fakeWalls;
    public GameObject extraDoors;
    private GameObject playerCanvas;
    public GameObject dressUpCanvas;
    public GameObject text;
    public GameObject optionalText;

    void Start()
    {
        StartCoroutine(WaitFindPlayer());
    }
    private IEnumerator WaitFindPlayer()
	{
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        yield return new WaitUntil(() => playerManager.player != null);
        player = playerManager.player;
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
            DisplayMessage();
        }
    }
    private void DisplayMessage()
	{
        dressUpCanvas.SetActive(true);
        text.GetComponent<TextMeshProUGUI>().text = "Thank You For Playing!\n\nCompletion: " + Completion() + "%";
        if (Completion() < 100)
		{
            optionalText.SetActive(true);
		}
        playerMovement.enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        playerCanvas.SetActive(false);
        Time.timeScale = 0f;
    }
    public void MainMenu()
	{
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Screen");
    }
    public void Continue()
    {
        Time.timeScale = 1f;
        dressUpCanvas.SetActive(false);
        playerMovement.enabled = true;
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        playerCanvas.SetActive(true);
    }
    private int Completion()
	{
        int completion = 0;
        for (int i = 0; i < 12; i++)
        {
            if (playerStatus.equipmentUnlocked[i])
			{
                completion += 3;
			}
        }
        for (int i = 0; i < 4; i++)
        {
            if (playerStatus.setsCompleted[i])
			{
                completion += 3;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (playerStatus.levelsUnlocked[i])
			{
                completion += 10;
            }
        }
        if (playerStatus.bossDefeated())
		{
            completion += 12;
		}
        return completion;
    }
}
