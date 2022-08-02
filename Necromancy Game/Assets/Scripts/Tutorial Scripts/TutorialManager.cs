using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject singleButtons;
    public GameObject doubleButtons;
    public GameObject holdButtons;
    public TextMeshProUGUI mainText;
    public RectTransform arrowBasisObject;
    public RectTransform arrow;
    public RectTransform arrow1;
    public RectTransform arrow2;
    public RectTransform arrow3;
    public RectTransform arrow4;
    public GameObject corpsePrefab;
    public GameObject textBox;
    public GameObject loading;
    public GameObject tutorialObjectsLevel1;
    public GameObject tutorialObjectsLevel1Part2;
    public GameObject tutorialObjectsLevel2;
    public GameObject tutorialObjectsLevel2Part2;

    public TutorialInputManager tutorialInputManager;
    public InputManager inputManager;
    public SelectManager selectManager;

    public bool buttonPressed = false;
    private bool waitDueToPause = false;
    private bool allThreePressed = false;
    private bool clickedContinue = false;
    private float timer = 0f;
    private float arrowTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        arrowTimer += Time.deltaTime;
        if (arrowTimer > Mathf.PI) {
            arrowTimer -= Mathf.PI;
        }
        arrowBasisObject.anchoredPosition = new Vector3(0f, 10f * Mathf.Sin(arrowTimer * 4f), 0f);
        if (allThreePressed && !inputManager.paused && !clickedContinue && !waitDueToPause)
        {
            if (!Input.GetKey(inputManager.leftButton) || !Input.GetKey(inputManager.middleButton) || !Input.GetKey(inputManager.rightButton))
            {
                allThreePressed = false;
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= .5f)
                {
                    inputManager.resumeToTutorial = true;
                    inputManager.enabled = true;
                    inputManager.Pause();
                    tutorialInputManager.enabled = false;
                    waitDueToPause = true;
                    timer = 0f;
                }
            }
        }
        else if ((Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.middleButton) && Input.GetKey(inputManager.rightButton)) && (Input.GetKeyDown(inputManager.leftButton) || Input.GetKeyDown(inputManager.middleButton) || Input.GetKeyDown(inputManager.rightButton)))
        {
            allThreePressed = true;
            waitDueToPause = false;
        }
    }

    /*IEnumerator LoadMainMenu()
    {
        loading.SetActive(true);
        Scene curentScene = SceneManager.GetActiveScene();
        AsyncOperation ao = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao.isDone);
        MainMenuManager mainMenuManager = GameObject.FindGameObjectWithTag("Main Menu Manager").GetComponent<MainMenuManager>();
        mainMenuManager.leftButton = inputManager.leftButton;
        mainMenuManager.middleButton = inputManager.middleButton;
        mainMenuManager.rightButton = inputManager.rightButton;
        mainMenuManager.leftButtonText.text = mainMenuManager.leftButton.ToString();
        mainMenuManager.middleButtonText.text = mainMenuManager.middleButton.ToString();
        mainMenuManager.rightButtonText.text = mainMenuManager.rightButton.ToString();
        SceneManager.UnloadSceneAsync(curentScene);
    }*/

    public void StartTutorial()
    {
        textBox.SetActive(true);
        tutorialInputManager.enabled = true;
        this.enabled = true;
        inputManager.holdInputWait = true;
        selectManager.boneCostObject0.SetActive(false);
        if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>().level == 1)
        {
            StartCoroutine(Level1Tutorial());
        }
        else if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>().level == 2)
        {
            StartCoroutine(Level1Tutorial());
        }
        else
        {
            StartCoroutine(SpecialTutorial());
        }
    }

    public void ContinueButtonPress()
    {
        StartCoroutine(ConinueButtonPressCoroutine());
    }

    IEnumerator ConinueButtonPressCoroutine()
    {
        allThreePressed = true;
        clickedContinue = true;
        timer = 0f;
        yield return new WaitForFixedUpdate();
        allThreePressed = false;
        clickedContinue = false;
    }

    IEnumerator Level1Tutorial()
    {
        //Menus
        tutorialObjectsLevel1.SetActive(true);
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        playerBase.maxSkeletons = 1;
        selectManager.healCooldown = true;
        selectManager.arrowCooldown = true;
        selectManager.troopCapacityText.text = "0\n1";
        singleButtons.SetActive(false);
        doubleButtons.SetActive(false);
        holdButtons.SetActive(false);
        selectManager.minionStatus.gameObject.SetActive(false);
        mainText.text = "First of all, thanks for playing.\nThis game can be played with a mouse or keyboard, but it is recomended to play with both.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        mainText.text = "You can pause by holding the 3 buttons, pressing Esc, or the invisible pause button on the top left corner.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Status
        arrowBasisObject.gameObject.SetActive(true);
        mainText.text = "This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow2.gameObject.SetActive(false);
        arrow1.anchoredPosition = new Vector2(-25, -100f);
        mainText.text = "This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(75, -100f);
        mainText.text = "This shows your bones, the currency of this game.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(340f, -100f);
        mainText.text = "This shows the amount of active troops over the max number of troops you can have active at one time.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Controls 1
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(265f, -165f);
        arrow1.anchoredPosition = new Vector2(190f, -180f);
        arrow2.gameObject.SetActive(true);
        arrow2.anchoredPosition = new Vector2(340f, -180f);
        selectManager.boneCostObject0.SetActive(true);
        holdButtons.SetActive(true);
        tutorialInputManager.enabled = true;
        tutorialInputManager.allowMouseMovement = true;
        continueButton.SetActive(false);
        mainText.text = "These buttons are activated when held down. The middle button upgrades the max troop capacity for the price above. Upgrades can also be clicked on.\n\nUpgrade Your Troop Capasity";
        playerBase.UpdateBones(50);
        yield return new WaitUntil(() => (playerBase.maxSkeletons == 2));
        arrow.anchoredPosition = new Vector2(340f, -180f);
        arrow2.anchoredPosition = new Vector2(265f, -165f);
        mainText.text = "Try moving the camera by holding the right button or holding the mouse down and moving right";//This can be skipped if the player moves the camera beforehand
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 3.5f));
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-190f, -180f);
        arrow2.anchoredPosition = new Vector2(-340f, -180f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMouseSelect = true;
        mainText.text = "These buttons are activated when pressed once. For now, select a corpse by pressing the middle button or clicking on one.";
        yield return new WaitUntil(() => (selectManager.selectingObject));


        //Corpse & Selecting
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelect = false;
        tutorialInputManager.allowHoldMovement = false;
        inputManager.holdInputWait = true;
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(190f, -180f);
        singleButtons.SetActive(false);
        selectManager.corpseSkeletonButton.SetActive(false);
        selectManager.corpseTombstoneButton.SetActive(false);
        mainText.text = "Lets create a minion to gather some bones. Hold the left button to create one from a corpse or click the button to create one.";
        yield return new WaitUntil(() => (playerBase.numSkeletons == 1));
        arrow.anchoredPosition = new Vector2(-190f, -180f);
        arrow4.gameObject.SetActive(true);
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowMouseSelect = true;
        singleButtons.SetActive(true);
        holdButtons.SetActive(false);
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().speedAcceleration = 0f;
        selectManager.minionStatus.gameObject.SetActive(false);
        mainText.text = "Oops... looks like the minion needs some tombstones first. While selecting something, you can switch what you are selecting by pressing the left or right button or clicking on the other corpse.\nSelect The Other Corpse To Continue";
        Transform firstSelectedCorpse = selectManager.selectedTroop;//Since waituntil activates after Update() is called, we don't have to worry about the selected object being assigned after selectingObject is set to true
        yield return new WaitUntil(() => (selectManager.selectedTroop != firstSelectedCorpse));
        arrow4.gameObject.SetActive(false);
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelect = false;
        arrow.anchoredPosition = new Vector2(265f, -165f);
        holdButtons.SetActive(true);
        selectManager.corpseMinionButton.SetActive(false);
        mainText.text = "Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button or click it to spawn some.";
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Grave").Length != 0));


        //Minion
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().speedAcceleration = 2f;
        mainText.text = "You can also upgrade a minion's shovel, which upgrades the amount of bones they mine and their attack power. Lets try to upgrade it by holding the middle button or clicking on it.";
        playerBase.UpdateBones(25);
        yield return new WaitUntil(() => (GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().diggingAttack.attackPower != 6));
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseSelectSingle = true;
        arrow1.gameObject.SetActive(true);
        arrow1.anchoredPosition = new Vector2(190f, -100f);
        arrow.anchoredPosition = new Vector2(340f, -180f);
        selectManager.minionStatus.gameObject.SetActive(true);
        mainText.text = "Minions have 2 modes, attack and dig, which is indicated above. Curently, the minion is in dig mode, hold the right button to change them into attack mode or click on the minion (clicking changes the minion back to dig mode too).";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        arrow1.gameObject.SetActive(false);
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelectSingle = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMouseSelectHold = true;
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        mainText.text = "We're done with the minion for now, so lets deselect it.\nTo deselect objects, press the middle button once or hold the mouse on the selected object.";
        yield return new WaitUntil(() => (!selectManager.selectingObject));
        yield return new WaitForSeconds(.5f);
        arrow.gameObject.SetActive(false);
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMouseSelectHold = false;
        selectManager.selectableObjects.Remove(GameObject.FindGameObjectWithTag("Minion").transform);
        GameObject.FindGameObjectWithTag("Minion").SetActive(false);
        tutorialObjectsLevel1Part2.SetActive(true);
        selectManager.selectableObjects.Add(GameObject.FindGameObjectWithTag("Minion").transform);
        selectManager.transform.position = new Vector3(0f, 0f, -10f);
        mainText.text = "Lets see how a single minion does ageinst a goblin...";
        yield return new WaitForFixedUpdate();
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().inDiggingMode = false;
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Minion").Length == 0));
        yield return new WaitForSeconds(.5f);


        //Ending
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseBaseSingle = true;
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(-265f, -180f);
        mainText.text = "Oh... Your under attack, I wonder why? Lets fight back with some arrows of our own. Press the right button or click your base.";
        buttonPressed = false;
        selectManager.arrowCooldown = false;
        selectManager.healCooldown = false;
        while (!buttonPressed)
        {
            if (playerBase.health < 950)
            {
                playerBase.defence = 100;
            }
            yield return new WaitForFixedUpdate();
        }
        if (playerBase.health > 989)
        {
            playerBase.Hit(25);
        }
        playerBase.defence = 100;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseBaseSingle = false;
        tutorialInputManager.allowLeft = true;
        tutorialInputManager.allowMouseBaseHold = true;
        arrow.anchoredPosition = new Vector2(-340f, -180f);
        mainText.text = "Those arrows don't always garentee a kill and they require a cooldown so use them as a last ditch effort. Either way, you took some damage, so heal yourself by pressing the left button or holding down on the base.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMouseBaseHold = false;
        arrow.gameObject.SetActive(false);
        continueButton.SetActive(true);
        mainText.text = "Healing also heals all troups, but it still requires a cooldown. There is still more, but thats all you need to learn for the 1st level\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Reset
        playerBase.timeSurvived = 1f;
        inputManager.allowResume = false;
        inputManager.Resume();
    }

    IEnumerator Level2Tutorial()
    {
        //Start
        tutorialObjectsLevel2.SetActive(true);
        selectManager.Select(GameObject.FindGameObjectWithTag("Corpse").transform);
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        doubleButtons.SetActive(false);
        arrowBasisObject.gameObject.SetActive(true);
        arrow.gameObject.SetActive(true);
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(340f, -180f);
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowHoldMovement = false;
        selectManager.corpseMinionButton.SetActive(false);
        selectManager.corpseTombstoneButton.SetActive(false);
        mainText.text = "Now that you defeated the goblin king, you can summon goblin skeletons from goblin corpses. Lets create a skeleton by holding down the right button or clicking on it.";
        yield return new WaitUntil(() => (playerBase.numSkeletons != 0));
        mainText.text = "You can pause by holding the 3 buttons, pressing Esc, or the invisible pause button on the top left corner.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Skeleton
        mainText.text = "This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow2.gameObject.SetActive(false);
        arrow1.anchoredPosition = new Vector2(-25, -100f);
        mainText.text = "This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(75, -100f);
        mainText.text = "This shows your bones, the currency of this game.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(340f, -100f);
        mainText.text = "This shows the amount of active troops over the max number of troops you can have active at one time.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Controls 1
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(265f, -165f);
        arrow1.anchoredPosition = new Vector2(190f, -180f);
        arrow2.gameObject.SetActive(true);
        arrow2.anchoredPosition = new Vector2(340f, -180f);
        selectManager.boneCostObject0.SetActive(true);
        holdButtons.SetActive(true);
        tutorialInputManager.enabled = true;
        tutorialInputManager.allowMouseMovement = true;
        continueButton.SetActive(false);
        mainText.text = "These buttons are activated when held down. The middle button upgrades the max troop capacity for the price above. Upgrades can also be clicked on.\n\nUpgrade Your Troop Capasity";
        playerBase.UpdateBones(50);
        yield return new WaitUntil(() => (playerBase.maxSkeletons == 2));
        arrow.anchoredPosition = new Vector2(340f, -180f);
        arrow2.anchoredPosition = new Vector2(265f, -165f);
        mainText.text = "Try moving the camera by holding the right button or holding the mouse down and moving right";//This can be skipped if the player moves the camera beforehand
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 3.5f));
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-190f, -180f);
        arrow2.anchoredPosition = new Vector2(-340f, -180f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMouseSelect = true;
        mainText.text = "These buttons are activated when pressed once. For now, select a corpse by pressing the middle button or clicking on one.";
        yield return new WaitUntil(() => (selectManager.selectingObject));


        //Corpse & Selecting
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelect = false;
        tutorialInputManager.allowHoldMovement = false;
        inputManager.holdInputWait = true;
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(190f, -180f);
        singleButtons.SetActive(false);
        selectManager.corpseSkeletonButton.SetActive(false);
        selectManager.corpseTombstoneButton.SetActive(false);
        mainText.text = "Lets create a minion to gather some bones. Hold the left button to create one from a corpse or click the button to create one.";
        yield return new WaitUntil(() => (playerBase.numSkeletons == 1));
        arrow.anchoredPosition = new Vector2(-190f, -180f);
        arrow4.gameObject.SetActive(true);
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowMouseSelect = true;
        singleButtons.SetActive(true);
        holdButtons.SetActive(false);
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().speedAcceleration = 0f;
        selectManager.minionStatus.gameObject.SetActive(false);
        mainText.text = "Oops... looks like the minion needs some tombstones first. While selecting something, you can switch what you are selecting by pressing the left or right button or clicking on the other corpse.\nSelect The Other Corpse To Continue";
        Transform firstSelectedCorpse = selectManager.selectedTroop;//Since waituntil activates after Update() is called, we don't have to worry about the selected object being assigned after selectingObject is set to true
        yield return new WaitUntil(() => (selectManager.selectedTroop != firstSelectedCorpse));
        arrow4.gameObject.SetActive(false);
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelect = false;
        arrow.anchoredPosition = new Vector2(265f, -165f);
        holdButtons.SetActive(true);
        selectManager.corpseSkeletonButton.SetActive(false);
        selectManager.corpseMinionButton.SetActive(false);
        mainText.text = "Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button or click it to spawn some.";
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Grave").Length != 0));


        //Minion
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().speedAcceleration = 2f;
        mainText.text = "You can also upgrade a minion's shovel, which upgrades the amount of bones they mine and their attack power. Lets try to upgrade it by holding the middle button or clicking on it.";
        playerBase.UpdateBones(25);
        yield return new WaitUntil(() => (GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().diggingAttack.attackPower != 6));
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseSelectSingle = true;
        arrow1.gameObject.SetActive(true);
        arrow1.anchoredPosition = new Vector2(190f, -100f);
        arrow.anchoredPosition = new Vector2(340f, -180f);
        selectManager.minionStatus.gameObject.SetActive(true);
        mainText.text = "Minions have 2 modes, attack and dig, which is indicated above. Curently, the minion is in dig mode, hold the right button to change them into attack mode or click on the minion (clicking changes the minion back to dig mode too).";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        arrow1.gameObject.SetActive(false);
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseSelectSingle = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMouseSelectHold = true;
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        mainText.text = "We're done with the minion for now, so lets deselect it.\nTo deselect objects, press the middle button once or hold the mouse on the selected object.";
        yield return new WaitUntil(() => (!selectManager.selectingObject));
        yield return new WaitForSeconds(.5f);
        arrow.gameObject.SetActive(false);
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMouseSelectHold = false;
        selectManager.selectableObjects.Remove(GameObject.FindGameObjectWithTag("Minion").transform);
        GameObject.FindGameObjectWithTag("Minion").SetActive(false);
        tutorialObjectsLevel1Part2.SetActive(true);
        selectManager.selectableObjects.Add(GameObject.FindGameObjectWithTag("Minion").transform);
        selectManager.transform.position = new Vector3(0f, 0f, -10f);
        mainText.text = "Lets see how a single minion does ageinst a goblin...";
        yield return new WaitForFixedUpdate();
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().inDiggingMode = false;
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Minion").Length == 0));
        yield return new WaitForSeconds(.5f);


        //Ending
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseBaseSingle = true;
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(-265f, -180f);
        mainText.text = "Oh... Your under attack, I wonder why? Lets fight back with some arrows of our own. Press the right button or click your base.";
        buttonPressed = false;
        selectManager.arrowCooldown = false;
        selectManager.healCooldown = false;
        while (!buttonPressed)
        {
            if (playerBase.health < 950)
            {
                playerBase.defence = 100;
            }
            yield return new WaitForFixedUpdate();
        }
        if (playerBase.health > 989)
        {
            playerBase.Hit(25);
        }
        playerBase.defence = 100;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMouseBaseSingle = false;
        tutorialInputManager.allowLeft = true;
        tutorialInputManager.allowMouseBaseHold = true;
        arrow.anchoredPosition = new Vector2(-340f, -180f);
        mainText.text = "Those arrows don't always garentee a kill and they require a cooldown so use them as a last ditch effort. Either way, you took some damage, so heal yourself by pressing the left button or holding down on the base.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMouseBaseHold = false;
        arrow.gameObject.SetActive(false);
        continueButton.SetActive(true);
        mainText.text = "Healing also heals all troups, but it still requires a cooldown. There is still more, but thats all you need to learn for the 1st level\n\nTo Continue, Press All 3 Buttons Or Click The Bottom";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Reset
        playerBase.timeSurvived = 1f;
        inputManager.allowResume = false;
        inputManager.Resume();
    }

    IEnumerator SpecialTutorial()
    {
        //Status
        yield return new WaitUntil(() => (allThreePressed));
    }

    /*IEnumerator Tutorial()
    {
        //Status
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrowBasisObject.gameObject.SetActive(true);
        mainText.text = "This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow2.gameObject.SetActive(false);
        arrow1.anchoredPosition = new Vector2(-25, -100f);
        mainText.text = "This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(75, -100f);
        mainText.text = "This shows your bones, the currency of this game.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(340f, -100f);
        mainText.text = "This shows the amount of active troops over the max number of troops you can have active at one time.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));

        //Controls 1
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(265f, -165f);
        arrow1.anchoredPosition = new Vector2(190f, -180f);
        arrow2.gameObject.SetActive(true);
        arrow2.anchoredPosition = new Vector2(340f, -180f);
        selectManager.boneCostObject0.SetActive(true);
        holdButtons.SetActive(true);
        tutorialInputManager.enabled = true;
        mainText.text = "These buttons are activated when held down. The middle button upgrades the max troop capacity for the price at the top.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.anchoredPosition = new Vector2(340f, -180f);
        arrow2.anchoredPosition = new Vector2(265f, -165f);
        mainText.text = "Try moving the camera by holding the right button";//This can be skipped if the player moves the camera
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 3.5f));
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-190f, -180f);
        arrow2.anchoredPosition = new Vector2(-340f, -180f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        mainText.text = "These buttons are activated when pressed once. For now, select a corpse by pressing the middle button.";
        yield return new WaitUntil(() => (selectManager.selectingObject));

        //Corpse
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(-190f, -180f);
        arrow1.anchoredPosition = new Vector2(-265f, -165f);
        arrow4.gameObject.SetActive(true);
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
        arrow1.gameObject.SetActive(false);
        arrow4.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(190f, -180f);
        holdButtons.SetActive(true);
        mainText.text = "Lets create a minion to gather bones. Hold the left button to create one from a corpse.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        arrow.anchoredPosition = new Vector2(265f, -165f);
        mainText.text = "Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button to spawn some.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));

        //Minion
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition = new Vector2(-265, -165f);
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
        arrow.anchoredPosition = new Vector2(75f, -165f);
        mainText.text = "You can press buttons twice to select these options. We could use some skeletons to attack for us, so hit the right button twice to change all corpses into skeletons.";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));

        //Skeleton
        tutorialInputManager.allowDouble = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowMiddle = true;
        arrow.anchoredPosition = new Vector2(265f, -165f);
        arrow1.gameObject.SetActive(true);
        arrow1.anchoredPosition = new Vector2(190f, -180f);
        arrow2.gameObject.SetActive(true);
        arrow2.anchoredPosition = new Vector2(340f, -180f);
        mainText.text = "Skeletons can be upgraded by spending bones (using the amount indicated above). If you want the skeleton to die for whatever reason, you can do so holding the middle button.\nKILL THE NEWBORN...";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-75f, -165f);
        arrow2.anchoredPosition = new Vector2(0f, -180f);
        arrow3.gameObject.SetActive(true);
        arrow3.anchoredPosition = new Vector2(75f, -165f);
        mainText.text = "Skeletons have three modes. Retreat make them return home ignoring enemies. Stay makes them stay in place, attacking enemies that get near. Attack makes them charge to the right, attacking any enemies. Deselect the skeleton by pressing the middle button";
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMiddle = false;
        arrow.gameObject.SetActive(false);
        mainText.text = "You can control the mode all your skeletons are in when deselected.\n\nPress All 3 Buttons To Continue";
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow3.gameObject.SetActive(false);
        mainText.text = "This marks the end of the tutorial. In the real game, press all 3 buttons to pause the game.\n\nPress All 3 Buttons To Stay And Test Out The Controls\nHold All 3 Buttons To Return To Main Menu Any Time";
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
        textBox.SetActive(false);
    }*/
}
