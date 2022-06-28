using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMonologueManager : MonoBehaviour
{
    public Text textBox;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject continueButton;

    private int buttonClicked = 0;
    //0 = none, 1 = yes, 2 = no, 3 = continue

    private string[] introText = new string[]{
        "Stop right there!",
        "I know the atrocities you've commited! You think it's okay for you to walk around here so freely?",
        "I've been looking for you everywhere, but in the end, you walked right into my grasp!",
        "Huh? You don't know what you've done wrong?",
        "You're the one who made a character AND THREW AWAY ALL OF THE PIECES YOU DIDN'T WANT!",
        "*angry snarling*",
        "Did you not see what terrible creatures those discarded heads have become?",
        "They tried the best they could to manifest into something recognizable, but without someone choosing their bodies they're practically hopeless!",
        "Did you not see what happened to all of those excess hair colors and skin colors you so willfully threw away?",
        "They were only able to become tiny, mindless slimes!",
        "These abominations of creatures have been ravaging this world and killing all of its inhabitants!",
        "It didn't surprise you when you found the villages of the deer tribe completely abandoned, or when you couldn't find Joe in Joe's restaurant?",
        "And then there's what you did to me...",
        "WHY DIDN'T YOU GIVE ME A FACE?!",
        "Nevermind, give it no thought. You're a blight to this world, and I might as well exterminate you right here!",
        "Since you were mindless enough to leave behind the entire set of Mech Armor from 568011, I've snatced it!",
        "Since you were mindless enough to leave behind the entire Deer Tribe getup from that forest, I've snatced it!",
        "Since you were mindless enough to leave behind the entire cake baker outfit from Joe's restaurant, I've snatced it!",
        "Since you were mindless enough to leave behind the President's suit and wig from The White House, I've snatced them!",
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
        0,
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,

    };
    //0 = continue, 1 = choose, 2 = yes/no

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
    private IEnumerator TextProgress()
    {
        yield return new WaitForSeconds(2f);
    }
}
