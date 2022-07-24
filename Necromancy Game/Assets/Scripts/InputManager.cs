using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum ButtonPressed
{
    left = 0,
    middle = 1,
    right = 2
}

public class InputManager : MonoBehaviour
{
    public KeyCode leftButton = KeyCode.A;
    public KeyCode middleButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    private float buttonPressTimer = 0f;
    public bool buttonPressed = false;
    private float buttonPressTime = .4f;
    private ButtonPressed buttonType;

    public GameObject impossibleActionPrefab;
    public PlayerBase playerBase;
    public SelectManager selectManager;
    public Image[] buttonImages;
    public bool holdInputWait = false;
    public bool holdRight = false;
    public bool holdLeft = false;

    private bool allowInputs = true;
    public bool paused = false;
    public bool allowResume = true;
    public GameObject pauseMenu;
    public GameObject loading;
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI leftButtonText;
    public TextMeshProUGUI middleButtonText;
    public TextMeshProUGUI rightButtonText;

    public Image leftButtonImage;
    public Image middleButtonImage;
    public Image rightButtonImage;
    public Image holdLeftButtonImage;
    public Image holdMiddleButtonImage;
    public Image holdRightButtonImage;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            List<GameObject> hitObjects = new List<GameObject>();
            while (hit.collider != null)
            {
                hitObjects.Add(hit.collider.gameObject);
                hit.collider.gameObject.layer = 2;
                hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            }
            if (hitObjects.Count != 0)
            {
                int lowestYIndex = 0;
                for (int i = 0; i < hitObjects.Count; i++)
                {
                    hitObjects[i].layer = 0;
                    if (hitObjects[lowestYIndex].transform.position.y > hitObjects[i].transform.position.y)
                    {
                        lowestYIndex = i;
                    }
                }
                if (hitObjects[lowestYIndex].CompareTag("Skeleton Select"))
                {
                    if (selectManager.selectedTroop == hitObjects[lowestYIndex].transform.parent)
                    {
                        if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton"))
                        {
                            if (selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode != SkeletonMode.left)
                            {
                                selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
                            }
                            else
                            {
                                selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                            }
                        }
                        else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Minion"))
                        {
                            if (selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode)
                            {
                                selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = false;
                                if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                                {
                                    selectManager.selectedTroop.GetComponent<Minion>().grave.targetSelect.SetActive(false);
                                }
                            }
                            else
                            {
                                selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = true;
                                if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                                {
                                    selectManager.selectedTroop.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                                }
                            }
                        }
                    }
                    else
                    {
                        selectManager.Select(hitObjects[lowestYIndex].transform.parent);
                    }
                }
                else if (hitObjects[lowestYIndex].CompareTag("Enemy Select"))
                {
                    if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton"))
                    {
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                        if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                        }
                        hitObjects[lowestYIndex].transform.parent.GetComponent<Enemy>().targetSelect.SetActive(true);
                        selectManager.selectedTroop.GetComponent<Skeleton>().goal = hitObjects[lowestYIndex].transform.parent;
                    }
                    else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Minion"))
                    {
                        if (selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode)
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = false;
                            if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                            {
                                selectManager.selectedTroop.GetComponent<Minion>().grave.targetSelect.SetActive(false);
                            }
                        }
                        else if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                        }
                        selectManager.selectedTroop.GetComponent<Minion>().goal = hitObjects[lowestYIndex].transform.parent;
                        hitObjects[lowestYIndex].transform.parent.GetComponent<Enemy>().targetSelect.SetActive(true);
                    }
                }
                else if (hitObjects[lowestYIndex].CompareTag("Grave Select"))
                {
                    if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton"))
                    {
                        if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal = null;
                        }
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
                        selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal = camera.ScreenToWorldPoint(Input.mousePosition);
                    }
                    else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Minion"))
                    {
                        if (selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode)
                        {
                            if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                            {
                                selectManager.selectedTroop.GetComponent<Minion>().grave.targetSelect.SetActive(false);
                            }
                        }
                        else if (selectManager.selectedTroop.GetComponent<Minion>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = true;
                            selectManager.selectedTroop.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                        }
                        selectManager.selectedTroop.GetComponent<Minion>().goal = hitObjects[lowestYIndex].transform.parent;
                        selectManager.selectedTroop.GetComponent<Minion>().grave = hitObjects[lowestYIndex].transform.parent.GetComponent<Grave>();
                        hitObjects[lowestYIndex].transform.parent.GetComponent<Grave>().targetSelect.SetActive(true);
                    }
                }
            }
            else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton"))
            {
                if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                {
                    selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                    selectManager.selectedTroop.GetComponent<Skeleton>().goal = null;
                }
                selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
                selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        //Paused Inputs
        if (paused && allowInputs)
        {
            if (buttonPressed)
            {
                buttonPressTimer += Time.unscaledDeltaTime;
            }
            if (Input.GetKey(leftButton) && Input.GetKey(middleButton) && Input.GetKey(rightButton) && (Input.GetKeyDown(leftButton) || Input.GetKeyDown(middleButton) || Input.GetKeyDown(rightButton)))
            {
                Resume();
            }
            if (Input.GetKeyDown(leftButton) && !Input.GetKey(middleButton) && !Input.GetKey(rightButton))
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    buttonType = ButtonPressed.left;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
            }
            else if (Input.GetKeyDown(middleButton) && !Input.GetKey(leftButton) && !Input.GetKey(rightButton))
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    buttonType = ButtonPressed.middle;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
            }
            else if (Input.GetKeyDown(rightButton) && !Input.GetKey(leftButton) && !Input.GetKey(middleButton))
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    buttonType = ButtonPressed.right;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
            }
            else if (buttonPressTimer >= buttonPressTime)
            {
                buttonPressTimer = 0f;
                buttonPressed = false;
                if (!(Input.GetKey(leftButton) && Input.GetKey(middleButton)) && !(Input.GetKey(leftButton) && Input.GetKey(rightButton)) && !(Input.GetKey(middleButton) && Input.GetKey(rightButton)))
                {
                    if (Input.GetKey(leftButton))
                    {
                        StartCoroutine(PauseHoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.left)
                    {
                        StartCoroutine(PauseSinglePress(buttonType));
                    }
                    else if (Input.GetKey(middleButton))
                    {
                        StartCoroutine(PauseHoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.middle)
                    {
                        StartCoroutine(PauseSinglePress(buttonType));
                    }
                    else if (Input.GetKey(rightButton))
                    {
                        StartCoroutine(PauseHoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.right)
                    {
                        StartCoroutine(PauseSinglePress(buttonType));
                    }
                }
            }
        }

        //Normal Inputs
        else if (allowInputs)
        {
            if (buttonPressed)
            {
                buttonPressTimer += Time.deltaTime;
            }
            if (Input.GetKey(leftButton) && Input.GetKey(middleButton) && Input.GetKey(rightButton) && (Input.GetKeyDown(leftButton) || Input.GetKeyDown(middleButton) || Input.GetKeyDown(rightButton)))
            {
                Pause();
            }
            else if (Input.GetKeyDown(leftButton) && !Input.GetKey(middleButton) && !Input.GetKey(rightButton))
            {
                if (!buttonPressed)
                {
                    holdInputWait = true;
                    buttonPressed = true;
                    buttonType = ButtonPressed.left;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    //Debug.Log("Double Left");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    //Debug.Log("Single Middle + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.left;
                }
                else /*if (buttonType == ButtonPressed.right)*/
                {
                    //Debug.Log("Single Right + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.left;
                }
            }
            else if (Input.GetKeyDown(middleButton) && !Input.GetKey(leftButton) && !Input.GetKey(rightButton))
            {
                if (!buttonPressed)
                {
                    holdInputWait = true;
                    buttonPressed = true;
                    buttonType = ButtonPressed.middle;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    //Debug.Log("Double Middle");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    //Debug.Log("Single Left + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.middle;
                }
                else /*if (buttonType == ButtonPressed.right)*/
                {
                    //Debug.Log("Single Right + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.middle;
                }
            }
            else if (Input.GetKeyDown(rightButton) && !Input.GetKey(leftButton) && !Input.GetKey(middleButton))
            {
                if (!buttonPressed)
                {
                    holdInputWait = true;
                    buttonPressed = true;
                    buttonType = ButtonPressed.right;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    //Debug.Log("Double Right");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    //Debug.Log("Single Left + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.right;
                }
                else /*if (buttonType == ButtonPressed.middle)*/
                {
                    //Debug.Log("Single Middle + ");
                    SinglePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonType = ButtonPressed.right;
                }
            }
            else if (buttonPressTimer >= buttonPressTime)
            {
                buttonPressTimer = 0f;
                buttonPressed = false;
                holdInputWait = false;
                if (!(Input.GetKey(leftButton) && Input.GetKey(middleButton)) && !(Input.GetKey(leftButton) && Input.GetKey(rightButton)) && !(Input.GetKey(middleButton) && Input.GetKey(rightButton)))
                {
                    if (Input.GetKey(leftButton))
                    {
                        //Debug.Log("Hold Left");
                        HoldPress(buttonType);
                    }
                    else if (buttonType == ButtonPressed.left)
                    {
                        //Debug.Log("Single Left");
                        SinglePress(buttonType);
                    }
                    else if (Input.GetKey(middleButton))
                    {
                        //Debug.Log("Hold Middle");
                        HoldPress(buttonType);
                    }
                    else if (buttonType == ButtonPressed.middle)
                    {
                        //Debug.Log("Single Middle");
                        SinglePress(buttonType);
                    }
                    else if (Input.GetKey(rightButton))
                    {
                        //Debug.Log("Hold Right");
                        HoldPress(buttonType);
                    }
                    else if (buttonType == ButtonPressed.right)
                    {
                        //Debug.Log("Single Right");
                        SinglePress(buttonType);
                    }
                }
            }
        }
    }

    public void Pause()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        holdInputWait = true;
        buttonPressed = false;
        buttonPressTimer = 0f;
        timeSurvivedText.text = "Time Survived: " + ((int)playerBase.timeSurvived) + " Seconds";
        leftButtonText.text = leftButton.ToString();
        middleButtonText.text = middleButton.ToString();
        rightButtonText.text = rightButton.ToString();
    }

    public void Resume()
    {
        if (allowResume)
        {
            paused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            holdInputWait = false;
        }
        else
        {
            StartCoroutine(ReloadScene());
        }
    }

    IEnumerator ReloadScene()
    {
        loading.SetActive(true);
        DontDestroyOnLoad(gameObject);
        AsyncOperation ao = SceneManager.LoadSceneAsync("Endless Mode", LoadSceneMode.Single);
        yield return new WaitUntil(() => ao.isDone);
        GameObject[] inputManagers = GameObject.FindGameObjectsWithTag("Input Manager");
        InputManager otherInputManager = inputManagers[0] == gameObject ? inputManagers[1].GetComponent<InputManager>() : inputManagers[0].GetComponent<InputManager>();
        otherInputManager.leftButton = leftButton;
        otherInputManager.middleButton = middleButton;
        otherInputManager.rightButton = rightButton;
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    IEnumerator PauseSinglePress(ButtonPressed type)
    {
        if (type == ButtonPressed.left)
        {
            Resume();
        }
        else if (type == ButtonPressed.middle)
        {
            loading.SetActive(true);
            AsyncOperation ao = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
            MainMenuManager mainMenuManager = GameObject.FindGameObjectWithTag("Main Menu Manager").GetComponent<MainMenuManager>();
            mainMenuManager.leftButton = leftButton;
            mainMenuManager.middleButton = middleButton;
            mainMenuManager.rightButton = rightButton;
            mainMenuManager.leftButtonText.text = mainMenuManager.leftButton.ToString();
            mainMenuManager.middleButtonText.text = mainMenuManager.middleButton.ToString();
            mainMenuManager.rightButtonText.text = mainMenuManager.rightButton.ToString();
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync("Endless Mode");
        }
        else /*if (type == ButtonPressed.right)*/
        {
            rightButtonImage.color = Color.gray;
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }

    IEnumerator PauseHoldPress(ButtonPressed type)
    {
        if (type == ButtonPressed.left)
        {
            holdLeftButtonImage.color = Color.gray;
            allowInputs = false;
            leftButtonText.text = "Awaiting Input...";
            yield return new WaitUntil(() => !Input.GetKey(leftButton));
            holdLeftButtonImage.color = Color.white;
            KeyCode newKey = KeyCode.None;
            while (newKey == KeyCode.None)
            {
                yield return new WaitUntil(() => !Input.anyKey);
                for (int i = 0; i < 370; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != middleButton && (KeyCode)i != rightButton)
                    {
                        newKey = (KeyCode)i;
                        break;
                    }
                }
            }
            leftButton = newKey;
            leftButtonText.text = newKey.ToString();
            yield return new WaitForEndOfFrame();
            allowInputs = true;
        }
        else if (type == ButtonPressed.middle)
        {
            holdMiddleButtonImage.color = Color.gray;
            allowInputs = false;
            middleButtonText.text = "Awaiting Input...";
            yield return new WaitUntil(() => !Input.GetKey(middleButton));
            holdMiddleButtonImage.color = Color.white;
            KeyCode newKey = KeyCode.None;
            while (newKey == KeyCode.None)
            {
                yield return new WaitUntil(() => !Input.anyKey);
                for (int i = 0; i < 370; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != rightButton)
                    {
                        newKey = (KeyCode)i;
                        break;
                    }
                }
            }
            middleButton = newKey;
            middleButtonText.text = newKey.ToString();
            yield return new WaitForEndOfFrame();
            allowInputs = true;
        }
        else /*if (type == ButtonPressed.right)*/
        {
            holdRightButtonImage.color = Color.gray;
            allowInputs = false;
            rightButtonText.text = "Awaiting Input...";
            yield return new WaitUntil(() => !Input.GetKey(rightButton));
            holdRightButtonImage.color = Color.white;
            KeyCode newKey = KeyCode.None;
            while (newKey == KeyCode.None)
            {
                yield return new WaitUntil(() => !Input.anyKey);
                for (int i = 0; i < 370; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != middleButton)
                    {
                        newKey = (KeyCode)i;
                        break;
                    }
                }
            }
            rightButton = newKey;
            rightButtonText.text = newKey.ToString();
            yield return new WaitForEndOfFrame();
            allowInputs = true;
        }
    }

    private void SinglePress(ButtonPressed type)
    {
        if (selectManager.selectingObject)
        {
            if (type == ButtonPressed.left)
            {
                StartCoroutine(PressButtonVisually(buttonImages[0]));
                selectManager.SelectLeftTroop();
            }
            else if (type == ButtonPressed.middle)
            {
                StartCoroutine(PressButtonVisually(buttonImages[1]));
                selectManager.Deselect();
            }
            else /*if (type == ButtonPressed.right)*/
            {
                StartCoroutine(PressButtonVisually(buttonImages[2]));
                selectManager.SelectRightTroop();
            }
        }
        else if (type == ButtonPressed.left)
        {
            StartCoroutine(PressButtonVisually(buttonImages[0]));
            //selectManager.AllMinionsMine();
        }
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[1]));
            selectManager.SelectNearestTroop();
        }
        else /*if (type == ButtonPressed.right)*/
        {
            StartCoroutine(PressButtonVisually(buttonImages[2]));
            //selectManager.AllMinionsAttack();
        }
    }

    private void DoublePress(ButtonPressed type)
    {
        if (selectManager.selectingObject)
        {
            if (selectManager.selectedTroop.CompareTag("Skeleton"))
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[3]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[4]));
                    selectManager.TroopStay();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[5]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                }
            }
            else if (selectManager.selectedTroop.CompareTag("Minion"))
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[3]));
                    selectManager.AllMinionsMine();
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[4]));
                    //selectManager.TroupStay();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[5]));
                    selectManager.AllMinionsAttack();
                }
            }
            else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[3]));
                    selectManager.AllCorpsesSpawnMinions();
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[4]));
                    selectManager.AllCorpsesSpawnTombstones();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[5]));
                    selectManager.AllCorpsesSpawnSkeletons();
                }
            }
        }
        else if (type == ButtonPressed.left)
        {
            StartCoroutine(PressButtonVisually(buttonImages[3]));
            selectManager.AllTroopsLeft();
        }
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[4]));
            selectManager.AllTroopsStay();
        }
        else /*if (type == ButtonPressed.right)*/
        {
            StartCoroutine(PressButtonVisually(buttonImages[5]));
            selectManager.AllTroopsRight();
        }
    }

    private void HoldPress(ButtonPressed type)
    {
        if (selectManager.selectingObject)
        {
            if (selectManager.selectedTroop.CompareTag("Skeleton"))
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[6]));
                    if (selectManager.selectedTroop.GetComponent<Skeleton>().defenceBoneUpgradeAmount != -1 && playerBase.bones >= selectManager.selectedTroop.GetComponent<Skeleton>().defenceBoneUpgradeAmount)
                    {
                        selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeDefence();
                    }
                    else
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.textPosition.anchoredPosition = new Vector2(190f, 150f);
                    }
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[7]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().TurnIntoTombstone();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[8]));
                    if (selectManager.selectedTroop.GetComponent<Skeleton>().attackBoneUpgradeAmount != -1 && playerBase.bones >= selectManager.selectedTroop.GetComponent<Skeleton>().attackBoneUpgradeAmount)
                    {
                        selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeAttack();
                    }
                    else
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.textPosition.anchoredPosition = new Vector2(340f, 150f);
                    }
                }
            }
            else if (selectManager.selectedTroop.CompareTag("Minion"))
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[6]));
                    selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = true;
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[7]));
                    if (selectManager.selectedTroop.GetComponent<Minion>().boneUpgradeAmount != -1 && playerBase.bones >= selectManager.selectedTroop.GetComponent<Minion>().boneUpgradeAmount)
                    {
                        selectManager.selectedTroop.GetComponent<Minion>().Upgrade();
                    }
                    else
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.textPosition.anchoredPosition = new Vector2(265f, 150f);
                    }
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[8]));
                    selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = false;
                }
            }
            else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[6]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnMinion();
                    if (selectManager.corpseActionFail)
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.text.text = "Max Troops Reached";
                        notice.textPosition.anchoredPosition = new Vector2(75f, 150f);
                        selectManager.corpseActionFail = false;
                    }
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[7]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnTombstones();
                    if (selectManager.corpseActionFail)
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.text.text = "Graveyard Full";
                        notice.textPosition.anchoredPosition = new Vector2(75f, 150f);
                        selectManager.corpseActionFail = false;
                    }
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[8]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnSkeleton();
                    if (selectManager.corpseActionFail)
                    {
                        InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                        notice.text.text = "Max Troops Reached";
                        notice.textPosition.anchoredPosition = new Vector2(75f, 150f);
                        selectManager.corpseActionFail = false;
                    }
                }
            }
        }//Left and right handled inside select manager
        /*else if (type == ButtonPressed.left)
        {
            StartCoroutine(HoldButtonVisually(buttonImages[6], leftButton));
        }*/
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[7]));
            if (playerBase.bones >= playerBase.maxSkeletonUpgradeAmount)
            {
                playerBase.UpgradeMaxSkeletons();
            }
            else
            {
                InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                notice.textPosition.anchoredPosition = new Vector2(265f, 150f);
            }
        }
        /*else /*if (type == ButtonPressed.right)*/
        /*{
            StartCoroutine(HoldButtonVisually(buttonImages[8], rightButton));
        }*/
    }

    IEnumerator SlightWait()
    {
        yield return new WaitForSeconds(buttonPressTime / 2f);
        if (!buttonPressed)
        {
            holdInputWait = false;
        }
    }

    IEnumerator PressButtonVisually(Image buttonImage)
    {
        buttonImage.color = Color.gray;
        yield return new WaitForSeconds(buttonPressTime);
        buttonImage.color = Color.white;
    }

    public void MovingAroundInputChange(Image buttonImage, KeyCode buttonType)
    {
        StartCoroutine(HoldButtonVisually(buttonImage, buttonType));
    }

    IEnumerator HoldButtonVisually(Image buttonImage, KeyCode buttonType)
    {
        buttonImage.color = Color.gray;
        yield return new WaitUntil(() => (Input.GetKeyUp(buttonType) || buttonPressed));
        buttonImage.color = Color.white;
        if (buttonImage == buttonImages[6])
        {
            holdLeft = false;
        }
        else
        {
            holdRight = false;
        }
    }
}
