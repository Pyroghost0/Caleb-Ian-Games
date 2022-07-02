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
    public BossEquipment bossEquipment;
    public BossBehavior bossBehavior;

    public Animator anim;

    public GameObject bossCamera;
    public GameObject mainCamera;
    private int buttonClicked = 0;
    private AudioManager audioManager;
    //0 = none, 1 = yes, 2 = no, 3 = continue

    private string[] introText = new string[]{
        "Stop right there!",
        "I know the atrocities you've commited! You think it's okay for you to walk around here so freely?",
        "I've been looking for you everywhere, but in the end, you walked right into my grasp!",
        "Huh? You don't know what you've done wrong?",
        "You're the one who made a character AND THREW AWAY ALL OF THE PIECES YOU DIDN'T WANT!",
        "...",
        "Did you not see what terrible creatures those discarded faces have become?",
        "They tried the best they could to manifest into something recognizable, but without someone choosing their bodies they're practically hopeless!",
        "Did you not see what happened to all of those excess hair colors and skin colors you so willfully threw away?",
        "They were only able to become tiny, mindless slimes!",
        "These abominations of creatures have been ravaging this world and killing all of its inhabitants!",
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
        "PREPARE TO BE ELIMINATED!"
    };

    private int[] textType = new int[]{
        1,
        2,
        0,
        0,
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

    };
    //0 = continue, 1 = choose, 2 = yes/no

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioManager = player.GetComponent<AudioManager>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
    private IEnumerator IntroProgress()
    {
        panel.SetActive(true);
        ChangeText(0);
        yield return new WaitForSeconds(2f);
        ChangeText(1);
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerMovement>().playerAnim.SetFloat("MoveX", 0);
        player.GetComponent<PlayerMovement>().playerAnim.SetFloat("MoveY", 0);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetActive(false);
        bossCamera.SetActive(true);

        for (int i = 2; i < 13; i++)
		{
            yield return new WaitUntil(() => buttonClicked > 0);
            ChangeText(i);
            buttonClicked = 0;
        }
        anim.SetTrigger("TakeOffHat");
        yield return new WaitForSeconds(2f);
        ChangeText(13);
        buttonClicked = 0;
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("PutOnHat");
        ChangeText(14);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        if (bossEquipment.equipedSet == 0)
		{
            ChangeText(15);
		}
        else if (bossEquipment.equipedSet == 1)
		{
            ChangeText(16);
		}
        else if (bossEquipment.equipedSet == 2)
		{
            ChangeText(17);
		}
		else
		{
            ChangeText(18);
		}
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(19);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(20);
        buttonClicked = 0;
        yield return new WaitUntil(() => buttonClicked > 0);
        ChangeText(21);
        buttonClicked = 0;
        yield return new WaitForSeconds(1f);
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.transform.GetChild(0).GetChild(1).GetComponent<Gun>().enabled = true;
        yield return new WaitForSeconds(1f);
        audioManager.BgmRestart();
        audioManager.BgmChangeVolume(1f);
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
}
