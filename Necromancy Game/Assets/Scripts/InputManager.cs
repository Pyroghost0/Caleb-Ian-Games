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
                Debug.Log("Double Left");
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
            else
            {
                Debug.Log("Invalid Input");
                buttonPressTimer = 0f;
                buttonPressed = false;
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
                Debug.Log("Double Middle");
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
            else
            {
                Debug.Log("Invalid Input");
                buttonPressTimer = 0f;
                buttonPressed = false;
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
                Debug.Log("Double Right");
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
            else
            {
                Debug.Log("Invalid Input");
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
        }
        else if (buttonPressTimer >= buttonPressTime)
        {
            if (Input.GetKey(leftButton))
            {
                Debug.Log("Hold Left");
            }
            else if (buttonType == ButtonPressed.left)
            {
                Debug.Log("Single Left");
            }
            else if (Input.GetKey(middleButton))
            {
                Debug.Log("Hold Middle");
            }
            else if (buttonType == ButtonPressed.middle)
            {
                Debug.Log("Single Middle");
            }
            else if (Input.GetKey(rightButton))
            {
                Debug.Log("Hold Right");
            }
            else if (buttonType == ButtonPressed.right)
            {
                Debug.Log("Single Right");
            }
            buttonPressTimer = 0f;
            buttonPressed = false;
        }
    }
}
