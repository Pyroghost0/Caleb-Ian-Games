using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreatorManager : MonoBehaviour
{
    //character data
    private bool isMale;
    private int headNumber = 2;
    private int skinColor = 0;
    private int hairColor = 0;

    public Text textBox;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject continueButton;

    public GameObject maleBaseButton;
    public GameObject[] maleHeadButton;
    public GameObject mHair;
    public GameObject[] mHead = new GameObject[3];

    public GameObject femaleBaseButton;
    public GameObject[] femaleHeadButton;
    public GameObject fHair;
    public GameObject[] fHead = new GameObject[3];

    public Material Skin;
    public Material Hair;
    public Color[] SkinBase;
    public Color[] SkinEmission;
    public Color[] HairBase;
    public Color[] HairEmission;
    public GameObject SkinColors;
    public GameObject HairColors;

    public Animator HeadsAnim;

    private int buttonClicked = 0;
    public string level;
    public bool costume = false;
    //0 = none, 1 = yes, 2 = no, 3 = continue, 4 = maleBase, 5 = femaleBase,
    //6 = headA, 7 = headB, 8 = headC, 11-17 = skin, 21-27 = hair

    private string[] introText = new string[]{
        "Welcome to Qualms. Before jumping into the game, please make a character.",
        "Choose a body type.",
        "Choose the male body and discard the female?",
        "Choose the female body and discard the male?",
        "Pick a face. You can only choose one, so pick carefully.",
        "Pick a face. You can only choose one, so pick carefully.",
        "Use this face?",
        "Choose a hair color and a skin color.",
        "Play the game with this character?",
        "Loading..."

    };
    
    private int[] textType = new int[]{
        0,
        1,
        2,
        2,
        1,
        0,
        2,
        0,
        2,
        1
    };
    //0 = continue, 1 = choose, 2 = yes/no
    
    void Start()
    {
        Skin.EnableKeyword("_EMISSION");
        Hair.EnableKeyword("_EMISSION");
        ChangeMaterial(true, 0);
        ChangeMaterial(false, 0);
        StartCoroutine(TextProgress());
        
    }
    private void ChangeMaterial(bool isSkin, int index)
    {
        if (isSkin)
		{
            Skin.SetColor("_Color", SkinBase[index]);
            Skin.SetColor("_EmissionColor", SkinEmission[index]);
            skinColor = index;
		}
		else
		{
            Hair.SetColor("_Color", HairBase[index]);
            Hair.SetColor("_EmissionColor", HairEmission[index]);
            hairColor = index;
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
    private void SwitchHead(int index)
	{
        Debug.Log("Switch head to " + index);
        mHair.SetActive(false);
        mHead[0].SetActive(false);
        mHead[1].SetActive(false);
        mHead[2].SetActive(false);
        fHair.SetActive(false);
        fHead[0].SetActive(false);
        fHead[1].SetActive(false);
        fHead[2].SetActive(false);
        if (isMale)
		{
            mHair.SetActive(true);
            mHead[index].SetActive(true);
        }
		else
		{
            fHair.SetActive(true);
            fHead[index].SetActive(true);
        }
        headNumber = index + 1;
	}
    private IEnumerator TextProgress()
	{
        //Intro
        ChangeText(-1);
        yield return new WaitForSeconds(0.5f);
        ChangeText(0);
        yield return new WaitUntil(() => buttonClicked == 3);
        buttonClicked = 0;
        maleBaseButton.SetActive(true);
        femaleBaseButton.SetActive(true);

        //Choose Male / Female
        ChangeText(1);
        yield return new WaitUntil(() => buttonClicked == 4 || buttonClicked == 5);

        do
        {
            if (buttonClicked == 4)
            {
                ChangeText(2);
                isMale = true;
            }
            else if (buttonClicked == 5)
            {
                ChangeText(3);
                isMale = false;
            }else if (buttonClicked == 2)
			{
                ChangeText(1);
            }
            buttonClicked = 0;
            yield return new WaitUntil(() => buttonClicked != 0);
        } while (buttonClicked != 1);
        buttonClicked = 0;

        if (isMale)
		{
            maleBaseButton.GetComponent<Animator>().SetTrigger("ChoseMale");
            femaleBaseButton.GetComponent<Animator>().SetTrigger("ChoseMale");
		}
		else
		{
            maleBaseButton.GetComponent<Animator>().SetTrigger("ChoseFemale");
            femaleBaseButton.GetComponent<Animator>().SetTrigger("ChoseFemale");
        }
        ChangeText(-1);
        yield return new WaitForSeconds(2f);
        ChangeText(4);
        if (isMale)
		{
            maleHeadButton[0].SetActive(true);
            maleHeadButton[1].SetActive(true);
            maleHeadButton[2].SetActive(true);
		}
		else
		{
            femaleHeadButton[0].SetActive(true);
            femaleHeadButton[1].SetActive(true);
            femaleHeadButton[2].SetActive(true);
		}
        
        //Choose Face
        do
        {
            yield return new WaitForSeconds(0.1f);
            if (buttonClicked == 3)
			{
                ChangeText(6);
            }
            else if (buttonClicked == 2)
			{
                ChangeText(5);
            }
            else if (buttonClicked != 0 && buttonClicked != 1)
			{
                SwitchHead(buttonClicked - 6);
                ChangeText(5);
            }
        } while (buttonClicked != 1);
        buttonClicked = 0;
        HeadsAnim.SetTrigger("ChoseHead");
        maleBaseButton.GetComponent<Animator>().SetTrigger("ChoseHead");
        femaleBaseButton.GetComponent<Animator>().SetTrigger("ChoseHead");
        ChangeText(-1);
        yield return new WaitForSeconds(2f);
        ChangeText(7);
        SkinColors.SetActive(true);
        HairColors.SetActive(true);

        //Choose Colors
        do
        {
            yield return new WaitForSeconds(0.1f);
            if (buttonClicked == 3)
            {
                ChangeText(8);
                SkinColors.SetActive(false);
                HairColors.SetActive(false);
            }
            else if (buttonClicked == 2)
            {
                ChangeText(7);
                SkinColors.SetActive(true);
                HairColors.SetActive(true);
            }
            else if (buttonClicked != 0 && buttonClicked != 1)
            {
                if (buttonClicked > 20)
                {
                    ChangeMaterial(false, buttonClicked - 21);
                    buttonClicked = 0;
                }
                else
                {
                    ChangeMaterial(true, buttonClicked - 11);
                    buttonClicked = 0;
                }
            }
        } while (buttonClicked != 1);
        buttonClicked = 0;

        StartCoroutine(WaitLoad());
        ChangeText(9);
    }

    IEnumerator WaitLoad()
    {
        if (level.Length == 0)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
            yield return new WaitUntil(() => playerManager.player != null);
            PlayerStatus playerStatus = playerManager.player.GetComponent<PlayerStatus>();
            playerStatus.isMale = isMale;
            playerStatus.headNumber = headNumber - 1;
            playerStatus.skinColor = skinColor;
            playerStatus.hairColor = hairColor;
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Character Creator"));
        }
        else if (costume)
        {

            AsyncOperation ao = SceneManager.LoadSceneAsync("Dress Up Room", LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
            yield return new WaitUntil(() => playerManager.player != null);
            PlayerStatus playerStatus = playerManager.player.GetComponent<PlayerStatus>();
            playerStatus.isMale = isMale;
            playerStatus.headNumber = headNumber - 1;
            playerStatus.skinColor = skinColor;
            playerStatus.hairColor = hairColor;
            playerStatus.changeCharacter();
            GameObject.FindGameObjectWithTag("Dress Up Door").GetComponent<DoorPortal>().nextSceneName = level;
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Character Creator"));
        }
        else
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
            yield return new WaitUntil(() => playerManager.player != null);
            PlayerStatus playerStatus = playerManager.player.GetComponent<PlayerStatus>();
            playerStatus.isMale = isMale;
            playerStatus.headNumber = headNumber - 1;
            playerStatus.skinColor = skinColor;
            playerStatus.hairColor = hairColor;
            playerStatus.changeCharacter();
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Character Creator"));
        }
    }
}
