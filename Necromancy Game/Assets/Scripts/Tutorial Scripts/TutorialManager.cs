using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject singleButtons;
    public GameObject doubleButtons;
    public GameObject holdButtons;
    public TextMeshProUGUI mainText;
    public RectTransform arrow;
    public GameObject corpsePrefab;
    public GameObject textBox;

    public TutorialInputManager tutorialInputManager;
    public InputManager inputManager;
    public SelectManager selectManager;

    public bool buttonPressed = false;
    private bool allThreePressed = false;
    private float timer = 0f;
    private float arrowYPosition = -110;
    private float arrowTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        inputManager.holdInputWait = true;
        tutorialInputManager.enabled = false;
        selectManager.boneCostObject0.SetActive(false);
        StartCoroutine(Tutorial());
    }

    // Update is called once per frame
    void Update()
    {
        arrowTimer += Time.deltaTime;
        if (arrowTimer > Mathf.PI) {
            arrowTimer -= Mathf.PI;
        }
        arrow.anchoredPosition = new Vector3(arrow.anchoredPosition.x, 10f * Mathf.Sin(arrowTimer * 4f) + arrowYPosition, 0f);
        if (allThreePressed)
        {
            if (!Input.GetKey(inputManager.leftButton) || !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton))
            {
                allThreePressed = false;
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                {
                    SceneManager.LoadScene("Main Menu");
                }
            }
        }
        else if (Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.middleButton) && Input.GetKey(inputManager.rightButton))
        {
            allThreePressed = true;
        }
    }

    IEnumerator Tutorial()
    {
        //Status
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.gameObject.SetActive(true);
        mainText.text = "This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.anchoredPosition += new Vector2(215f, 0f);
        mainText.text = "This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.anchoredPosition += new Vector2(125f, 0f);
        mainText.text = "This shows your bones, the currency of this game.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.anchoredPosition += new Vector2(265f, 0f);
        mainText.text = "This shows the amount of active troops over the max number of troops you can have active at one time.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));

        //Controls 1
        arrow.anchoredPosition += new Vector2(-75f, -80f);
        arrowYPosition -= 80f;
        selectManager.boneCostObject0.SetActive(true);
        holdButtons.SetActive(true);
        tutorialInputManager.enabled = true;
        mainText.text = "These buttons are activated when held down. The middle button upgrades the max troop capacity for the price at the top.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        mainText.text = "Try moving the camera by holding the right button";//This can be skipped if the player moves the camera
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 5f));
        arrow.anchoredPosition += new Vector2(-530f, 0f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        mainText.text = "These buttons are activated when pressed once. For now, select a corpse by pressing the middle button.";
        yield return new WaitUntil(() => (selectManager.selectingObject));

        //Corpse
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowHoldMovement = false;
        inputManager.holdInputWait = true;
        tutorialInputManager.allowMiddle = false;
        holdButtons.SetActive(false);
        mainText.text = "While selecting something, you can switch what you are selecting by pressing the left or right button. Select the other corpse.";
        Transform firstSelectedCorpse = selectManager.selectedTroop;//Since waituntil activates after Update() is called, we don't have to worry about the selected object being assigned after selectingObject is set to true
        yield return new WaitUntil(() => (selectManager.selectedTroop != firstSelectedCorpse));
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowRight = false;
        arrow.anchoredPosition += new Vector2(530f, 0f);
        holdButtons.SetActive(true);
        mainText.text = "Lets create a minion to gather bones. Hold the left button to create one from a corpse.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        mainText.text = "Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button to spawn some.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));

        //Minion
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition += new Vector2(-530f, -80f);
        Instantiate(corpsePrefab, new Vector3(3f, 0, 0f), corpsePrefab.transform.rotation);
        Instantiate(corpsePrefab, new Vector3(7f, -.5f, 0f), corpsePrefab.transform.rotation);
        mainText.text = "Minions have 2 modes, attack and dig. Minions can't be selected individually, and all follow the same command, which is set by pressing left or right when nothing is selected. I spawned more corpses for you, select one when you are ready.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        doubleButtons.SetActive(true);
        tutorialInputManager.allowDouble = true;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMiddle = false;
        arrow.anchoredPosition += new Vector2(265f, 0f);
        mainText.text = "You can press buttons twice to select these options. We could use some skeletons to attack for us, so hit the right button twice to change all corpses into skeletons.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));

        //Skeleton
        tutorialInputManager.allowDouble = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowMiddle = true;
        arrow.anchoredPosition += new Vector2(265f, 0f);
        mainText.text = "Skeletons can be upgraded by spending bones (using the amount indicated above). If you want the skeleton to die for whatever reason, you can do so holding the middle button.\nKILL THE NEWBORN...";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition += new Vector2(-265f, 0f);
        mainText.text = "Skeletons have three modes. Retreat make them return home ignoring enemies. Stay makes them stay in place, attacking enemies that get near. Attack makes them charge to the right, attacking any enemies. Deselect them by pressing the middle button";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMiddle = false;
        mainText.text = "You can control the mode all your skeletons are in when deselected. This marks the end of the tutorial,\n\nPress All 3 Buttons To Stay And Test Out The Controls\nHold All 3 Buttons To Return To Main Menu";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        tutorialInputManager.allowLeft = true;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowHoldMovement = true;
        inputManager.holdInputWait = false;
        arrow.gameObject.SetActive(false);
        textBox.SetActive(false);
    }
}
