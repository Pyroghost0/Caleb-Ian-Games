using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorManager : MonoBehaviour
{
    //character data
    private bool isMale;
    private int headNumber;
    private int skinColor;
    private int hairColor;

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

    public Animator HeadsAnim;

    private int buttonClicked = 0;//0 = none, 1 = yes, 2 = no, 3 = continue, 4 = maleBase, 5 = femaleBase, 6 = headA, 7 = headB, 8 = headC

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
    //0 = continue, 1 = choose, 2 = yes/no
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
    void Start()
    {
        StartCoroutine(TextProgress());
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
            yield return new WaitForSeconds(0.5f);
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
    }
}
