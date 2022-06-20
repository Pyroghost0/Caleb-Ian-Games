using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreatorManager : MonoBehaviour
{
    //character data
    private bool isMale;
    private int headNumber;
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

    public Material[] Skin;
    public Material[] Hair;
    public Animator HeadsAnim;

    private int buttonClicked = 0;
    //0 = none, 1 = yes, 2 = no, 3 = continue, 4 = maleBase, 5 = femaleBase,
    //6 = headA, 7 = headB, 8 = headC, 11-17 = skin, 21-27 = hair

    private string[] introText = new string[]{
        "Before entering the world, you must create a character.",
        "Choose a body type.",
        "Choose the male body and discard the female?",
        "Choose the female body and discard the male?",
        "Pick a face.",
        "Use this face?",
        "Choose a hair color and a skin color.",
        "Play the game with this character?"

    };
    
    private int[] textType = new int[]{
        0,
        1,
        2,
        2,
        0,
        2,
        0,
        2
    };
    //0 = continue, 1 = choose, 2 = yes/no
    
    void Start()
    {
        StartCoroutine(TextProgress());
        
    }
    private void ChangeMaterial(bool isSkin, int index)
    {
        //MeshRenderer[] renderers;
        /*if (isMale)
        {
            renderers = maleBaseButton.transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        }
		else
		{
            renderers = femaleBaseButton.transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials[0] = Skin[index];
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                renderers[i].sharedMaterials[j] = Skin[index];
                Debug.Log(renderers[i].materials[j].name);
                Debug.Log(Skin[index].name);
                if (isSkin && renderers[i].sharedMaterials[j].name.StartsWith("Skin"))
                {
                    renderers[i].sharedMaterials[j] = Skin[index];
                }
                if (!isSkin && renderers[i].sharedMaterials[j].name.StartsWith("Hair"))
                {
                    renderers[i].sharedMaterials[j] = Hair[index];
                }
            }
        }*/
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
                ChangeText(5);
            }
            else if (buttonClicked == 2)
			{
                ChangeText(4);
            }
            else if (buttonClicked != 0 && buttonClicked != 1)
			{
                SwitchHead(buttonClicked - 6);
            }
        } while (buttonClicked != 1);
        buttonClicked = 0;
        HeadsAnim.SetTrigger("ChoseHead");
        maleBaseButton.GetComponent<Animator>().SetTrigger("ChoseHead");
        femaleBaseButton.GetComponent<Animator>().SetTrigger("ChoseHead");
        ChangeText(-1);
        yield return new WaitForSeconds(2f);
        ChangeText(6);

        //Choose Colors
        /*do
        {
            yield return new WaitForSeconds(0.1f);
            if (buttonClicked == 3)
            {
                ChangeText(7);
            }
            else if (buttonClicked == 2)
            {
                ChangeText(6);
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
        buttonClicked = 0;*/

        //Confirm Character
        ChangeText(7);
        yield return new WaitUntil(() => buttonClicked == 1);
        StartCoroutine(WaitLoad());
    }

    IEnumerator WaitLoad()
    {
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        AsyncOperation ao2 = SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone && ao2.isDone);

        PlayerStatus playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        playerStatus.isMale = isMale;
        playerStatus.headNumber = headNumber-1;
        playerStatus.skinColor = skinColor;
        playerStatus.hairColor = hairColor;
        playerStatus.isTutorial = true;
        playerStatus.changeCharacter();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Character Creator"));
    }
}
