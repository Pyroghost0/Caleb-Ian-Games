using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private short[] enemyValue;
    public Transform[] formation;
    public short startingValue = 5;
    public float timerToValueRatio = .3f;

    public Transform[] startingCorpsePositions;
    public PlayerBase playerBase;
    private float timeSinceStart;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startingCorpsePositions.Length; i++)
        {
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)].GetComponent<Enemy>().corpsePrefab, startingCorpsePositions[i]);
        }
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            for (int j = i + 1; j < enemyPrefabs.Length; j++)
            {
                if (enemyPrefabs[i].GetComponent<Enemy>().enemyValue > enemyPrefabs[j].GetComponent<Enemy>().enemyValue)
                {
                    GameObject temp = enemyPrefabs[i];
                    enemyPrefabs[i] = enemyPrefabs[j];
                    enemyPrefabs[j] = temp;
                }
            }
        }
        enemyValue = new short[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            enemyValue[i] = enemyPrefabs[i].GetComponent<Enemy>().enemyValue;
        }
        StartCoroutine(EndlessSpawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator EndlessSpawn()
    {
        while (playerBase.health > 0)
        {
            //Spawn

            //Add as many random enemies into the formation
            int value = (int)(timeSinceStart * timerToValueRatio) + startingValue;
            List<int> enemyNumbers = new List<int>();
            for (int i = 0; i < formation.Length; i++)
            {
                int maxPrefabNumber = -1;
                for (int j = 0; j < enemyPrefabs.Length; j++)
                {
                    if (value >= enemyValue[j])
                    {
                        maxPrefabNumber++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (maxPrefabNumber == -1)
                {
                    break;
                }
                else
                {
                    int randNum = Random.Range(0, maxPrefabNumber + 1);
                    enemyNumbers.Add(randNum);
                    value -= enemyValue[randNum];
                }
            }
            //Sort values from lowest to highest
            for (int i = 0; i < enemyNumbers.Count; i++)
            {
                for (int j = i + 1; j < enemyNumbers.Count; j++)
                {
                    if (enemyPrefabs[enemyNumbers[i]].GetComponent<Enemy>().enemyValue > enemyPrefabs[enemyNumbers[j]].GetComponent<Enemy>().enemyValue)
                    {
                        int temp = enemyNumbers[i];
                        enemyNumbers[i] = enemyNumbers[j];
                        enemyNumbers[j] = temp;
                    }
                }
            }
            //If enemy can have higher value, upgrade it
            int tempValue = 0;//Tests if nothing changes
            while (tempValue != value && value != 0)
            {
                tempValue = value;
                for (int i = 0; i < enemyNumbers.Count; i++)
                {//For each enemy
                    for (int j = enemyNumbers[i]+1; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyValue[enemyNumbers[i]] < enemyValue[j])
                        {//Find the next enemy with a higher value
                            if (value + enemyValue[enemyNumbers[i]] >= enemyValue[j])
                            {
                                value -= enemyValue[j] - enemyValue[enemyNumbers[i]];
                                int futureEnemyNumMin = j;
                                int futureEnemyNumMax = j;
                                for (int k = j+1; k < enemyPrefabs.Length; k++)
                                {//Choose a random enemy with a higher value
                                    if (enemyValue[futureEnemyNumMin] == enemyValue[k])
                                    {
                                        futureEnemyNumMax++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                enemyNumbers[i] = Random.Range(futureEnemyNumMin, futureEnemyNumMax + 1);
                            }
                            break;
                        }
                    }
                }
            }
            //*TEMP* sort values from highest to lowest for formation *TEMP*
            for (int i = 0; i < enemyNumbers.Count; i++)
            {
                for (int j = i + 1; j < enemyNumbers.Count; j++)
                {
                    if (enemyPrefabs[enemyNumbers[i]].GetComponent<Enemy>().enemyValue < enemyPrefabs[enemyNumbers[j]].GetComponent<Enemy>().enemyValue)
                    {
                        int temp = enemyNumbers[i];
                        enemyNumbers[i] = enemyNumbers[j];
                        enemyNumbers[j] = temp;
                    }
                }
            }
            //Spawn enemies into the formation
            for (int i = 0; i < enemyNumbers.Count; i++)
            {
                Instantiate(enemyPrefabs[enemyNumbers[i]], formation[i].position, enemyPrefabs[enemyNumbers[i]].transform.rotation);
            }

            //Wait
            float timer = 0f;
            float waitTime = timeSinceStart + (value * 3 / timerToValueRatio);
            waitTime = Random.Range(7f * Mathf.Pow(1.008f, -1.5f) + 1f, 10.5f * Mathf.Pow(1.006f, -1f) + 1.5f);
            while (timer < waitTime)
            {
                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            timeSinceStart += waitTime;
        }
    }
}
