/* Coded by Caleb Kahn
 * Qualms
 * Manages enemies by respawning them if they die
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject activeEnemy;
    public float distence = 70f;
    public bool respawnIfDestroyed = true;
    private bool isWalker = false;
    private bool isFlyer = false;

    void Start()
    {
        if (activeEnemy.CompareTag("Walker"))
        {
            isWalker = true;
            activeEnemy.GetComponent<WalkerBehavior>().seesPlayerDistence = distence;
            activeEnemy.GetComponent<WalkerBehavior>().maxDistenceFromPlayer = distence+10f;
        }
        else if (activeEnemy.CompareTag("Flyer"))
        {
            isFlyer = true;
            activeEnemy.GetComponent<FlyerBehavior>().seesPlayerDistence = distence;
            activeEnemy.GetComponent<FlyerBehavior>().maxDistenceFromPlayer = distence + 10f;
        }
    }

    public void RespawnEnemy()
    {
        if (respawnIfDestroyed || (activeEnemy.gameObject != null))
        {
            Destroy(activeEnemy);
            GameObject enemyAdded = Instantiate(enemyPrefab, transform.position, enemyPrefab.transform.rotation);
            if (isWalker)
            {
                enemyAdded.GetComponent<WalkerBehavior>().seesPlayerDistence = distence;
                enemyAdded.GetComponent<WalkerBehavior>().maxDistenceFromPlayer = distence + 10f;
            }
            else if (isFlyer)
            {
                enemyAdded.GetComponent<FlyerBehavior>().seesPlayerDistence = distence;
                enemyAdded.GetComponent<FlyerBehavior>().maxDistenceFromPlayer = distence + 10f;
            }
            activeEnemy = enemyAdded;
        }
    }
}
