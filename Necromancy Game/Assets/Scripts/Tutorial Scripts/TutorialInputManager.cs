using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialInputManager : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public InputManager inputManager;
    private float buttonPressTimer = 0f;
    private float buttonPressTime = .4f;
    private ButtonPressed buttonType;

    public GameObject impossibleActionPrefab;
    public PlayerBase playerBase;
    public SelectManager selectManager;
    public Image[] buttonImages;

    public bool allowLeft = true;
    public bool allowMiddle = true;
    public bool allowRight = true;
    public bool allowSingle = false;
    public bool allowDouble = false;
    public bool allowHold = true;
    public bool allowHoldMovement = true;
    public bool allowMouseSelect = false;
    public bool allowMouseSelectSingle = false;
    public bool allowMouseSelectDouble = false;
    public bool allowMouseSelectHold = false;
    public bool allowMouseBaseSingle = false;
    public bool allowMouseBaseHold = false;
    public bool allowMousePressEnemies = false;
    public bool allowMousePressGraves = false;
    public bool allowMousePressStayTarget = false;
    public bool allowMouseMovement = false;

    private float slopeBase = 1.6f;
    private float posYBase = 8f;
    private bool movingCamera = false;
    private bool doingMouseCoroutine = false;
    private float xHoldPosition;
    private Vector3 startingXHoldPosition;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && inputManager.allowInputs && !inputManager.paused && !doingMouseCoroutine && Input.mousePosition.y / Screen.height < .75f)
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            List<GameObject> hitObjects = new List<GameObject>();
            bool hitBase = false;
            while (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player Base"))
                {
                    hitBase = true;
                }
                else
                {
                    hitObjects.Add(hit.collider.gameObject);
                }
                hit.collider.gameObject.layer = 2;
                hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            }
            playerBase.gameObject.layer = 0;
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
                    if (selectManager.selectingObject && selectManager.selectedTroop == hitObjects[lowestYIndex].transform.parent)
                    {
                        StartCoroutine(MousePressDownSkeleton());
                    }
                    else if (allowMouseSelect)
                    {
                        selectManager.Select(hitObjects[lowestYIndex].transform.parent);
                    }
                }
                else if (hitObjects[lowestYIndex].CompareTag("Enemy Select"))
                {
                    if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton") && allowMousePressEnemies)
                    {
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                        selectManager.stayPositionMarker.gameObject.SetActive(false);
                        selectManager.skeletonStatus.sprite = selectManager.skeletonAttack;
                        if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                        }
                        hitObjects[lowestYIndex].transform.parent.GetComponent<Enemy>().targetSelect.SetActive(true);
                        selectManager.selectedTroop.GetComponent<Skeleton>().goal = hitObjects[lowestYIndex].transform.parent;
                    }
                    else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Minion") && allowMousePressEnemies)
                    {
                        if (selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode)
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = false;
                            selectManager.minionStatus.sprite = selectManager.minionAttack;
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
                    else if (!selectManager.selectingObject && allowMouseMovement)
                    {
                        xHoldPosition = selectManager.transform.position.x;
                        startingXHoldPosition = Input.mousePosition;
                        movingCamera = true;
                    }
                }
                else if (hitObjects[lowestYIndex].CompareTag("Grave Select"))
                {
                    if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton") && camera.ScreenToWorldPoint(Input.mousePosition).y < .9f && allowMousePressGraves)
                    {
                        if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                        {
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                            selectManager.selectedTroop.GetComponent<Skeleton>().goal = null;
                        }
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
                        selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
                        selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal = new Vector2(camera.ScreenToWorldPoint(Input.mousePosition).x, camera.ScreenToWorldPoint(Input.mousePosition).y);
                        selectManager.stayPositionMarker.gameObject.SetActive(true);
                        selectManager.stayPositionMarker.position = selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal;
                    }
                    else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Minion") && allowMousePressStayTarget)
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
                            selectManager.minionStatus.sprite = selectManager.minionDig;
                            selectManager.selectedTroop.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                        }
                        selectManager.selectedTroop.GetComponent<Minion>().goal = hitObjects[lowestYIndex].transform.parent;
                        selectManager.selectedTroop.GetComponent<Minion>().grave = hitObjects[lowestYIndex].transform.parent.GetComponent<Grave>();
                        hitObjects[lowestYIndex].transform.parent.GetComponent<Grave>().targetSelect.SetActive(true);
                    }
                    else if (!selectManager.selectingObject && allowMouseMovement)
                    {
                        xHoldPosition = selectManager.transform.position.x;
                        startingXHoldPosition = Input.mousePosition;
                        movingCamera = true;
                    }
                }
            }
            else if (hitBase && (allowMouseBaseHold || allowMouseBaseSingle))
            {
                StartCoroutine(MousePressDownBase());
            }
            else if (selectManager.selectingObject && selectManager.selectedTroop.CompareTag("Skeleton") && camera.ScreenToWorldPoint(Input.mousePosition).y < .9f && allowMousePressStayTarget)
            {
                if (selectManager.selectedTroop.GetComponent<Skeleton>().goal != null)
                {
                    selectManager.selectedTroop.GetComponent<Skeleton>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                    selectManager.selectedTroop.GetComponent<Skeleton>().goal = null;
                }
                selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
                selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
                selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal = new Vector2(camera.ScreenToWorldPoint(Input.mousePosition).x, camera.ScreenToWorldPoint(Input.mousePosition).y);
                selectManager.stayPositionMarker.gameObject.SetActive(true);
                selectManager.stayPositionMarker.position = selectManager.selectedTroop.GetComponent<Skeleton>().stayGoal;
            }
            else if (!selectManager.selectingObject)
            {
                xHoldPosition = selectManager.transform.position.x;
                startingXHoldPosition = Input.mousePosition;
                movingCamera = true;
            }
        }
        else if (movingCamera)
        {
            if (inputManager.paused || !inputManager.allowInputs || selectManager.selectingObject || !Input.GetKey(KeyCode.Mouse0) || !allowMouseMovement)
            {
                movingCamera = false;
            }
            else
            {
                selectManager.transform.position = new Vector3(Mathf.Clamp(xHoldPosition + ((camera.ScreenToWorldPoint(startingXHoldPosition).x - camera.ScreenToWorldPoint(Input.mousePosition).x) * Time.deltaTime * -1.5f), selectManager.minX, selectManager.maxX), selectManager.transform.position.y, 0f);
                xHoldPosition = selectManager.transform.position.x;
            }
        }

        //Normal Inputs
        else if (inputManager.allowInputs)
        {
            if (inputManager.buttonPressed)
            {
                buttonPressTimer += Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inputManager.Pause();
                inputManager.enabled = true;
                this.enabled = false;
            }
            else if (Input.GetKeyDown(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton))
            {
                if (!inputManager.buttonPressed)
                {
                    if (allowHoldMovement)
                    {
                        inputManager.holdInputWait = true;
                    }
                    inputManager.buttonPressed = true;
                    buttonType = ButtonPressed.left;
                }
                else if (buttonType == ButtonPressed.left)
                {
                    //Debug.Log("Double Left");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    inputManager.buttonPressed = false;
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
            else if (Input.GetKeyDown(inputManager.middleButton) && !Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.rightButton))
            {
                if (!inputManager.buttonPressed)
                {
                    if (allowHoldMovement)
                    {
                        inputManager.holdInputWait = true;
                    }
                    inputManager.buttonPressed = true;
                    buttonType = ButtonPressed.middle;
                }
                else if (buttonType == ButtonPressed.middle)
                {
                    //Debug.Log("Double Middle");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    inputManager.buttonPressed = false;
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
            else if (Input.GetKeyDown(inputManager.rightButton) && !Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton))
            {
                if (!inputManager.buttonPressed)
                {
                    if (allowHoldMovement)
                    {
                        inputManager.holdInputWait = true;
                    }
                    inputManager.buttonPressed = true;
                    buttonType = ButtonPressed.right;
                }
                else if (buttonType == ButtonPressed.right)
                {
                    //Debug.Log("Double Right");
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    inputManager.buttonPressed = false;
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
                inputManager.buttonPressed = false;
                if (allowHoldMovement)
                {
                    inputManager.holdInputWait = false;
                }
                if (!(Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.middleButton)) && !(Input.GetKey(inputManager.leftButton) && Input.GetKey(inputManager.rightButton)) && !(Input.GetKey(inputManager.middleButton) && Input.GetKey(inputManager.rightButton)))
                {
                    if (Input.GetKey(inputManager.leftButton))
                    {
                        //Debug.Log("Hold Left");
                        HoldPress(buttonType);
                    }
                    else if (buttonType == ButtonPressed.left)
                    {
                        //Debug.Log("Single Left");
                        SinglePress(buttonType);
                    }
                    else if (Input.GetKey(inputManager.middleButton))
                    {
                        //Debug.Log("Hold Middle");
                        HoldPress(buttonType);
                    }
                    else if (buttonType == ButtonPressed.middle)
                    {
                        //Debug.Log("Single Middle");
                        SinglePress(buttonType);
                    }
                    else if (Input.GetKey(inputManager.rightButton))
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

    IEnumerator MousePressDownSkeleton()
    {
        doingMouseCoroutine = true;
        Transform selectedObject = selectManager.selectedTroop;
        float timer = 0f;
        while (timer < buttonPressTime && inputManager.allowInputs)
        {
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            if (!Input.GetKey(KeyCode.Mouse0))
            {
                break;
            }
        }
        if (timer < buttonPressTime && inputManager.allowInputs)
        {
            while (timer < buttonPressTime && inputManager.allowInputs)
            {
                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
                if (Input.GetKey(KeyCode.Mouse0) && inputManager.allowInputs)
                {
                    if (selectManager.selectingObject && selectedObject == selectManager.selectedTroop && allowMouseSelectDouble)
                    {
                        if (selectedObject.CompareTag("Skeleton"))
                        {
                            selectedObject.GetComponent<Skeleton>().SpecialAttack();
                        }
                        else if (selectedObject.CompareTag("Minion"))
                        {
                            selectManager.TakeMinionBones();
                        }
                    }
                    yield return new WaitForFixedUpdate();
                    break;
                }
            }
            if (!Input.GetKey(KeyCode.Mouse0) && inputManager.allowInputs)
            {
                if (selectManager.selectingObject && selectedObject == selectManager.selectedTroop && allowMouseSelectSingle)
                {
                    if (selectedObject.CompareTag("Skeleton"))
                    {
                        if (selectedObject.GetComponent<Skeleton>().skeletonMode == SkeletonMode.stay)
                        {
                            selectManager.stayPositionMarker.gameObject.SetActive(false);
                        }
                        if (selectedObject.GetComponent<Skeleton>().skeletonMode == SkeletonMode.left || selectedObject.position.y > (slopeBase * (selectedObject.position.x - 3f)) + posYBase)
                        {
                            selectedObject.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                            selectManager.skeletonStatus.sprite = selectManager.skeletonAttack;
                        }
                        else
                        {
                            selectedObject.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
                            selectManager.skeletonStatus.sprite = selectManager.skeletonRun;
                        }
                    }
                    else if (selectedObject.CompareTag("Minion"))
                    {
                        if (selectedObject.GetComponent<Minion>().inDiggingMode)
                        {
                            selectedObject.GetComponent<Minion>().inDiggingMode = false;
                            selectManager.minionStatus.sprite = selectManager.minionAttack;
                            if (selectedObject.GetComponent<Minion>().goal != null && selectedObject.GetComponent<Minion>().goal.CompareTag("Grave"))
                            {
                                selectedObject.GetComponent<Minion>().grave.targetSelect.SetActive(false);
                                selectedObject.GetComponent<Minion>().grave = null;
                                selectManager.selectedTroop.GetComponent<Minion>().goal = null;
                            }
                        }
                        else
                        {
                            selectedObject.GetComponent<Minion>().inDiggingMode = true;
                            selectManager.minionStatus.sprite = selectManager.minionDig;
                            if (selectedObject.GetComponent<Minion>().goal != null && selectedObject.GetComponent<Minion>().goal.CompareTag("Enemy"))
                            {
                                selectedObject.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                                selectManager.selectedTroop.GetComponent<Minion>().goal = null;
                            }
                        }
                    }
                }
            }
        }
        else if (inputManager.allowInputs && allowMouseSelectHold)
        {
            selectManager.Deselect();
        }
        yield return new WaitForFixedUpdate();
        doingMouseCoroutine = false;
    }

    IEnumerator MousePressDownBase()
    {
        float timer = 0f;
        while (timer < buttonPressTime && inputManager.allowInputs)
        {
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            if (!Input.GetKey(KeyCode.Mouse0))
            {
                break;
            }
        }
        if (timer < buttonPressTime && inputManager.allowInputs && allowMouseBaseSingle)
        {
            playerBase.ArrowAttack();
        }
        else if (inputManager.allowInputs && allowMouseBaseHold)
        {
            playerBase.HealAll();
        }
    }

    private void SinglePress(ButtonPressed type)
    {
        if (allowSingle)
        {
            if (selectManager.selectingObject)
            {
                if (type == ButtonPressed.left && allowLeft)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[0]));
                    selectManager.SelectLeftTroop();
                    tutorialManager.buttonPressed = true;
                }
                else if (type == ButtonPressed.middle && allowMiddle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[1]));
                    selectManager.Deselect();
                    tutorialManager.buttonPressed = true;
                }
                else if (type == ButtonPressed.right && allowRight)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[2]));
                    selectManager.SelectRightTroop();
                    tutorialManager.buttonPressed = true;
                }
            }
            else if (type == ButtonPressed.left && allowLeft)
            {
                StartCoroutine(PressButtonVisually(buttonImages[0]));
                playerBase.HealAll();
                tutorialManager.buttonPressed = true;
            }
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                StartCoroutine(PressButtonVisually(buttonImages[1]));
                selectManager.SelectNearestTroop();
                tutorialManager.buttonPressed = true;
            }
            else if (type == ButtonPressed.right && allowRight)
            {
                StartCoroutine(PressButtonVisually(buttonImages[2]));
                playerBase.ArrowAttack();
                tutorialManager.buttonPressed = true;
            }
        }
    }

    private void DoublePress(ButtonPressed type)
    {
        if (allowDouble)
        {
            if (selectManager.selectingObject)
            {
                if (selectManager.selectedTroop.CompareTag("Skeleton"))
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[3]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
                        selectManager.skeletonStatus.sprite = selectManager.skeletonRun;
                        selectManager.stayPositionMarker.gameObject.SetActive(false);
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[4]));
                        selectManager.TroopStay();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[5]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                        selectManager.skeletonStatus.sprite = selectManager.skeletonAttack;
                        selectManager.stayPositionMarker.gameObject.SetActive(false);
                        tutorialManager.buttonPressed = true;
                    }
                }
                else if (selectManager.selectedTroop.CompareTag("Minion"))
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[3]));
                        selectManager.AllMinionsMine();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[4]));
                        selectManager.TakeMinionBones();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[5]));
                        selectManager.AllMinionsAttack();
                        tutorialManager.buttonPressed = true;
                    }
                }
                else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[3]));
                        selectManager.AllCorpsesSpawnMinions();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[4]));
                        selectManager.AllCorpsesSpawnTombstones();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[5]));
                        selectManager.AllCorpsesSpawnSkeletons();
                        tutorialManager.buttonPressed = true;
                    }
                }
            }
            else if (type == ButtonPressed.left && allowLeft)
            {
                StartCoroutine(PressButtonVisually(buttonImages[3]));
                selectManager.AllTroopsLeft();
                tutorialManager.buttonPressed = true;
            }
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                StartCoroutine(PressButtonVisually(buttonImages[4]));
                selectManager.AllTroopsStay();
                tutorialManager.buttonPressed = true;
            }
            else if (type == ButtonPressed.right && allowRight)
            {
                StartCoroutine(PressButtonVisually(buttonImages[5]));
                selectManager.AllTroopsRight();
                tutorialManager.buttonPressed = true;
            }
        }
    }

    private void HoldPress(ButtonPressed type)
    {
        if (allowHold)
        {
            if (selectManager.selectingObject)
            {
                if (selectManager.selectedTroop.CompareTag("Skeleton"))
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[6]));
                        selectManager.UpgradeDefence();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[7]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().SpecialAttack();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[8]));
                        selectManager.UpgradeAttack();
                        tutorialManager.buttonPressed = true;
                    }
                }
                else if (selectManager.selectedTroop.CompareTag("Minion"))
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[6]));
                        selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = true;
                        if (selectManager.selectedTroop.GetComponent<Minion>().goal != null && selectManager.selectedTroop.GetComponent<Minion>().goal.CompareTag("Enemy"))
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().goal.GetComponent<Enemy>().targetSelect.SetActive(false);
                            selectManager.selectedTroop.GetComponent<Minion>().goal = null;
                        }
                        selectManager.minionStatus.sprite = selectManager.minionDig;
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[7]));
                        selectManager.UpgradeShovel();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[8]));
                        selectManager.selectedTroop.GetComponent<Minion>().inDiggingMode = false;
                        if (selectManager.selectedTroop.GetComponent<Minion>().goal != null && selectManager.selectedTroop.GetComponent<Minion>().goal.CompareTag("Grave"))
                        {
                            selectManager.selectedTroop.GetComponent<Minion>().grave.targetSelect.SetActive(false);
                            selectManager.selectedTroop.GetComponent<Minion>().grave = null;
                            selectManager.selectedTroop.GetComponent<Minion>().goal = null;
                        }
                        selectManager.minionStatus.sprite = selectManager.minionAttack;
                        tutorialManager.buttonPressed = true;
                    }
                }
                else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[6]));
                        selectManager.CorpseSpawnMinion();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[7]));
                        selectManager.CorpseSpawnTombstone();
                        tutorialManager.buttonPressed = true;
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        StartCoroutine(PressButtonVisually(buttonImages[8]));
                        selectManager.CorpseSpawnSkeleton();
                        tutorialManager.buttonPressed = true;
                    }
                }
            }//Left and right handled inside select manager
            /*else if (type == ButtonPressed.left)
            {
                StartCoroutine(HoldButtonVisually(buttonImages[6], inputManager.leftButton));
            }*/
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                StartCoroutine(PressButtonVisually(buttonImages[7]));
                selectManager.UpgradeShovel();
                tutorialManager.buttonPressed = true;
            }
            /*else /*if (type == ButtonPressed.right)*/
            /*{
                StartCoroutine(HoldButtonVisually(buttonImages[8], inputManager.rightButton));
            }*/
        }
    }

    IEnumerator SlightWait()
    {
        yield return new WaitForSeconds(buttonPressTime / 2f);
        if (!inputManager.buttonPressed && allowHoldMovement)
        {
            inputManager.holdInputWait = false;
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
        yield return new WaitUntil(() => (Input.GetKeyUp(buttonType) || inputManager.buttonPressed));
        buttonImage.color = Color.white;
        if (buttonImage == buttonImages[6])
        {
            inputManager.holdLeft = false;
        }
        else
        {
            inputManager.holdRight = false;
        }
    }
}
