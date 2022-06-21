using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorPortal : MonoBehaviour
{
    public string nextSceneName;
    public string currentSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
            for (int i = 0; i < slimes.Length; i++)
            {
                Destroy(slimes[i]);
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Single Enemy Spawner");
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i].GetComponent<SingleEnemySpawn>().activeEnemy);
            }
            GameObject[] equipment = GameObject.FindGameObjectsWithTag("Equipment");
            for (int i = 0; i < equipment.Length; i++)
            {
                Destroy(equipment[i]);
            }
            other.gameObject.transform.position = Vector3.zero;
            other.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(currentSceneName);
        }
    }
}
