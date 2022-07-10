using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool buttonPressed = false;
    private float buttonPressTime = .3f;
    private ButtonPressed buttonType;

    public SelectManager selectManager;

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

        if (Input.GetKeyDown(leftButton))
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
        else if (Input.GetKeyDown(middleButton))
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
        else if (Input.GetKeyDown(rightButton))
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
            buttonPressTimer = 0f;
            buttonPressed = false;
        }
    }

    private void SinglePress(ButtonPressed type)
    {
        if (selectManager.selectingObject)
        {
            if (type == ButtonPressed.left)
            {
                selectManager.SelectLeftTroup();
            }
            else if (type == ButtonPressed.middle)
            {
                selectManager.Deselect();
            }
            else /*if (type == ButtonPressed.right)*/
            {
                selectManager.SelectRightTroop();
            }
        }
        else if (type == ButtonPressed.left)
        {
            selectManager.AllMinionsMine();
        }
        else if (type == ButtonPressed.middle)
        {
            selectManager.SelectNearestTroop();
        }
        else /*if (type == ButtonPressed.right)*/
        {
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
                    selectManager.TroupLeft();
                }
                else if (type == ButtonPressed.middle)
                {
                    selectManager.TroupStay();
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    selectManager.TroupRight();
                }
            }
        }
        else if (type == ButtonPressed.left)
        {
            selectManager.AllTroupsLeft();
        }
        else if (type == ButtonPressed.middle)
        {
            selectManager.AllTroupsStay();
        }
        else /*if (type == ButtonPressed.right)*/
        {
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
                    selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeDefence();
                }
                else if (type == ButtonPressed.middle)
                {
                    //selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeDefence();//Explode????
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    selectManager.selectedTroop.GetComponent<Skeleton>().UpgradeAttack();
                }
            }
            else /*if (selectManager.selectedTroop.CompareTag("Corpse"))*/
            {
                if (type == ButtonPressed.left)
                {
                    selectManager.selectedTroop.GetComponent<Corpse>().AddBones();
                }
                else if (type == ButtonPressed.middle)
                {
                    //selectManager.selectedTroop.GetComponent<Corpse>().UpgradeDefence();//Spawn Tombstones????
                }
                else /*if (type == ButtonPressed.right)*/
                {
                    selectManager.selectedTroop.GetComponent<Corpse>().SpawnSkeleton();
                }
            }
        }//Left and right are inside select manager
        else if (type == ButtonPressed.middle)
        {
            selectManager.BuyMinion();
        }
    }
}
