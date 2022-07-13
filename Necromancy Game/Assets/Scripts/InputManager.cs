using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float buttonPressTime = .3f;
    private ButtonPressed buttonType;

    public GameObject impossibleActionPrefab;
    public PlayerBase playerBase;
    public SelectManager selectManager;
    public Image[] buttonImages;
    public bool holdInputWait = false;
    public bool holdRight = false;
    public bool holdLeft = false;

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
        {
            buttonPressTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(leftButton) && !Input.GetKey(middleButton) && !Input.GetKey(rightButton))
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

    private void SinglePress(ButtonPressed type)
    {
        if (selectManager.selectingObject)
        {
            if (type == ButtonPressed.left)
            {
                StartCoroutine(PressButtonVisually(buttonImages[0]));
                selectManager.SelectLeftTroup();
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
            selectManager.AllMinionsMine();
        }
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[1]));
            selectManager.SelectNearestTroop();
        }
        else /*if (type == ButtonPressed.right)*/
        {
            StartCoroutine(PressButtonVisually(buttonImages[2]));
            selectManager.AllMinionsAttack();
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
                    selectManager.TroupStay();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[5]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
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
            selectManager.AllTroupsLeft();
        }
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[4]));
            selectManager.AllTroupsStay();
        }
        else /*if (type == ButtonPressed.right)*/
        {
            StartCoroutine(PressButtonVisually(buttonImages[5]));
            selectManager.AllTroupsRight();
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
