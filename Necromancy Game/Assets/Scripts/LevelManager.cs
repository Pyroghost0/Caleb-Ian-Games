using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        yield return new WaitForSeconds(3f);
        InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
        inputManager.Pause();
        inputManager.allowResume = false;
        resumeText.text = "Play Again";
        pauseText.text = "You Win!";
    }
}
