using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject[] enemySpawns;
    public float[] spawnWaitTimes;
    public GameObject winMessage;
    public TextMeshProUGUI resumeText;
    public TextMeshProUGUI pauseText;
    public int level = 1;
    public bool altLevel = false;
    public bool tutorialBefore = false;
    public bool tutorialAfter = false;
    public GameObject tutorialPopup;
    public int numGravesForLevelIfTutorial = 0;
    public InputManager inputManager;
    public TutorialManager tutorialManager;
    public GameObject startingCorpsesIfTutorial;

    void Start()
    {
        if (tutorialBefore)
        {
            StartCoroutine(TutorialPopupRoutine());
        }
        else
        {
            StartCoroutine(LevelSpawning());
            gameObject.SetActive(false);
        }
    }

    IEnumerator TutorialPopupRoutine()
    {
        yield return new WaitUntil(() => (Input.GetKey(inputManager.leftButton) || Input.GetKey(inputManager.middleButton) || Input.GetKey(inputManager.rightButton)));
        if (Input.GetKey(inputManager.leftButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.leftButton)));
            if (tutorialPopup.activeSelf)
            {
                PlayTutorial();
            }
        }
        else if (Input.GetKey(inputManager.middleButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.middleButton)));
            if (tutorialPopup.activeSelf)
            {
                SkipTutorial();
            }
        }
        else /*if (Input.GetKey(inputManager.rightButton))*/
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.rightButton)));
            if (tutorialPopup.activeSelf)
            {
                MainMenu();
            }
        }
    }

    public void StartLevel()
    {
        startingCorpsesIfTutorial.SetActive(true);
        GameObject.FindGameObjectWithTag("Input Manager").SetActive(true);
        GameObject.FindGameObjectWithTag("Player Base").SetActive(true);
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGraves(numGravesForLevelIfTutorial);
        StartCoroutine(LevelSpawning());
    }

    IEnumerator LevelSpawning()
    {
        for (int i = 0; i < enemySpawns.Length; i++)
        {
            yield return new WaitForSeconds(spawnWaitTimes[i]);
            enemySpawns[i].SetActive(true);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        while (enemies.Length != 0)
        {
            yield return new WaitForSeconds(.5f);
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        winMessage.SetActive(true);
        bool[] data = Data.Load();
        if (altLevel)
        {
            data[(level + 5) * 2] = true;//Completed
        }
        else
        {
            data[(level - 1) * 2] = true;//Completed
            data[(level * 2)+1] = true;//Next level available
            data[((level + 5) * 2)+1] = true;//Alt level available
        }
        Data.Save(data);
        if (tutorialAfter)
        {
            tutorialPopup.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(3f);
            inputManager.Pause();
            inputManager.allowResume = false;
            resumeText.text = "Play Again";
            pauseText.text = "You Win!";
        }
    }

    public void PlayTutorial()
    {
        tutorialPopup.SetActive(false);
        tutorialManager.StartTutorial();
    }

    public void SkipTutorial()
    {
        tutorialPopup.SetActive(false);
        if (tutorialBefore)
        {
            StartLevel();
        }
        else
        {
            inputManager.Pause();
            inputManager.allowResume = false;
            resumeText.text = "Play Again";
            pauseText.text = "You Win!";
        }
    }

    public void MainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        inputManager.loading.SetActive(true);
        Scene curentScene = SceneManager.GetActiveScene();
        AsyncOperation ao = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao.isDone);
        MainMenuManager mainMenuManager = GameObject.FindGameObjectWithTag("Main Menu Manager").GetComponent<MainMenuManager>();
        mainMenuManager.leftButton = inputManager.leftButton;
        mainMenuManager.middleButton = inputManager.middleButton;
        mainMenuManager.rightButton = inputManager.rightButton;
        mainMenuManager.leftButtonText.text = mainMenuManager.leftButton.ToString();
        mainMenuManager.middleButtonText.text = mainMenuManager.middleButton.ToString();
        mainMenuManager.rightButtonText.text = mainMenuManager.rightButton.ToString();
        SceneManager.UnloadSceneAsync(curentScene);
    }
}
