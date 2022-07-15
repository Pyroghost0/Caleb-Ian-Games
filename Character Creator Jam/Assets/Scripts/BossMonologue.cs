using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossMonologue : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI textBox;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject continueButton;
    private GameObject player;
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    private GameObject playerCanvas;
    public BossEquipment bossEquipment;
    public BossBehavior bossBehavior;

    public Animator anim;

    public GameObject bossCamera;
    public GameObject mainCamera;
    private int buttonClicked = 0;
    private AudioManager audioManager;
    public GameObject endingWall;
    public AudioSource audio;
    //0 = none, 1 = yes, 2 = no, 3 = continue

    private string[] introText = new string[]{
        "So, you finally decided to show your face...",
        "The signs I left behind seem to have done their job. You've fallen right into my trap! It's time to atone for your sins!",
        "Huh? You don't know what you've done wrong?",
        "You're the one who created a character,",
        "THEN THREW AWAY ALL OF THE PIECES YOU DIDN'T WANT!",
        "You didn't see the terrible creatures those pieces became without your input?",
        "The mindless slimes that formed from all of the colors you discarded, and the monsters that formed from the faces you trashed...",
        "Those abominations of creatures have been ravaging this world and killing all of its inhabitants!",
        "It didn't surprise you when you found the villages of the deer tribe completely abandoned, or when you couldn't find Joe in Joe's restaurant?",
        "And then there's what you did to me...",
        "IT'S YOUR FAULT THAT I DON'T HAVE A FACE!",
        "Nevermind, give it no thought. You're a blight to this world, and I might as well exterminate you right here!",
        "Since you were mindless enough to leave behind the entire set of Mech Armor from 568011, I've snatched it!",
        "Since you were mindless enough to leave behind the entire Deer Tribe getup from that forest, I've snatched it!",
        "Since you were mindless enough to leave behind the entire cake baker outfit from Joe's restaurant, I've snatched it!",
        "Since you were mindless enough to leave behind the President's suit and wig from The White House, I've snatched them!",
        "You don't know the secret power of this outfit do you?",
        "When all three pieces are worn together in this stadium, the user gets 10X HEALTH, 5X BULLET STRENGTH, and INFINITE AMMO!",
        "PREPARE TO BE ELIMINATED!",
        "So that's it for me, huh?",
        "I guess I overestimated... my strength..."
    };

    private int[] textType = new int[]{
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        0,
        0

    };
    //0 = continue, 1 = choose, 2 = yes/no

    private void FindPlayer()
	{
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        if (playerManager.player != null)
		{
            player = playerManager.player;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerCanvas = GameObject.FindGameObjectWithTag("Player Canvas");
            audioManager = player.GetComponent<AudioManager>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    private void ChangeText(int textIndex)
    {
        if (textIndex == -1)
        {
            textBox.text = "";
            yesButton.SetActive(false);
            noButton.SetActive(false);
            continueButton.SetActive(false);
        }
        else if (textType[textIndex] == 0)
        {
            textBox.text = introText[textIndex];
            yesButton.SetActive(false);
            noButton.SetActive(false);
            continueButton.SetActive(true);
        }
        else if (textType[textIndex] == 1)
        {
            textBox.text = introText[textIndex];
            yesButton.SetActive(false);
            noButton.SetActive(false);
            continueButton.SetActive(false);
        }
        else if (textType[textIndex] == 2)
        {
            textBox.text = introText[textIndex];
            yesButton.SetActive(true);
            noButton.SetActive(true);
            continueButton.SetActive(false);
        }
    }
    public void ButtonClicked(int buttonIndex)
    {
        buttonClicked = buttonIndex;
    }
    public void StartIntro()
	{
        StartCoroutine(IntroProgress());
	}
    public void StartOutro()
	{
        StartCoroutine(OutroProgress());
	}
    private IEnumerator IntroProgress()
    {
        panel.SetActive(true);
        ChangeText(0);
        while (player == null)
		{
            FindPlayer();
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1.8f);
        ChangeText(1);
        playerMovement.playerAnim.SetFloat("MoveX", 0);
        playerMovement.playerAnim.SetFloat("MoveY", 0);
        playerMovement.enabled = false;
        playerMovement.walk.enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerCanvas.transform.GetChild(0).gameObject.SetActive(false);
        playerCanvas.transform.GetChild(1).gameObject.SetActive(false);
        playerCanvas.transform.GetChild(2).gameObject.SetActive(false);
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetActive(false);
        bossCamera.SetActive(true);

        for (int i = 2; i < 10; i++)
		{
            yield return new WaitUntil(() => buttonClicked > 0);
            ChangeText(i);
            buttonClicked = 0;
        }
        anim.SetTrigger("TakeOffHat");
        yield return new WaitForSeconds(2f);
        ChangeText(10);
        buttonClicked = 0;
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("PutOnHat");
        ChangeText(11);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        if (bossEquipment.equipedSet == 0)
		{
            ChangeText(12);
		}
        else if (bossEquipment.equipedSet == 1)
		{
            ChangeText(13);
		}
        else if (bossEquipment.equipedSet == 2)
		{
            ChangeText(14);
		}
		else
		{
            ChangeText(15);
		}
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(16);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(17);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(18);
        buttonClicked = 0;
        yield return new WaitForSeconds(1f);
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement.enabled = true;
        playerCanvas.transform.GetChild(0).gameObject.SetActive(true);
        playerCanvas.transform.GetChild(1).gameObject.SetActive(true);
        playerCanvas.transform.GetChild(2).gameObject.SetActive(true);
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = true;
        yield return new WaitForSeconds(1f);
        audioManager.BgmRestart();
        audioManager.BgmChangeVolume(0.5f);
        audioManager.BgmChangePitch(1.2f);
        panel.SetActive(false);
        gameObject.transform.localScale *= 1.2f;
        yield return new WaitForSeconds(0.5f);
        audioManager.BgmChangePitch(1.6f);

        bossCamera.SetActive(false);
        mainCamera.SetActive(true);
        audioManager.ToggleBgmEcho(true);
        gameObject.transform.localScale *= 1.5f;
        bossBehavior.enabled = true;
        bossBehavior.StartFight();

    }
    private IEnumerator OutroProgress()
	{
        bossBehavior.enabled = false;
        player.GetComponent<PlayerStatus>().HealHealth(200f);

        yield return new WaitForSeconds(2f);
        playerMovement.playerAnim.SetFloat("MoveX", 0);
        playerMovement.playerAnim.SetFloat("MoveY", 0);
        playerMovement.enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<PlayerStatus>().DefeatBoss();
        playerCanvas.transform.GetChild(0).gameObject.SetActive(false);
        playerCanvas.transform.GetChild(1).gameObject.SetActive(false);
        playerCanvas.transform.GetChild(2).gameObject.SetActive(false);
        GameObject bossBar = GameObject.FindGameObjectWithTag("Boss Health Bar");
        for (int i = 0; i < bossBar.transform.childCount; i++)
        {
            bossBar.transform.GetChild(i).gameObject.SetActive(false);
        }
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetActive(false);
        bossCamera.SetActive(true);
        audioManager.BgmChangeVolume(0.2f);
        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        yield return new WaitForSeconds(0.5f);
        audioManager.BgmChangeVolume(0f);
        panel.SetActive(true);
        ChangeText(19);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(20);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);

        Cursor.lockState = CursorLockMode.Locked;
        playerMovement.enabled = true;
        playerMovement.walk.enabled = true;
        playerCanvas.transform.GetChild(0).gameObject.SetActive(true);
        playerCanvas.transform.GetChild(1).gameObject.SetActive(true);
        playerCanvas.transform.GetChild(2).gameObject.SetActive(true);
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = true;
        audio.Play();
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        bossCamera.SetActive(false);
        mainCamera.SetActive(true);
        endingWall.SetActive(false);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
