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

    private bool allowInputs = true;
    public KeyCode leftButton = KeyCode.A;
    public KeyCode middleButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    private float buttonPressTimer = 0f;
    public bool buttonPressed = false;
    private float buttonPressTime = .4f;
    private ButtonPressed buttonType;

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

    IEnumerator SinglePress(ButtonPressed type)
    {
        if (type == ButtonPressed.left)
        {
            //StartGame();
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
            //StartTutorial();
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
            rightButtonImage.color = Color.gray;
            ExitGame();
        }
    }

    public void StartGame()
    {
        loading.SetActive(true);
        SceneManager.LoadScene("Endless Mode");
    }

    public void StartTutorial()
    {
        loading.SetActive(true);
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    IEnumerator HoldPress(ButtonPressed type)
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
                    if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != rightButton)
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
                    if (Input.GetKeyDown((KeyCode)i) && (KeyCode)i != leftButton && (KeyCode)i != middleButton)
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

    /*public void EnterLevelSelect()
    {
        isMainScreen = false;
        text100.SetActive(false);
        if (!isData)
        {
            Debug.Log("No Save Data");
            for (int i = 0; i < buttons1.Length; i++)
            {
                buttons1[i].SetActive(false);
            }
            buttons2[7].SetActive(true);
        }
        else
        {
            Debug.Log("Loaded Save Data");
            for (int i = 0; i < buttons1.Length; i++)
            {
                buttons1[i].SetActive(false);
            }
            for (int i = 0; i < buttons2.Length - 3; i++)
            {
                if (data[i + 20] == 0)
                {
                    buttons2[i].SetActive(false);
                }
                else
                {
                    buttons2[i].SetActive(true);
                }
            }
            buttons2[5].SetActive(true);
            buttons2[6].SetActive(true);
            buttons2[7].SetActive(true);
        }
    }*/

    /*public void LoadLevel(string sceneName)
    {
        for (int i = 0; i < buttons1.Length; i++)
        {
            buttons1[i].SetActive(false);
        }
        for (int i = 0; i < buttons2.Length; i++)
        {
            buttons2[i].SetActive(false);
        }
        loading.SetActive(true);
        if (selectCharacter)
        {
            StartCoroutine(WaitLoadCreator(sceneName));
        }
        else if (selectCostume)
        {
            StartCoroutine(WaitLoadCostume(sceneName));
        }
        else
        {
            StartCoroutine(WaitLoadScene(sceneName));
        }
    }

    IEnumerator WaitLoadCreator(string sceneName)
    {
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Character Creator", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        GameObject.FindGameObjectWithTag("Character Creator Manager").GetComponent<CharacterCreatorManager>().level = sceneName;
        GameObject.FindGameObjectWithTag("Character Creator Manager").GetComponent<CharacterCreatorManager>().costume = selectCostume;
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    IEnumerator WaitLoadCostume(string sceneName)
    {
        AsyncOperation ao2 = SceneManager.LoadSceneAsync("Dress Up Room", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao2.isDone);
        GameObject.FindGameObjectWithTag("Dress Up Door").GetComponent<DoorPortal>().nextSceneName = sceneName;
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    IEnumerator WaitLoadScene(string sceneName)
    {
        AsyncOperation ao2 = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao2.isDone);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    public void BackToMainMenu()
    {
        isMainScreen = true;
        if (fullCompletion) text100.SetActive(true);
        for (int i = 0; i < buttons2.Length; i++)
        {
            buttons2[i].SetActive(false);
        }
        for (int i = 0; i < buttons1.Length; i++)
        {
            buttons1[i].SetActive(true);
        }
    }

    public void ChangeOption(bool isCharacter)
    {
        if (isCharacter)
        {
            characterBlackPart.SetActive(selectCharacter);
            selectCharacter = !selectCharacter;
        }
        else
        {
            costumeBlackPart.SetActive(selectCostume);
            selectCostume = !selectCostume;
        }
    }*/
}
