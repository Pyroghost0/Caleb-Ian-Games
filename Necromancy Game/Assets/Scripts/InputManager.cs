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

    public SelectManager selectManager;
    public Image[] buttonImages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
                buttonPressed = true;
                buttonType = ButtonPressed.left;
            }
            else if (buttonType == ButtonPressed.left)
            {
                //Debug.Log("Double Left");
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
                buttonPressed = true;
                buttonType = ButtonPressed.middle;
            }
            else if (buttonType == ButtonPressed.middle)
            {
                //Debug.Log("Double Middle");
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
                buttonPressed = true;
                buttonType = ButtonPressed.right;
            }
            else if (buttonType == ButtonPressed.right)
            {
                //Debug.Log("Double Right");
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
                    selectManager.TroupLeft();
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[4]));
                    selectManager.TroupStay();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[5]));
                    selectManager.TroupRight();
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
                    selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeDefence();
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[7]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().TurnIntoTombstone();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[8]));
                    selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeAttack();
                }
            }
            else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
            {
                if (type == ButtonPressed.left)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[6]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnMinion();
                }
                else if (type == ButtonPressed.middle)
                {
                    StartCoroutine(PressButtonVisually(buttonImages[7]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnTombStones();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    StartCoroutine(PressButtonVisually(buttonImages[8]));
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnSkeleton();
                }
            }
        }//Left and right are inside select manager
        else if (type == ButtonPressed.middle)
        {
            StartCoroutine(PressButtonVisually(buttonImages[7]));
            selectManager.BuyMinion();
        }
    }

    IEnumerator PressButtonVisually(Image buttonImage)
    {
        buttonImage.color = Color.gray;
        yield return new WaitForSeconds(buttonPressTime);
        buttonImage.color = Color.white;
    }

    IEnumerator HoldButtonVisually(Image buttonImage, KeyCode buttonType)
    {
        buttonImage.color = Color.gray;
        yield return new WaitUntil(() => (Input.GetKeyUp(buttonType) || buttonPressed));
        buttonImage.color = Color.white;
    }
}
