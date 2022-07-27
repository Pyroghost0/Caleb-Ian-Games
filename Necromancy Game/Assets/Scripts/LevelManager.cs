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
        yield return new WaitForSeconds(1f);
        InputManager inputManager = GameObject.FindGameObjectWithTag("Input Manager").GetComponent<InputManager>();
        inputManager.Pause();
        inputManager.allowResume = false;
        resumeText.text = "Play Again";
        pauseText.text = "You Win!";
    }
}
