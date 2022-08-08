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
    //public GameObject corpsePrefab;
    public GameObject textBox;
    public GameObject loading;
    public GameObject tutorialObjectsLevel1;
    public GameObject tutorialObjectsLevel1Part2;
    public GameObject tutorialObjectsLevel2;
    public GameObject tutorialObjectsLevel2Part2;
    public GameObject tutorialObjectsLevel2Part3;
    public GameObject tutorialObjectsSpecialLevel;

    public TutorialInputManager tutorialInputManager;
    public InputManager inputManager;
    public SelectManager selectManager;

    public bool buttonPressed = false;
    private bool waitDueToPause = false;
    private bool allThreePressed = false;
    private bool clickedContinue = false;
    private float timer = 0f;
    private float arrowTimer = 0f;
    private Coroutine slowTextCoroutine;

    public bool isTutorialScene = false;
    public GameObject controlsMenu;
    public GameObject startTutorialMenu;
    public GameObject skeletonContainObject;
    private float buttonPressTimer = 0f;
    private bool buttonPressedStart = false;
    private float buttonPressTime = .4f;
    private ButtonPressed buttonType;

    // Update is called once per frame
    void Update()
    {
        if (controlsMenu != null && controlsMenu.activeSelf)
        {
            if (Input.GetKeyDown(inputManager.middleButton) && !Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.rightButton))
            {
                controlsMenu.SetActive(false);
            }
        }
        else if (startTutorialMenu != null && startTutorialMenu.activeSelf)
        {
            if (buttonPressedStart)
            {
                buttonPressTimer += Time.deltaTime;
            }
            if (Input.GetKeyDown(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton))
            {
                if (!buttonPressedStart)
                {
                    buttonPressedStart = true;
                    buttonType = ButtonPressed.left;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
            }
            else if (Input.GetKeyDown(inputManager.middleButton) && !Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.rightButton))
            {
                if (!buttonPressedStart)
                {
                    buttonPressedStart = true;
                    buttonType = ButtonPressed.middle;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
            }
            else if (Input.GetKeyDown(inputManager.rightButton) && !Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton))
            {
                if (!buttonPressedStart)
                {
                    buttonPressedStart = true;
                    buttonType = ButtonPressed.right;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    buttonPressTimer = 0f;
                    buttonPressedStart = false;
                }
            }
            else if (buttonPressTimer >= buttonPressTime)
            {
                buttonPressTimer = 0f;
                buttonPressedStart = false;
                if (!(Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.middleButton)) && !(Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.rightButton)) && !(Input.GetKey(inputManager.middleButton) && Input.GetKey(inputManager.rightButton)))
                {
                    if (Input.GetKey(inputManager.leftButton))
                    {
                        LeftButtonHold();
                    }
                    else if (buttonType == ButtonPressed.left)
                    {
                        LeftButton();
                    }
                    else if (Input.GetKey(inputManager.middleButton))
                    {
                        MiddleButtonHold();
                    }
                    else if (buttonType == ButtonPressed.middle)
                    {
                        MiddleButton();
                    }
                    else if (Input.GetKey(inputManager.rightButton))
                    {
                        RightButtonHold();
                    }
                    else if (buttonType == ButtonPressed.right)
                    {
                        RightButton();
                    }
                }
            }
        }
        else
        {
            arrowTimer += Time.deltaTime;
            if (arrowTimer > Mathf.PI)
            {
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
    }

    public void LeftButton()
    {
        controlsMenu.SetActive(true);
    }

    public void MiddleButton()
    {
        inputManager.enabled = true;
        inputManager.MainMenu();
    }

    public void RightButton()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ControlsMenuClose()
    {
        controlsMenu.SetActive(false);
    }

    public void LeftButtonHold()
    {
        startTutorialMenu.SetActive(false);
        skeletonContainObject.SetActive(false);
        textBox.SetActive(true);
        tutorialInputManager.enabled = true;
        inputManager.holdInputWait = true;
        StartCoroutine(Level1Tutorial());
    }

    public void MiddleButtonHold()
    {
        startTutorialMenu.SetActive(false);
        textBox.SetActive(true);
        tutorialInputManager.enabled = true;
        inputManager.holdInputWait = true;
        StartCoroutine(Level2Tutorial());
    }

    public void RightButtonHold()
    {
        startTutorialMenu.SetActive(false);
        textBox.SetActive(true);
        tutorialInputManager.enabled = true;
        inputManager.holdInputWait = true;
        StartCoroutine(SpecialTutorial());
    }

    public void StartTutorial()
    {
        textBox.SetActive(true);
        tutorialInputManager.enabled = true;
        this.enabled = true;
        inputManager.holdInputWait = true;
        LevelManager levelManager = GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>();
        if (levelManager.altLevel)
        {
            if (levelManager.level == 1)
            {
                selectManager.specialGoblin = true;
            }
            else if (levelManager.level == 2)
            {
                selectManager.specialWolf = true;
            }
            else if (levelManager.level == 3)
            {
                selectManager.specialWitch = true;
            }
            else if (levelManager.level == 4)
            {
                selectManager.specialOrc = true;
            }
            else /*if (levelManager.level == 5)*/
            {
                selectManager.specialOgre = true;
            }
            StartCoroutine(SpecialTutorial());
        }
        else if (levelManager.level == 1)
        {
            StartCoroutine(Level1Tutorial());
        }
        else /*if (GameObject.FindGameObjectWithTag("Level Manager").GetComponent<LevelManager>().level == 2)*/
        {
            StartCoroutine(Level2Tutorial());
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
        selectManager.boneCostObject0.SetActive(false);
        singleButtons.SetActive(false);
        doubleButtons.SetActive(false);
        holdButtons.SetActive(false);
        selectManager.minionStatus.gameObject.SetActive(false);
        slowTextCoroutine = StartCoroutine(SlowText("First of all, thanks for playing.\nThis game can be played with a mouse or keyboard, but it is recomended to play with both.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("You can pause by holding the 3 buttons, pressing Esc, or the invisible pause button on the top left corner.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));


        //Status
        arrowBasisObject.gameObject.SetActive(true);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow2.gameObject.SetActive(false);
        arrow1.anchoredPosition = new Vector2(-25f, -100f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(75f, -100f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("This shows your bones, the currency of this game.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(340f, -100f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("This shows the amount of active troops over the max number of troops you can have active at one time.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("These buttons are activated when held down. The middle button upgrades the max troop capacity for the price above. Upgrades can also be clicked on.\n\nUpgrade Your Troop Capasity"));
        playerBase.UpdateBones(50);
        yield return new WaitUntil(() => (playerBase.maxSkeletons == 2));
        arrow.anchoredPosition = new Vector2(340f, -180f);
        arrow2.anchoredPosition = new Vector2(265f, -165f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Try moving the camera by holding the right button or holding the mouse down and moving right"));//This can be skipped if the player moves the camera beforehand
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 3.5f));
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-190f, -180f);
        arrow2.anchoredPosition = new Vector2(-340f, -180f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowMouseSelect = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("These buttons are activated when pressed once. For now, select a corpse by pressing the middle button or clicking on one."));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Lets create a minion to gather some bones. Hold the left button to create one from a corpse or click the button to create one."));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Oops... looks like the minion needs some tombstones first. While selecting something, you can switch what you are selecting by pressing the left or right button or clicking on the other corpse.\nSelect The Other Corpse To Continue"));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button or click it to spawn some."));
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Grave").Length != 0));


        //Minion
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().speedAcceleration = 2f;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("You can also upgrade a minion's shovel, which upgrades the amount of bones they mine and their attack power. Lets try to upgrade it by holding the middle button or clicking on it."));
        playerBase.UpdateBones(25);
        yield return new WaitUntil(() => (GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().diggingAttack.attackPower != 6));
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseSelectSingle = true;
        arrow1.gameObject.SetActive(true);
        arrow1.anchoredPosition = new Vector2(190f, -100f);
        arrow.anchoredPosition = new Vector2(340f, -180f);
        selectManager.minionStatus.gameObject.SetActive(true);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Minions have 2 modes, attack and dig, which are indicated above. Curently, the minion is in dig mode, hold the right button to change them into attack mode or click on the minion (clicking changes the minion back to dig mode too)."));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("We're done with the minion for now, so lets deselect it.\nTo deselect objects, press the middle button once or hold the mouse on the selected object."));
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Lets see how a single minion does ageinst a goblin..."));
        yield return new WaitForFixedUpdate();
        GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().inDiggingMode = false;
        yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Minion").Length == 0));
        yield return new WaitForSeconds(.5f);


        //Ending
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMouseBaseSingle = true;
        arrow.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(-190f, -180f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Oh... Your under attack, I wonder why? Lets fight back with some arrows of our own. Press the right button or click your base."));
        buttonPressed = false;
        selectManager.arrowCooldown = false;
        selectManager.healCooldown = false;
        while (!buttonPressed)
        {
            if (playerBase.health < 1000)
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
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Those arrows don't always garentee a kill and they require a cooldown so use them as a last ditch effort. Either way, you took some damage, so heal yourself by pressing the left button or holding down on the base."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMouseBaseHold = false;
        arrow.gameObject.SetActive(false);
        continueButton.SetActive(true);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Healing also heals all troups, but it still requires a cooldown. There is still more, but thats all you need to learn for the 1st level\n\nTo End, Press All 3 Buttons Or Click The Bottom"));
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
        continueButton.SetActive(false);
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
        slowTextCoroutine = StartCoroutine(SlowText("Now that you defeated the goblin chief, you can summon goblin skeletons from goblin corpses. Lets create a skeleton by holding down the right button or clicking on it."));
        yield return new WaitUntil(() => (playerBase.numSkeletons != 0));
        arrow4.gameObject.SetActive(true);
        arrow4.anchoredPosition = new Vector2(190f, -180f);
        playerBase.UpdateBones(25);
        tutorialInputManager.allowLeft = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("You can upgrade both a skeleton's attack and defence 3 times each. Why don't you upgrade one by holding the left or right button clicking on it."));
        yield return new WaitUntil(() => (selectManager.selectedTroop.GetComponent<Skeleton>().attack.attackPower != 75 || selectManager.selectedTroop.GetComponent<Skeleton>().defence != 4));


        //Skeleton
        arrow1.gameObject.SetActive(true);
        arrow2.gameObject.SetActive(true);
        arrow3.gameObject.SetActive(true);
        arrow4.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(-75f, -165f);
        arrow1.anchoredPosition = new Vector2(0f, -180f);
        arrow2.anchoredPosition = new Vector2(75f, -165f);
        arrow3.anchoredPosition = new Vector2(150f, -100f);
        doubleButtons.SetActive(true);
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowHold = false;
        tutorialInputManager.allowDouble = true;
        tutorialInputManager.allowMouseSelectSingle = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Skeletons have 3 modes, which are indicated above. Curently the skeleton is in stay mode, lets change it to retreat mode. To press these buttons, press them twice quickly, so press the left button twice. If your using a mouse, click the skeleton while in stay or attack mode."));
        yield return new WaitUntil(() => (selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode == SkeletonMode.left));
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowRight = true;
        arrow3.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(75f, -165f);
        arrow2.anchoredPosition = new Vector2(-75f, -165f);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("In retreat mode, the skeleton runs toward the base ignoring enemies. Lets change it to attack mode where it runs right and stops to fight nearby enemies. Press the right button twice or click the enemy while in retreat mode to change the skeleton to attack mode."));
        yield return new WaitUntil(() => (selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode == SkeletonMode.right));
        arrow.anchoredPosition = new Vector2(0f, -180f);
        arrow1.anchoredPosition = new Vector2(75f, -165f);
        tutorialInputManager.allowMouseSelectSingle = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowMousePressStayTarget = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Lets change the skeleton back to stay mode where the skeleton stays near a specified spot, indicated by a yellow marker, and attacks nearby enemies. To do it, press the middle button twice, or if your using mouse, click where you want the skeleton to stay."));
        yield return new WaitUntil(() => (selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode == SkeletonMode.stay));
        yield return new WaitForSeconds(.5f);


        //Minion
        selectManager.selectableObjects.Remove(GameObject.FindGameObjectWithTag("Skeleton").transform);
        Destroy(GameObject.FindGameObjectWithTag("Skeleton"));
        playerBase.numSkeletons--;
        selectManager.troopCapacityText.text = "0\n" + playerBase.maxSkeletons.ToString();
        tutorialObjectsLevel2Part2.SetActive(true);
        arrow4.gameObject.SetActive(true);
        arrow.anchoredPosition = new Vector2(-75f, -165f);
        arrow2.anchoredPosition = new Vector2(0f, -180f);
        arrow4.anchoredPosition = new Vector2(190f, -165f);
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowMousePressStayTarget = false;
        tutorialInputManager.allowLeft = true;
        selectManager.Select(GameObject.FindGameObjectWithTag("Corpse").transform);
        selectManager.corpseSkeletonButton.SetActive(false);
        selectManager.corpseTombstoneButton.SetActive(false);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("While you have a corpse selected, you can press the middle button twice to make all of them turn into minions. If your using a mouse, you can't do that, so, for now, just make a single minion instead."));
        yield return new WaitUntil(() => (playerBase.numSkeletons != 0));
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGraves(5);
        selectManager.selectedTroop.GetComponent<Minion>().bonesStored = 6;
        arrow4.gameObject.SetActive(false);
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(0f, -180f);
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowMouseSelectDouble = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Minions get slightly weighed down if they carry bones, and when they are full, they have to go all the way back to the base to collect them. However, you can collect them yourself by pressing the middle button twice or double clicking the minion."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < minions.Length; i++)
        {
            minions[i].GetComponent<Minion>().defence = 100;
        }
        tutorialObjectsLevel2Part3.SetActive(true);
        arrow.anchoredPosition = new Vector2(75f, -180f);
        tutorialInputManager.allowMouseSelectDouble = false;
        tutorialInputManager.allowMousePressEnemies = true;
        tutorialInputManager.allowMiddle = false;
        tutorialInputManager.allowRight = true;
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("On keyboard, you can command all minions into one mode by pressing buttons twice. Alternitively, on mouse, you can select a target for the curently selected minion or skeleton (for minions, graves too). Either set all minions to attack mode or click the goblin to target it."));
        yield return new WaitUntil(() => (!selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode));
        yield return new WaitForSeconds(.5f);
        //yield return new WaitUntil(() => (GameObject.FindGameObjectsWithTag("Enemy").Length == 0));


        //End
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowLeft = true;
        tutorialInputManager.allowMiddle = true;
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowHoldMovement = true;
        tutorialInputManager.allowMouseSelect = true;
        tutorialInputManager.allowMouseSelectSingle = true;
        tutorialInputManager.allowMouseSelectDouble = true;
        tutorialInputManager.allowMouseSelectHold = true;
        tutorialInputManager.allowMouseBaseSingle = true;
        tutorialInputManager.allowMouseBaseHold = true;
        tutorialInputManager.allowMousePressGraves = true;
        tutorialInputManager.allowMousePressStayTarget = true;
        tutorialInputManager.allowMouseMovement = true;
        inputManager.holdInputWait = false;
        arrow1.gameObject.SetActive(true);//right
        arrow2.gameObject.SetActive(true);//middle
        arrow3.gameObject.SetActive(true);
        arrow4.gameObject.SetActive(true);
        arrow4.GetComponent<UnityEngine.UI.Image>().color = arrow1.GetComponent<UnityEngine.UI.Image>().color;
        arrow.GetComponent<UnityEngine.UI.Image>().color = arrow1.GetComponent<UnityEngine.UI.Image>().color;
        arrow.anchoredPosition = new Vector2(-75f, -180f);
        arrow4.anchoredPosition = new Vector2(190, -100f);
        continueButton.SetActive(true);
        selectManager.Deselect();
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("Without anything selected, you can change all of your skeletons (not minions) modes using the double press buttons. You can see what the current default modes are set to above.\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.gameObject.SetActive(false);
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow3.gameObject.SetActive(false);
        arrow4.gameObject.SetActive(false);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("You have reached the end. To sum up the controls for keyboard, Single buttons = select, Double buttons = modes/affects all, Hold buttons = upgrade.\n\nTo Continue, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("To sum up the controls for mouse, Click things to select/target them, click top right to upgrade, click while selected to change modes.\n\nTo End, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));

        //Reset
        playerBase.timeSurvived = 1f;
        inputManager.allowResume = false;
        inputManager.Resume();
    }

    IEnumerator SpecialTutorial()
    {
        //Start
        if (selectManager.selectedTroop)
        {
            selectManager.Deselect();
        }
        while (selectManager.selectableObjects.Count != 0)
        {
            GameObject.Destroy(selectManager.selectableObjects[0].gameObject);
            selectManager.selectableObjects.RemoveAt(0);
        }
        tutorialObjectsSpecialLevel.SetActive(true);
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        playerBase.numSkeletons = 0;
        selectManager.troopCapacityText.text = "0\n" + playerBase.maxSkeletons;
        selectManager.Select(GameObject.FindGameObjectWithTag("Skeleton").transform);
        arrowBasisObject.gameObject.SetActive(true);
        arrow.gameObject.SetActive(true);
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(265f, -165f);
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowRight = false;
        tutorialInputManager.allowHoldMovement = false;
        tutorialInputManager.allowMouseSelectDouble = true;
        selectManager.attackUpgradeButton.SetActive(false);
        selectManager.defenceUpgradeButton.SetActive(false);
        slowTextCoroutine = StartCoroutine(SlowText("After defeating a special challenge for the level, you unlock a special ability for the main enemy. Lets try it out by holding down the middle button or clicking the skeleton twice."));
        yield return new WaitUntil(() => (buttonPressed));
        arrow.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        continueButton.SetActive(true);
        StopCoroutine(slowTextCoroutine);
        slowTextCoroutine = StartCoroutine(SlowText("As you can see special abilities have a cooldown, but the cooldown only aplies to that skeleton, so multiple skelton's abilities can be activated at once.\n\nTo End, Press All 3 Buttons Or Click The Bottom"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));

        //Reset
        if (isTutorialScene)
        {
            inputManager.allowResume = false;
            inputManager.Resume();
        }
        else
        {
            playerBase.timeSurvived = 1f;
            inputManager.enabled = true;
            inputManager.MainMenu();
        }
    }

    IEnumerator SlowText(string text)
    {

        mainText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            yield return new WaitForSeconds(.025f);
            mainText.text += text[i];
        }
        
    }

    /*IEnumerator Tutorial()
    {
        //Status
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrowBasisObject.gameObject.SetActive(true);
        slowTextCoroutine = StartCoroutine(SlowText("This is the health bar of whatever you have selected. Next to it are the numbers that show the current health over the max health.\n\nPress All 3 Buttons To Continue"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow2.gameObject.SetActive(false);
        arrow1.anchoredPosition = new Vector2(-25, -100f);
        slowTextCoroutine = StartCoroutine(SlowText("This shows the name of the selected object. Whenever nothing is selected, it shows your base's health, just make sure its doesn't fall to zero, or you lose!\n\nPress All 3 Buttons To Continue"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(75, -100f);
        slowTextCoroutine = StartCoroutine(SlowText("This shows your bones, the currency of this game.\n\nPress All 3 Buttons To Continue"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.anchoredPosition = new Vector2(340f, -100f);
        slowTextCoroutine = StartCoroutine(SlowText("This shows the amount of active troops over the max number of troops you can have active at one time.\n\nPress All 3 Buttons To Continue"));
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
        slowTextCoroutine = StartCoroutine(SlowText("These buttons are activated when held down. The middle button upgrades the max troop capacity for the price at the top.\n\nPress All 3 Buttons To Continue"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow.anchoredPosition = new Vector2(340f, -180f);
        arrow2.anchoredPosition = new Vector2(265f, -165f);
        slowTextCoroutine = StartCoroutine(SlowText("Try moving the camera by holding the right button"));//This can be skipped if the player moves the camera
        yield return new WaitUntil(() => (inputManager.selectManager.transform.position.x > 3.5f));
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-190f, -180f);
        arrow2.anchoredPosition = new Vector2(-340f, -180f);
        singleButtons.SetActive(true);
        tutorialInputManager.allowSingle = true;
        slowTextCoroutine = StartCoroutine(SlowText("These buttons are activated when pressed once. For now, select a corpse by pressing the middle button."));
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
        slowTextCoroutine = StartCoroutine(SlowText("While selecting something, you can switch what you are selecting by pressing the left or right button. Select the other corpse."));
        Transform firstSelectedCorpse = selectManager.selectedTroop;//Since waituntil activates after Update() is called, we don't have to worry about the selected object being assigned after selectingObject is set to true
        yield return new WaitUntil(() => (selectManager.selectedTroop != firstSelectedCorpse));
        tutorialInputManager.allowHold = true;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowRight = false;
        arrow1.gameObject.SetActive(false);
        arrow4.gameObject.SetActive(false);
        arrow.anchoredPosition = new Vector2(190f, -180f);
        holdButtons.SetActive(true);
        slowTextCoroutine = StartCoroutine(SlowText("Lets create a minion to gather bones. Hold the left button to create one from a corpse."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowLeft = false;
        tutorialInputManager.allowMiddle = true;
        arrow.anchoredPosition = new Vector2(265f, -165f);
        slowTextCoroutine = StartCoroutine(SlowText("Minions slowly dig bones from graves, but currently, there isn't any. Lets spawn some using a corpse. Hold the middle button to spawn some."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));

        //Minion
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition = new Vector2(-265, -165f);
        Instantiate(corpsePrefab, new Vector3(3f, 0, 0f), corpsePrefab.transform.rotation);
        Instantiate(corpsePrefab, new Vector3(7f, -.5f, 0f), corpsePrefab.transform.rotation);
        slowTextCoroutine = StartCoroutine(SlowText("Minions have 2 modes, attack and dig. Minions can't be selected individually, and all follow the same command, which is set by pressing left or right when nothing is selected. I spawned more corpses for you, select one when you are ready."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        doubleButtons.SetActive(true);
        tutorialInputManager.allowDouble = true;
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowRight = true;
        tutorialInputManager.allowMiddle = false;
        arrow.anchoredPosition = new Vector2(75f, -165f);
        slowTextCoroutine = StartCoroutine(SlowText("You can press buttons twice to select these options. We could use some skeletons to attack for us, so hit the right button twice to change all corpses into skeletons."));
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
        slowTextCoroutine = StartCoroutine(SlowText("Skeletons can be upgraded by spending bones (using the amount indicated above). If you want the skeleton to die for whatever reason, you can do so holding the middle button.\nKILL THE NEWBORN..."));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = true;
        tutorialInputManager.allowHold = false;
        arrow.anchoredPosition = new Vector2(-265f, -165f);
        arrow1.anchoredPosition = new Vector2(-75f, -165f);
        arrow2.anchoredPosition = new Vector2(0f, -180f);
        arrow3.gameObject.SetActive(true);
        arrow3.anchoredPosition = new Vector2(75f, -165f);
        slowTextCoroutine = StartCoroutine(SlowText("Skeletons have three modes. Retreat make them return home ignoring enemies. Stay makes them stay in place, attacking enemies that get near. Attack makes them charge to the right, attacking any enemies. Deselect the skeleton by pressing the middle button"));
        buttonPressed = false;
        yield return new WaitUntil(() => (buttonPressed));
        tutorialInputManager.allowSingle = false;
        tutorialInputManager.allowMiddle = false;
        arrow.gameObject.SetActive(false);
        slowTextCoroutine = StartCoroutine(SlowText("You can control the mode all your skeletons are in when deselected.\n\nPress All 3 Buttons To Continue"));
        yield return new WaitUntil(() => (allThreePressed));
        yield return new WaitUntil(() => (!allThreePressed));
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
        arrow3.gameObject.SetActive(false);
        slowTextCoroutine = StartCoroutine(SlowText("This marks the end of the tutorial. In the real game, press all 3 buttons to pause the game.\n\nPress All 3 Buttons To Stay And Test Out The Controls\nHold All 3 Buttons To Return To Main Menu Any Time"));
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
