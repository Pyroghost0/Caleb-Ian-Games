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
    public GameObject tutorialPopup;
    public int numGravesForLevelIfTutorial = 0;
    public InputManager inputManager;
    public TutorialManager tutorialManager;
    public bool defeatedNewLevel = false;

    public GameObject startingCorpsesIfTutorial;
    public GameObject doubleButtonsForLevel1;

    void Start()
    {
        if (tutorialBefore && ((level == 1 && !Data.Load()[0]) || (level == 2 && !Data.Load()[2])))
        {
            StartCoroutine(TutorialPopupRoutine());
        }
        else
        {
            if (tutorialBefore)
            {
                tutorialPopup.SetActive(false);
                if (level == 1 && Data.Load()[0])
                {
                    doubleButtonsForLevel1.SetActive(true);
                }
                startingCorpsesIfTutorial.SetActive(true);
                inputManager.enabled = true;
                GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = true;
                GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGraves(numGravesForLevelIfTutorial);
            }
            StartCoroutine(LevelSpawning());
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
        if (level == 1 && Data.Load()[0] && !altLevel)
        {
            doubleButtonsForLevel1.SetActive(true);
        }
        else if (level == 1 && Data.Load()[2] && altLevel)
        {
            doubleButtonsForLevel1.SetActive(true);
        }
        startingCorpsesIfTutorial.SetActive(true);
        inputManager.enabled = true;
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = true;
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
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = false;
        bool[] data = Data.Load();
        bool altLevelSpecialTutorial = false;
        if (altLevel)
        {
            altLevelSpecialTutorial = level != 6 && !data[12] && !data[14] && !data[16] && !data[18] && !data[20];
            data[(level + 5) * 2] = true;//Completed
        }
        else
        {
            if (!data[(level - 1) * 2])
            {
                defeatedNewLevel = true;
            }
            data[(level - 1) * 2] = true;//Completed
            data[(level * 2)+1] = true;//Next level available
            data[((level + 5) * 2)+1] = true;//Alt level available
        }
        Data.Save(data);
        if (altLevelSpecialTutorial)
        {
            yield return new WaitForSeconds(1f);
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
        inputManager.enabled = true;
        inputManager.MainMenu();
    }
}
