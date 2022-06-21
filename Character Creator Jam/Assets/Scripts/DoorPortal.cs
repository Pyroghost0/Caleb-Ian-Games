using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorPortal : MonoBehaviour
{
    public string nextSceneName;
    public bool isDressUpDoor;
    public List<Scene> scenes;
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
            if (!isDressUpDoor)
			{
                StartCoroutine(LoadDressUpRoom());
			}
			else
			{
                SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("Dress Up Room");
            }
            
        }
    }
    private IEnumerator LoadDressUpRoom()
	{
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Dress Up Room", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        GameObject.FindGameObjectWithTag("Dress Up Door").GetComponent<DoorPortal>().nextSceneName = nextSceneName;

        if (scenes[0].isLoaded)
            SceneManager.UnloadSceneAsync("Mech Level");
        else if (scenes[1].isLoaded)
            SceneManager.UnloadSceneAsync("Deer Level");
        else if (scenes[2].isLoaded)
            SceneManager.UnloadSceneAsync("Baker Level");
        else if (scenes[3].isLoaded)
            SceneManager.UnloadSceneAsync("POTUS Level");
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(currentSceneName);
        }
    }
}
