using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public GameObject[] buttons1;
    public GameObject[] buttons2;
    public bool selectCharacter = false;
    public bool selectCostume = false;
    public GameObject characterBlackPart;
    public GameObject costumeBlackPart;
    public GameObject text100;

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
            for (int i = 0; i < 25; i++)
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
    }
	public void StartGame()
    {
        selectCharacter = true;
        LoadLevel("Tutorial");
    }

    public void EnterLevelSelect()
    {
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
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void LoadLevel(string sceneName)
    {
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
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);

        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        player.LoadData(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<AudioManager>().ChangeScene("Dress Up Room");
 
        AsyncOperation ao2 = SceneManager.LoadSceneAsync("Dress Up Room", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao2.isDone);
        GameObject.FindGameObjectWithTag("Dress Up Door").GetComponent<DoorPortal>().nextSceneName = sceneName;

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    IEnumerator WaitLoadScene(string sceneName)
    {
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<AudioManager>().ChangeScene(sceneName);
        if (sceneName != "Tutorial")
        {
            player.LoadData(false);
        }
        AsyncOperation ao2 = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao2.isDone);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    public void BackToMainMenu()
    {
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
    }
}
