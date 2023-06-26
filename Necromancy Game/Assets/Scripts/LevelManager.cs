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
    public TextMeshProUGUI specialResumeText;
    public TextMeshProUGUI specialMainText;
    public TextMeshProUGUI resumeText;
    public TextMeshProUGUI pauseText;
    public int level = 1;
    public bool altLevel = false;
    public bool tutorialBefore = false;
    public GameObject tutorialPopup;
    public GameObject specialStartPopup;
    public GameObject newSpecialPopup;
    public int numGravesForLevelIfTutorial = 0;
    public InputManager inputManager;
    public TutorialManager tutorialManager;
    public bool defeatedNewLevel = false;
    public bool minionAttack = false;

    public GameObject[] SkeletonPrefabsIfSpecial3;
    public GameObject startingCorpsesIfTutorial;
    public GameObject doubleButtonsForLevel1;
    public GameObject skeletonStatusForLevel1;

    void Start()
    {
        if (altLevel)
        {
            StartCoroutine(SpecialLevelPopupRoutine());
        }
        else if (tutorialBefore && ((level == 1 && !Data.Load()[0]) || (level == 2 && !Data.Load()[2])))
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
                    skeletonStatusForLevel1.SetActive(true);
                }
                startingCorpsesIfTutorial.SetActive(true);
                inputManager.enabled = true;
                GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = true;
                GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGraves(numGravesForLevelIfTutorial);
            }
            StartCoroutine(LevelSpawning());
        }
    }

    IEnumerator SpecialLevelPopupRoutine()
    {
        yield return new WaitUntil(() => (!Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton)));
        yield return new WaitUntil(() => (Input.GetKey(inputManager.leftButton) || Input.GetKey(inputManager.middleButton) || Input.GetKey(inputManager.rightButton)));
        if (Input.GetKey(inputManager.leftButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.leftButton)));
            if (specialStartPopup.activeSelf)
            {
                StartSpecialLevel();
            }
        }
        else if (Input.GetKey(inputManager.middleButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.middleButton)));
            if (specialStartPopup.activeSelf)
            {
                MainMenu();
            }
        }
        else /*if (Input.GetKey(inputManager.rightButton))*/
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.rightButton)));
            if (specialStartPopup.activeSelf)
            {
                QuiGame();
            }
        }
    }

    IEnumerator NewSpecialPopupRoutine()
    {
        yield return new WaitUntil(() => (!Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton)));
        yield return new WaitUntil(() => (Input.GetKey(inputManager.leftButton) || Input.GetKey(inputManager.middleButton) || Input.GetKey(inputManager.rightButton)));
        if (Input.GetKey(inputManager.leftButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.leftButton)));
            if (newSpecialPopup.activeSelf)
            {
                PlayAgain();
            }
        }
        else if (Input.GetKey(inputManager.middleButton))
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.middleButton)));
            if (newSpecialPopup.activeSelf)
            {
                MainMenu();
            }
        }
        else /*if (Input.GetKey(inputManager.rightButton))*/
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(inputManager.rightButton)));
            if (newSpecialPopup.activeSelf)
            {
                QuiGame();
            }
        }
    }

    IEnumerator TutorialPopupRoutine()
    {
        yield return new WaitUntil(() => (!Input.GetKey(inputManager.leftButton) && !Input.GetKey(inputManager.middleButton) && !Input.GetKey(inputManager.rightButton)));
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
        if (level == 1 && Data.Load()[0])
        {
            doubleButtonsForLevel1.SetActive(true);
            skeletonStatusForLevel1.SetActive(true);
        }
        startingCorpsesIfTutorial.SetActive(true);
        inputManager.enabled = true;
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = true;
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGraves(numGravesForLevelIfTutorial);
        StartCoroutine(LevelSpawning());
    }

    IEnumerator LevelSpawning()
    {
        if (minionAttack)
        {
            inputManager.selectManager.currentMinionDigStatus = false;
            inputManager.selectManager.minionStatus.sprite = inputManager.selectManager.minionAttack;
        }
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
        if (!data[(level - 1) * 2])
        {
            defeatedNewLevel = true;
        }
        data[(level - 1) * 2] = true;//Completed
        data[(level * 2) + 1] = true;//Next level available
        data[((level + 5) * 2) + 1] = true;//Alt level available
        Data.Save(data);
        yield return new WaitForSeconds(3f);
        inputManager.Pause();
        inputManager.allowResume = false;
        resumeText.text = "Play Again";
        pauseText.text = "You Win!";
    }

    IEnumerator SpecialLevel1()
    {//Survive with one left
        if (Data.Load()[2])
        {
            doubleButtonsForLevel1.SetActive(true);
            skeletonStatusForLevel1.SetActive(true);
        }
        yield return new WaitForSeconds(spawnWaitTimes[0]);
        enemySpawns[0].SetActive(true);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        while (enemies.Length != 0)
        {
            yield return new WaitForSeconds(.5f);
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        if (GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().numSkeletons == 1)
        {
            StartCoroutine(WinSpecialLevel());
        }
        else
        {
            inputManager.Pause();
            inputManager.allowResume = false;
            resumeText.text = "Retry";
            pauseText.text = "You Failed...";
            GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().timeSurvived = 0f;
        }
    }

    IEnumerator SpecialLevel2()
    {//Mine all tombstones
        for (int i = 0; i < enemySpawns.Length; i++)
        {
            yield return new WaitForSeconds(spawnWaitTimes[i] / 5f);
            if (Special2Complete())
            {
                break;
            }
            yield return new WaitForSeconds(spawnWaitTimes[i] / 5f);
            if (Special2Complete())
            {
                break;
            }
            yield return new WaitForSeconds(spawnWaitTimes[i] / 5f);
            if (Special2Complete())
            {
                break;
            }
            yield return new WaitForSeconds(spawnWaitTimes[i] / 5f);
            if (Special2Complete())
            {
                break;
            }
            yield return new WaitForSeconds(spawnWaitTimes[i] / 5f);
            if (Special2Complete())
            {
                break;
            }
            enemySpawns[i].SetActive(true);
        }
        yield return new WaitUntil(() => (Special2Complete()));
        StartCoroutine(WinSpecialLevel());
    }

    private bool Special2Complete()
    {
        GameObject[] graves = GameObject.FindGameObjectsWithTag("Grave");
        foreach (GameObject grave in graves)
        {
            if (!grave.GetComponent<Grave>().coffin)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpecialLevel3()
    {//Random corpses, defeat all
        StartCoroutine(AllRandom());
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
        StartCoroutine(WinSpecialLevel());
    }

    IEnumerator AllRandom()
    {
        while (true)
        {
            GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
            foreach (GameObject corpse in corpses)
            {
                corpse.GetComponent<Corpse>().skeletonPrefab = SkeletonPrefabsIfSpecial3[Random.Range(0, 6)];
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SpecialLevel4()
    {//Invincible skeletons defeat level in cerain time
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        playerBase.enabled = false;
        yield return new WaitUntil(() => (playerBase.numSkeletons == 1));
        float acceloration;
        GameObject skeleton1 = GameObject.FindGameObjectWithTag("Skeleton");
        bool isSkeleton = false;
        if (GameObject.FindGameObjectWithTag("Skeleton") != null)
        {
            skeleton1.GetComponent<Skeleton>().defence = 200;
            acceloration = skeleton1.GetComponent<Skeleton>().speedAcceleration;
            skeleton1.GetComponent<Skeleton>().speedAcceleration = 0f;
            isSkeleton = true;
        }
        else
        {
            skeleton1.GetComponent<Minion>().defence = 200;
            acceloration = skeleton1.GetComponent<Minion>().speedAcceleration;
            skeleton1.GetComponent<Minion>().speedAcceleration = 0f;
        }
        yield return new WaitUntil(() => (playerBase.numSkeletons == 2));
        if (isSkeleton)
        {
            if (GameObject.FindGameObjectsWithTag("Skeleton").Length == 2)
            {
                GameObject skeleton2 = GameObject.FindGameObjectsWithTag("Skeleton")[0] == skeleton1 ? GameObject.FindGameObjectsWithTag("Skeleton")[1] : GameObject.FindGameObjectsWithTag("Skeleton")[0];
                skeleton2.GetComponent<Skeleton>().defence = 200;
                skeleton1.GetComponent<Skeleton>().speedAcceleration = acceloration;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Minion").GetComponent<Minion>().defence = 200;
                skeleton1.GetComponent<Skeleton>().speedAcceleration = acceloration;
            }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Skeleton") != null)
            {
                GameObject.FindGameObjectWithTag("Skeleton").GetComponent<Skeleton>().defence = 200;
                skeleton1.GetComponent<Minion>().speedAcceleration = acceloration;
            }
            else
            {
                GameObject skeleton2 = GameObject.FindGameObjectsWithTag("Minion")[0] == skeleton1 ? GameObject.FindGameObjectsWithTag("Minion")[1] : GameObject.FindGameObjectsWithTag("Minion")[0];
                skeleton2.GetComponent<Minion>().defence = 200;
                skeleton1.GetComponent<Minion>().speedAcceleration = acceloration;
            }
        }
        playerBase.enabled = true;
        enemySpawns[0].SetActive(true);
        bool won = false;
        for (int i = 0; i < 120; i++)
        {
            yield return new WaitForSeconds(.5f);
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                won = true;
                break;
            }
        }
        if (won)
        {
            StartCoroutine(WinSpecialLevel());
        }
        else
        {
            inputManager.Pause();
            inputManager.allowResume = false;
            resumeText.text = "Retry";
            pauseText.text = "You Failed...";
            GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().timeSurvived = 0f;
        }
    }

    IEnumerator SpecialLevel5()
    {//No skeletons die part
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        int previousNumSkeletons = playerBase.numSkeletons;
        //GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        //GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        Coroutine coroutine = StartCoroutine(SpecialLevel3());//Level 3 is just a normal spawning coroutine
        while (!winMessage.activeSelf)
        {
            yield return new WaitForEndOfFrame();
            if (playerBase.numSkeletons > previousNumSkeletons)
            {
                previousNumSkeletons++;
            }
            else if (playerBase.numSkeletons < previousNumSkeletons)
            {
                StopCoroutine(coroutine);
                inputManager.Pause();
                inputManager.allowResume = false;
                resumeText.text = "Retry";
                pauseText.text = "You Failed...";
                GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().timeSurvived = 0f;
            }
        }
    }

    IEnumerator SpecialLevel6()
    {//Skeletons slowly take damage
        Coroutine coroutine = StartCoroutine(SpecialLevel3());//Level 3 is just a normal spawning coroutine
        while (!winMessage.activeSelf)
        {
            yield return new WaitForSeconds(.5f);
            GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
            GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
            for (int i = 0; i < skeletons.Length; i++)
            {
                skeletons[i].GetComponent<Skeleton>().Hit(Vector3.zero, null, 0f, 1);
            }
            for (int i = 0; i < minions.Length; i++)
            {
                minions[i].GetComponent<Minion>().Hit(Vector3.zero, null, 0f, 1);
            }
        }
    }

    IEnumerator WinSpecialLevel()
    {
        winMessage.SetActive(true);
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = false;
        bool[] data = Data.Load();
        bool altLevelSpecialTutorial = !data[12] && !data[14] && !data[16] && !data[18] && !data[20] && level != 6;
        bool newSpecial = !data[(level + 5) * 2] && level != 6;
        data[(level + 5) * 2] = true;//Completed
        Data.Save(data);
        if (altLevelSpecialTutorial)
        {
            yield return new WaitForSeconds(1f);
            tutorialPopup.SetActive(true);
        }
        else if (newSpecial)
        {
            yield return new WaitForSeconds(1f);
            specialStartPopup.SetActive(true);
            inputManager.enabled = false;
            specialResumeText.text = "Play Again";
            specialMainText.text = "Congradulations! You unlocked the " + (level == 1 ? "Goblin" : level == 1 ? "Wolf" : level == 1 ? "Witch" : level == 1 ? "Orc" : "Ogre") + " special ability\n"
                + (level == 1 ? "Goblin shoots 3 arrows." : level == 1 ? "Wolf teleports to enemy closest to your base." : level == 1 ? "Witch gravitates enemies together." : level == 1 ? "Orc increases defences in exchange for the closest tombstone." : "Ogre hits all nearby enemies.");
            StartCoroutine(NewSpecialPopupRoutine());
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

    public void StartSpecialLevel()
    {
        specialStartPopup.SetActive(false);
        inputManager.enabled = true;
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().enabled = true;
        if (level == 1)
        {
            StartCoroutine(SpecialLevel1());
        }
        else if (level == 2)
        {
            StartCoroutine(SpecialLevel2());
        }
        else if (level == 3)
        {
            StartCoroutine(SpecialLevel3());
        }
        else if (level == 4)
        {
            StartCoroutine(SpecialLevel4());
        }
        else if (level == 5)
        {
            StartCoroutine(SpecialLevel5());
        }
        else /*if (level == 6)*/
        {
            StartCoroutine(SpecialLevel6());
        }
    }

    public void PlayAgain()
    {
        inputManager.enabled = true;
        inputManager.allowResume = false;
        inputManager.Resume();
    }

    public void PlayTutorial()
    {
        tutorialPopup.SetActive(false);
        winMessage.SetActive(false);
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

    public void QuiGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
