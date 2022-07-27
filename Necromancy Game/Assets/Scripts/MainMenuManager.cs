using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    //public GameObject[] buttons1;
    //public GameObject[] buttons2;
    public GameObject loading;
    public TextMeshProUGUI leftButtonText;
    public TextMeshProUGUI middleButtonText;
    public TextMeshProUGUI rightButtonText;

    public Image leftButtonImage;
    public Image middleButtonImage;
    public Image rightButtonImage;
    public Image holdLeftButtonImage;
    public Image holdMiddleButtonImage;
    public Image holdRightButtonImage;
    public Image otherLeftButtonImage;
    public Image otherMiddleButtonImage;
    public Image otherRightButtonImage;
    public Image otherHoldLeftButtonImage;
    public Image otherHoldMiddleButtonImage;
    public Image otherHoldRightButtonImage;

    private bool allowInputs = true;
    public KeyCode leftButton = KeyCode.A;
    public KeyCode middleButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    private float buttonPressTimer = 0f;
    public bool buttonPressed = false;
    private float buttonPressTime = .4f;
    private ButtonPressed buttonType;

    public bool inOtherSelection = false;
    public bool inMap = false;
    public TextMeshProUGUI singleLeftButtonText;
    public TextMeshProUGUI singleMiddleButtonText;
    public TextMeshProUGUI singleRightButtonText;
    public RectTransform mainMenuScreen;
    public RectTransform mapScreen;
    public string[] normalSceneName;
    public string[] extraSceneName;
    public RectTransform[] mapPoints;
    public int mapPointIndex;
    public RectTransform selector;
    private float clickTime;

    public bool[] unlockedLevels;

    void Start()
    {
        leftButtonText.text = leftButton.ToString();
        middleButtonText.text = middleButton.ToString();
        rightButtonText.text = rightButton.ToString();
    }

    public void Update()
    {
        if (allowInputs)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                clickTime = 0f;
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                clickTime += Time.deltaTime;
            }
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
                /*else if (buttonType == ButtonPressed.left)
                {
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }*/
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
                /*else if (buttonType == ButtonPressed.middle)
                {
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }*/
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
                /*else if (buttonType == ButtonPressed.right)
                {
                    StartCoroutine(SlightWait());
                    DoublePress(buttonType);
                    buttonPressTimer = 0f;
                    buttonPressed = false;
                }*/
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
                        StartCoroutine(HoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.left)
                    {
                        StartCoroutine(SinglePress(buttonType));
                    }
                    else if (Input.GetKey(middleButton))
                    {
                        StartCoroutine(HoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.middle)
                    {
                        StartCoroutine(SinglePress(buttonType));
                    }
                    else if (Input.GetKey(rightButton))
                    {
                        StartCoroutine(HoldPress(buttonType));
                    }
                    else if (buttonType == ButtonPressed.right)
                    {
                        StartCoroutine(SinglePress(buttonType));
                    }
                }
            }
        }
    }

    public void ClickSinleButtonLeft()
    {
        if (allowInputs)
        {
            StartCoroutine(SinglePress(ButtonPressed.left));
        }
    }

    public void ClickSinleButtoMiddle()
    {
        if (allowInputs)
        {
            StartCoroutine(SinglePress(ButtonPressed.middle));
        }
    }

    public void ClickSinleButtonRight()
    {
        if (allowInputs)
        {
            StartCoroutine(SinglePress(ButtonPressed.right));
        }
    }

    IEnumerator SinglePress(ButtonPressed type)
    {
        if (inMap)
        {
            if (type == ButtonPressed.left)
            {
                if (mapPointIndex != 0)
                {//Don't need to check if unlocked
                    mapPointIndex--;
                    selector.anchoredPosition = mapPoints[mapPointIndex].anchoredPosition;
                }
            }
            else if (type == ButtonPressed.middle)
            {
                Debug.Log("Select Level");
                /*allowInputs = false;
                otherMiddleButtonImage.color = Color.gray;
                loading.SetActive(true);
                if (inOtherSelection)
                {
                    AsyncOperation ao = SceneManager.LoadSceneAsync(extraSceneName[mapPointIndex], LoadSceneMode.Additive);
                    yield return new WaitUntil(() => ao.isDone);
                }
                else
                {
                    AsyncOperation ao = SceneManager.LoadSceneAsync(normalSceneName[mapPointIndex], LoadSceneMode.Additive);
                    yield return new WaitUntil(() => ao.isDone);
                }
                InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
                inputManager.leftButton = leftButton;
                inputManager.middleButton = middleButton;
                inputManager.rightButton = rightButton;
                SceneManager.UnloadSceneAsync("Main Menu");*/
            }
            else /*if (type == ButtonPressed.right)*/
            {
                if (mapPointIndex+1 < mapPoints.Length && unlockedLevels[mapPointIndex+1])
                {
                    mapPointIndex++;
                    selector.anchoredPosition = mapPoints[mapPointIndex].anchoredPosition;
                }
            }
        }
        else if (inOtherSelection)
        {
            if (type == ButtonPressed.left)
            {
                allowInputs = false;
                leftButtonImage.color = Color.gray;
                loading.SetActive(true);
                AsyncOperation ao = SceneManager.LoadSceneAsync("Endless Mode", LoadSceneMode.Additive);
                yield return new WaitUntil(() => ao.isDone);
                InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
                inputManager.leftButton = leftButton;
                inputManager.middleButton = middleButton;
                inputManager.rightButton = rightButton;
                SceneManager.UnloadSceneAsync("Main Menu");
            }
            else if (type == ButtonPressed.middle)
            {

                allowInputs = false;
                middleButtonImage.color = Color.gray;
                loading.SetActive(true);
                AsyncOperation ao = SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
                yield return new WaitUntil(() => ao.isDone);
                InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
                inputManager.leftButton = leftButton;
                inputManager.middleButton = middleButton;
                inputManager.rightButton = rightButton;
                SceneManager.UnloadSceneAsync("Main Menu");
            }
            else /*if (type == ButtonPressed.right)*/
            {
                singleLeftButtonText.text = "Campaign";
                singleMiddleButtonText.text = "Other Modes";
                singleRightButtonText.text = "Exit Game";
                inOtherSelection = false;
            }
        }
        else
        {
            if (type == ButtonPressed.left)
            {
                allowInputs = false;
                float timer = 0;
                while (timer < 2f)
                {
                    mainMenuScreen.anchoredPosition += new Vector2(mainMenuScreen.rect.x * Time.deltaTime, 0);
                    mapScreen.anchoredPosition += new Vector2(mainMenuScreen.rect.x * Time.deltaTime, 0);
                    timer += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                mainMenuScreen.anchoredPosition = new Vector2(mainMenuScreen.rect.x*2f, 0);
                mapScreen.anchoredPosition = Vector2.zero;
                allowInputs = true;
                inMap = true;
            }
            else if (type == ButtonPressed.middle)
            {
                singleLeftButtonText.text = "Endless Mode";
                singleMiddleButtonText.text = "Tutorial";
                singleRightButtonText.text = "Back";
                inOtherSelection = true;
            }
            else /*if (type == ButtonPressed.right)*/
            {
                rightButtonImage.color = Color.gray;
                Debug.Log("Quit Game");
                Application.Quit();
            }
        }
    }

    public void ClickHoldButtonLeft()
    {
        if (allowInputs)
        {
            StartCoroutine(HoldPress(ButtonPressed.left));
        }
    }

    public void ClickHoldButtonMiddle()
    {
        if (allowInputs)
        {
            StartCoroutine(HoldPress(ButtonPressed.middle));
        }
    }

    public void ClickHoldButtonRight()
    {
        if (allowInputs)
        {
            StartCoroutine(HoldPress(ButtonPressed.right));
        }
    }

    IEnumerator HoldPress(ButtonPressed type)
    {
        if (inMap)
        {
            if (type == ButtonPressed.left)
            {
                Debug.Log("More Details");
            }
            else if (type == ButtonPressed.middle)
            {
                Debug.Log("Changed To Special Missions");
            }
            else /*if (type == ButtonPressed.right)*/
            {
                allowInputs = false;
                float timer = 0;
                while (timer < 2f)
                {
                    mainMenuScreen.anchoredPosition -= new Vector2(mainMenuScreen.rect.x * Time.deltaTime, 0);
                    mapScreen.anchoredPosition -= new Vector2(mainMenuScreen.rect.x * Time.deltaTime, 0);
                    timer += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                mainMenuScreen.anchoredPosition = Vector2.zero;
                mapScreen.anchoredPosition = new Vector2(-mainMenuScreen.rect.x*2f, 0);
                allowInputs = true;
                inMap = false;
            }
        }
        else
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
                        if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != middleButton && (KeyCode)i != rightButton && (KeyCode)i != KeyCode.Mouse0 && (KeyCode)i != KeyCode.Escape)
                        {
                            newKey = (KeyCode)i;
                            break;
                        }
                    }
                }
                leftButton = newKey;
                leftButtonText.text = newKey.ToString();
                yield return new WaitForFixedUpdate();
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
                        if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != rightButton && (KeyCode)i != KeyCode.Mouse0 && (KeyCode)i != KeyCode.Escape)
                        {
                            newKey = (KeyCode)i;
                            break;
                        }
                    }
                }
                middleButton = newKey;
                middleButtonText.text = newKey.ToString();
                yield return new WaitForFixedUpdate();
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
                        if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != middleButton && (KeyCode)i != KeyCode.Mouse0 && (KeyCode)i != KeyCode.Escape)
                        {
                            newKey = (KeyCode)i;
                            break;
                        }
                    }
                }
                rightButton = newKey;
                rightButtonText.text = newKey.ToString();
                yield return new WaitForFixedUpdate();
                allowInputs = true;
            }
        }
    }

    public void SelectLevel(int num)
    {
        if (unlockedLevels[num])
        {
            if (num == mapPointIndex)
            {
                if (clickTime < buttonPressTime)
                {
                    Debug.Log("Select Level");
                }
                else
                {
                    Debug.Log("Side Missions");
                }
            }
            else
            {
                mapPointIndex = num;
                selector.anchoredPosition = mapPoints[num].anchoredPosition;
            }
        }
    }

    /*public void RemoveData()
    {
        SaveLoad.ClearData();
        removeMenu.SetActive(false);
        isData = false;
        fullCompletion = false;
        text100.SetActive(false);
    }
    public void FullUnlock()
    {
        SaveLoad.FullUnlock();
        isData = true;
        if (data == null)
        {
            data = new int[25];
            data[0] = 0;
            data[1] = 1;
            data[2] = 0;
            data[3] = 0;
            for (int i = 4; i < 25; i++)
            {
                data[i] = 1;
            }
        }
        else
        {
            for (int i = 4; i < 25; i++)
            {
                data[i] = 1;
            }
        }

        unlockMenu.SetActive(false);
        fullCompletion = true;
        text100.SetActive(true);
    }
    public void CloseMenu()
    {
        removeMenu.SetActive(false);
        unlockMenu.SetActive(false);
    }*/

}
