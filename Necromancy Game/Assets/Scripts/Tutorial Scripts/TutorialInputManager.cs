using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Update is called once per frame
    void Update()
    {
        if (inputManager.buttonPressed)
        {
            buttonPressTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton))
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
                if (allowHoldMovement)
                {
                    StartCoroutine(SlightWait());
                }
                DoublePress(buttonType);
                buttonPressTimer = 0f;
                inputManager.buttonPressed = false;
            }
            else if (buttonType == ButtonPressed.middle)
            {
                //Debug.Log("Single Middle + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
                buttonPressTimer = 0f;
                buttonType = ButtonPressed.left;
            }
            else if (buttonType == ButtonPressed.right)
            {
                //Debug.Log("Single Right + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
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
                if (allowHoldMovement)
                {
                    StartCoroutine(SlightWait());
                }
                DoublePress(buttonType);
                buttonPressTimer = 0f;
                inputManager.buttonPressed = false;
            }
            else if (buttonType == ButtonPressed.left)
            {
                //Debug.Log("Single Left + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
                buttonPressTimer = 0f;
                buttonType = ButtonPressed.middle;
            }
            else if (buttonType == ButtonPressed.right)
            {
                //Debug.Log("Single Right + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
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
                if (allowHoldMovement)
                {
                    StartCoroutine(SlightWait());
                }
                DoublePress(buttonType);
                buttonPressTimer = 0f;
                inputManager.buttonPressed = false;
            }
            else if (buttonType == ButtonPressed.left)
            {
                //Debug.Log("Single Left + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
                buttonPressTimer = 0f;
                buttonType = ButtonPressed.right;
            }
            else if (buttonType == ButtonPressed.middle)
            {
                //Debug.Log("Single Middle + ");
                if (allowSingle)
                {
                    SinglePress(buttonType);
                }
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

    private void SinglePress(ButtonPressed type)
    {
        if (allowSingle)
        {
            if (selectManager.selectingObject)
            {
                if (type == ButtonPressed.left && allowLeft)
                {
                    tutorialManager.buttonPressed = true;
                    StartCoroutine(PressButtonVisually(buttonImages[0]));
                    selectManager.SelectLeftTroop();
                }
                else if (type == ButtonPressed.middle && allowMiddle)
                {
                    tutorialManager.buttonPressed = true;
                    StartCoroutine(PressButtonVisually(buttonImages[1]));
                    selectManager.Deselect();
                }
                else if (type == ButtonPressed.right && allowRight)
                {
                    tutorialManager.buttonPressed = true;
                    StartCoroutine(PressButtonVisually(buttonImages[2]));
                    selectManager.SelectRightTroop();
                }
            }
            else if (type == ButtonPressed.left && allowLeft)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[0]));
                selectManager.AllMinionsMine();
            }
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[1]));
                selectManager.SelectNearestTroop();
            }
            else if (type == ButtonPressed.right && allowRight)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[2]));
                selectManager.AllMinionsAttack();
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
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[3]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[4]));
                        selectManager.TroopStay();
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[5]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
                    }
                }
                else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[3]));
                        selectManager.AllCorpsesSpawnMinions();
                    }
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[4]));
                        selectManager.AllCorpsesSpawnTombstones();
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[5]));
                        selectManager.AllCorpsesSpawnSkeletons();
                    }
                }
            }
            else if (type == ButtonPressed.left && allowLeft)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[3]));
                selectManager.AllTroopsLeft();
            }
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[4]));
                selectManager.AllTroopsStay();
            }
            else if (type == ButtonPressed.right && allowRight)
            {
                tutorialManager.buttonPressed = true;
                StartCoroutine(PressButtonVisually(buttonImages[5]));
                selectManager.AllTroopsRight();
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
                        tutorialManager.buttonPressed = true;
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
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        tutorialManager.buttonPressed = true;
                        StartCoroutine(PressButtonVisually(buttonImages[7]));
                        selectManager.selectedTroop.GetComponent<Skeleton>().TurnIntoTombstone();
                    }
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        tutorialManager.buttonPressed = true;
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
                else if (selectManager.selectedTroop.CompareTag("Corpse"))
                {
                    if (type == ButtonPressed.left && allowLeft)
                    {
                        tutorialManager.buttonPressed = true;
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
                    else if (type == ButtonPressed.middle && allowMiddle)
                    {
                        tutorialManager.buttonPressed = true;
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
                    else if (type == ButtonPressed.right && allowRight)
                    {
                        tutorialManager.buttonPressed = true;
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
            else if (type == ButtonPressed.middle && allowMiddle)
            {
                tutorialManager.buttonPressed = true;
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
    }

    IEnumerator SlightWait()
    {
        yield return new WaitForSeconds(buttonPressTime / 2f);
        if (!inputManager.buttonPressed)
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
}
