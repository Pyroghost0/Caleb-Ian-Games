using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
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
    public float[] mapYPositions;
    public string[] normalSceneName;
    public string[] extraSceneName;
    public RectTransform[] mapPoints;
    public int mapPointIndex;
    public RectTransform selector;
    private float clickTime;
    public RectTransform scrollBar;//70 -> 300
    public RectTransform map;//0 -> -750
    public GameObject normalMap;
    public GameObject saveDataMenu;

    public bool firstTime;
    private bool secondPartText = false;
    public GameObject detailsMenu;
    public TextMeshProUGUI startingText;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI detailsText;
    private string[] levelDetailsText = { "Goblins surround the outskirts of the kingdom because they are the most common monster and can suround the kingdom with their massive numbers. After killing a few goblins, you find where the goblin chief lives.",
    "Wolves have always been a natural enemy of the kingdom, but the wolves were somehow tamed by the monsters. It is now your turn to use the wolves by finding and killing the pack leader to gain the ability to summon skeleton wolves.",
    "Witches are one of your natural enemies, but they didn't interfere in the war. However, you've heard rumors that the monsters have befriended them in preporation ageinst you. You need to kill the Witch Queen before you face them elsewhere, and can't summon any skeletons.",
    "Orcs were perhaps the monster that killed the most during the war. They had high number, had high attack, and high defence. Now they lay inside, wilting away from the past years of peace. Now that you've arrived at the tower and pepaired countermeasures, kill the Orc chief.",
    "Ogres are the strongest monsters by far. Even a smaller, inexperienced ogre can easily kill a human. Thankfully, you aren't fighting with humans. Kill the one in charge, and finish what you came here to do.",
    "To your suprise, you find a way up while on the roof of the tower. After climbing, you see a man who says, \"So you are the one killing all my troops.\" Realising he is the one responsible for so much carnage, you decide to show him that your necromancy skills are superior to his skills."};

    void Start()
    {
        leftButtonText.text = leftButton.ToString();
        middleButtonText.text = middleButton.ToString();
        rightButtonText.text = rightButton.ToString();
        if (Data.Load() == null)
        {
            Debug.Log("New Save");
            bool[] newData = new bool[24];
            newData[1] = true;
            Data.Save(newData);
        }
        if (normalMap.activeSelf)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[i * 2])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else /*if (!Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[(i * 2) + 12])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2) + 13])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else if (!Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
                else /*if (Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = new Color(1f, .46f, .08f);
                }
            }
        }
        firstTime = !Data.Load()[0] && !inMap;
    }

    public void Update()
    {
        if (allowInputs)
        {
            if (inMap)
            {
                if (Input.mouseScrollDelta.y > 0 && map.anchoredPosition.y > -750f)
                {
                    map.anchoredPosition += new Vector2(0f, -Input.mouseScrollDelta.y * 10f);
                    scrollBar.anchoredPosition += new Vector2(0f, Input.mouseScrollDelta.y * .3066667f * 10f);
                    if (map.anchoredPosition.y < -750f)
                    {
                        map.anchoredPosition = new Vector2(0f, 750f);
                        scrollBar.anchoredPosition = new Vector2(-20f, 300f);
                    }
                }
                else if (Input.mouseScrollDelta.y < 0 && map.anchoredPosition.y < 0f)
                {
                    map.anchoredPosition += new Vector2(0f, -Input.mouseScrollDelta.y * 10f);
                    scrollBar.anchoredPosition += new Vector2(0f, Input.mouseScrollDelta.y * .3066667f * 10f);
                    if (map.anchoredPosition.y > 0f)
                    {
                        map.anchoredPosition = Vector2.zero;
                        scrollBar.anchoredPosition = new Vector2(-20f, 70f);
                    }
                }
            }
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
            if (Input.GetKey(leftButton) && Input.GetKey(middleButton) && Input.GetKey(rightButton) && (Input.GetKeyDown(leftButton) || Input.GetKeyDown(middleButton) || Input.GetKeyDown(rightButton)))
            {
                saveDataMenu.SetActive(true);
                allowInputs = false;
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
            else if (Input.GetKeyDown(leftButton) && !Input.GetKey(middleButton) && !Input.GetKey(rightButton))
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
        else if (saveDataMenu.activeSelf)
        {
            if (Input.GetKeyDown(leftButton) && !Input.GetKey(middleButton) && !Input.GetKey(rightButton))
            {
                UnlockAll();
            }
            else if (Input.GetKeyDown(middleButton) && !Input.GetKey(leftButton) && !Input.GetKey(rightButton))
            {
                Cancel();
            }
            else if (Input.GetKeyDown(rightButton) && !Input.GetKey(leftButton) && !Input.GetKey(middleButton))
            {
                DeleteData();
            }
        }
        else if (detailsMenu.activeSelf)
        {
            if (Input.GetKey(leftButton) && Input.GetKey(middleButton) && Input.GetKey(rightButton) && (Input.GetKeyDown(leftButton) || Input.GetKeyDown(middleButton) || Input.GetKeyDown(rightButton)))
            {
                saveDataMenu.SetActive(true);
                allowInputs = false;
                buttonPressTimer = 0f;
                buttonPressed = false;
            }
            else if (Input.GetKeyDown(middleButton) && !Input.GetKey(leftButton) && !Input.GetKey(rightButton))
            {
                DetailsMenuButtonClick();
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
                    map.anchoredPosition = new Vector2(0f, mapYPositions[mapPointIndex]);
                    scrollBar.anchoredPosition = new Vector2(-20f, mapYPositions[mapPointIndex] * -.3066667f + 70f);
                }
            }
            else if (type == ButtonPressed.middle)
            {
                StartCoroutine(LoadLevel());
            }
            else /*if (type == ButtonPressed.right)*/
            {
                if (mapPointIndex+1 < mapPoints.Length && Data.Load()[(mapPointIndex*2)+3])
                {
                    mapPointIndex++;
                    selector.anchoredPosition = mapPoints[mapPointIndex].anchoredPosition;
                    map.anchoredPosition = new Vector2(0f, mapYPositions[mapPointIndex]);
                    scrollBar.anchoredPosition = new Vector2(-20f, mapYPositions[mapPointIndex] * -.3066667f + 70f);
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
                if (firstTime)
                {
                    detailsMenu.SetActive(true);
                    allowInputs = false;
                    startingText.gameObject.SetActive(true);
                    levelText.gameObject.SetActive(false);
                    detailsText.gameObject.SetActive(false);
                    buttonText.text = "Continue";
                }
            }
            else if (type == ButtonPressed.middle)
            {
                singleLeftButtonText.text = "Endless Mode";
                singleMiddleButtonText.text = "Tutorials";
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
                OpenDetailsMenu();
            }
            else if (type == ButtonPressed.middle)
            {
                ChangeMap();
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
        if (Data.Load()[(num * 2) + 1])
        {
            if (num == mapPointIndex)
            {
                if (clickTime < buttonPressTime)
                {
                    StartCoroutine(LoadLevel());
                }
                else
                {
                    ChangeMap();
                }
            }
            else
            {
                mapPointIndex = num;
                selector.anchoredPosition = mapPoints[num].anchoredPosition;
                map.anchoredPosition = new Vector2(0f, mapYPositions[num]);
                scrollBar.anchoredPosition = new Vector2(-20f, mapYPositions[num] * -.3066667f + 70f);
            }
        }
    }

    IEnumerator LoadLevel()
    {
        allowInputs = false;
        otherMiddleButtonImage.color = Color.gray;
        loading.SetActive(true);
        if (normalMap.active)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(normalSceneName[mapPointIndex], LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
        }
        else
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(extraSceneName[mapPointIndex], LoadSceneMode.Additive);
            yield return new WaitUntil(() => ao.isDone);
        }
        
        InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
        inputManager.leftButton = leftButton;
        inputManager.middleButton = middleButton;
        inputManager.rightButton = rightButton;
        SceneManager.UnloadSceneAsync("Main Menu");
    }

    public void ChangeMap()
    {
        if (normalMap.activeSelf)
        {
            normalMap.SetActive(false);
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[(i*2)+12])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2)+ 13])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else if (!Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
                else /*if (Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = new Color(1f, .46f, .08f);
                }
            }
        }
        else
        {
            normalMap.SetActive(true);
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[i * 2])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else /*if (!Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
            }
        }
    }

    public void UnlockAll()
    {
        Data.FullUnlock();
        if (normalMap.activeSelf)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[i * 2])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else /*if (!Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (Data.Load()[(i * 2) + 12])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.green;
                }
                else if (Data.Load()[(i * 2) + 13])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.blue;
                }
                else if (!Data.Load()[(i * 2) + 1])
                {
                    mapPoints[i].GetComponent<Image>().color = Color.red;
                }
                else /*if (Data.Load()[(i * 2) + 1])*/
                {
                    mapPoints[i].GetComponent<Image>().color = new Color(1f, .46f, .08f);
                }
            }
        }
        allowInputs = true;
        saveDataMenu.SetActive(false);
    }

    public void Cancel()
    {
        allowInputs = !detailsMenu.activeSelf;
        saveDataMenu.SetActive(false);
    }

    public void DeleteData()
    {
        Data.ClearData();
        Debug.Log("New Save");
        bool[] newData = new bool[24];
        newData[1] = true;
        Data.Save(newData);
        selector.anchoredPosition = mapPoints[0].anchoredPosition;
        map.anchoredPosition = new Vector2(0f, mapYPositions[0]);
        scrollBar.anchoredPosition = new Vector2(-20f, mapYPositions[0] * -.3066667f + 70f);
        mapPointIndex = 0;
        normalMap.SetActive(true);
        mapPoints[0].GetComponent<Image>().color = Color.blue;
        for (int i = 1; i < 6; i++)
        {
            mapPoints[i].GetComponent<Image>().color = Color.red;
        }
        allowInputs = !detailsMenu.activeSelf;
        saveDataMenu.SetActive(false);
        if (!inMap)
        {
            firstTime = true;
            secondPartText = false;
            startingText.text = "Welcome to the campaign. Here you fight enemies to take back territory that has been long since forgotten about after enemies such as goblins, orcs, and ogres flooded the land. After years of learning necromancy, its time to use their numbers ageinst them.";
            levelText.text = "Level 1";
            detailsText.text = levelDetailsText[0];
        }
    }

    public void OpenDetailsMenu()
    {
        detailsMenu.SetActive(true);
        allowInputs = !detailsMenu.activeSelf;
        levelText.text = "Level " + (mapPointIndex + 1);
        detailsText.text = levelDetailsText[mapPointIndex];
    }

    public void DetailsMenuButtonClick()
    {
        if (firstTime)
        {
            if (secondPartText) {
                startingText.gameObject.SetActive(false);
                levelText.gameObject.SetActive(true);
                detailsText.gameObject.SetActive(true);
                firstTime = false;
                buttonText.text = "Close";
            }
            else
            {
                startingText.text = "To select a level, use the buttons indicated above, or click on the level. There is also an alternative challenge available once you defeat a level. You can access them by using the buttons above, or clicking and holding your current level.";
                secondPartText = true;
            }
        }
        else
        {
            detailsMenu.SetActive(false);
            allowInputs = true;
        }
    }
}
