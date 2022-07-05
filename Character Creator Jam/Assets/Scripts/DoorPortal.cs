using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorPortal : MonoBehaviour
{
    public string nextSceneName;
    public bool isDressUpDoor;
    public string currentSceneName;
    private bool loaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !loaded)
        {
            loaded = true;
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

            other.GetComponent<PlayerMovement>().groundChecker.inGround = false;
            
            if (!isDressUpDoor)
			{
                other.GetComponent<AudioManager>().ChangeScene("Dress Up Room");
                other.GetComponent<PlayerStatus>().CompletedLevel(currentSceneName);
                StartCoroutine(LoadDressUpRoom());
			}
			else
			{
                other.GetComponent<AudioManager>().ChangeScene(nextSceneName);
                StartCoroutine(LoadLevel());
            }
            
        }
    }
    private IEnumerator LoadDressUpRoom()
	{
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Dress Up Room", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        GameObject.FindGameObjectWithTag("Dress Up Door").GetComponent<DoorPortal>().nextSceneName = nextSceneName;
        SceneManager.UnloadSceneAsync(currentSceneName);
    }
    private IEnumerator LoadLevel()
	{
        AsyncOperation ao1 = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        SceneManager.UnloadSceneAsync("Dress Up Room");
    }
}
