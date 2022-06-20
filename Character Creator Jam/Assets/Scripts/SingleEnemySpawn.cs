using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject activeEnemy;

    public void RespawnEnemy()
    {
        Destroy(activeEnemy);
        GameObject enemyAdded = Instantiate(enemyPrefab, transform.position, enemyPrefab.transform.rotation);
        activeEnemy = enemyAdded;
        /*WalkerBehavior walker = enemyAdded.GetComponent<WalkerBehavior>();
        if (walker != null)
        {
            walker.walkerSpawn = gameObject;
        }*/
    }
}
