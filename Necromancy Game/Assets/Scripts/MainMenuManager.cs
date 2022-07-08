using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    /*public GameObject[] buttons1;
    public GameObject[] buttons2;
    public bool selectCharacter = false;
    public bool selectCostume = false;
    public GameObject characterBlackPart;
    public GameObject costumeBlackPart;
    public GameObject text100;
    public GameObject removeMenu;
    public GameObject unlockMenu;
    public GameObject loading;

    private bool isMainScreen = true;
    private int[] data;
    private bool isData;
    private bool fullCompletion = false;
    public void Start()
    {
        data = SaveLoad.Load();
        if (data == null)
        {
            isData = false;
            data = new int[25];
            data[0] = 0;
            data[1] = 1;
            data[2] = 0;
            data[3] = 0;
            for (int i = 4; i < 25; i++)
            {
                data[i] = 0;
            }
        }
        else
        {
            isData = true;
            fullCompletion = true;
            for (int i = 4; i < data.Length; i++)
            {
                if (data[i] < 1)
                {
                    fullCompletion = false;
                }
            }
            if (fullCompletion == true)
            {
                text100.SetActive(true);
            }
        }
    }*/

    public void Update()
    {
        /*if (isMainScreen && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Escape))
        {
            if (Input.GetKey(KeyCode.Backspace))
            {
                removeMenu.SetActive(true);
                unlockMenu.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.Return))
            {
                unlockMenu.SetActive(true);
                removeMenu.SetActive(false);
            }
        }*/
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Endless Mode");
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
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
