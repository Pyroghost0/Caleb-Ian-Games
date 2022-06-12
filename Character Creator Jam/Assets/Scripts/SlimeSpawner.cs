using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    public GameObject slimePrefab;
    public GameObject spawnPoint;
    public int numSlimes = 0;
    public int maxNumSlimes = 5;
    public float averageSlimeRespawnTime = 2f;
    public float spawnDistence = 50f;
    private GameObject player;
    private bool currentlySpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        StartCoroutine(SpawnSlimes());
    }

    public void SlimeDeath()
    {
        numSlimes--;
        if (!currentlySpawning)
        {
            StartCoroutine(SpawnSlimes());
        }
    }

    IEnumerator SpawnSlimes()
    {
        currentlySpawning = true;
        while (numSlimes < maxNumSlimes)
        {
            yield return new WaitForSeconds(Random.Range(averageSlimeRespawnTime / 4, averageSlimeRespawnTime * 3 / 4));
            if (Mathf.Abs((player.transform.position - transform.position).magnitude) < spawnDistence)
            {
                GameObject slime = Instantiate(slimePrefab, spawnPoint.transform.position, slimePrefab.transform.rotation);
                slime.GetComponent<SlimeBehavior>().slimeSpawner = this;
                numSlimes++;
            }
            yield return new WaitForSeconds(Random.Range(averageSlimeRespawnTime / 4, averageSlimeRespawnTime * 3 / 4));
        }
        currentlySpawning = false;
    }
}
