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
    public int maxSlimesPerCycle = -1;
    private GameObject player;
    private AudioManager audioManager;
    public int slimesSpawned = 0;
    private bool currentlySpawning = false;
    [SerializeField] private Material[] materials;
    private int slimeColor;
    private int playerSkinColor;
    private int playerHairColor;

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        StartCoroutine(SpawnSlimes());
    }
    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        if (player)
        {
            audioManager = player.GetComponent<AudioManager>();
            playerSkinColor = player.GetComponent<PlayerStatus>().skinColor;
            playerHairColor = player.GetComponent<PlayerStatus>().hairColor;
            materials = new Material[12];
            int offset = 0;
            for (int i = 0; i < materials.Length / 2; i++)
            {
                if (i == playerHairColor) offset = 1;
                materials[i] = (Material)Resources.Load("Hair " + (i + offset));
            }
            offset = 0;
            for (int i = 0; i < materials.Length / 2; i++)
            {
                if (i == playerSkinColor) offset = 1;
                materials[i + materials.Length / 2] = (Material)Resources.Load("Skin " + (i + offset));
            }
        }
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
        while (numSlimes < maxNumSlimes && (maxSlimesPerCycle == -1 || slimesSpawned < maxSlimesPerCycle))
        {
            if (player)
			{
                yield return new WaitForSeconds(Random.Range(averageSlimeRespawnTime / 4, averageSlimeRespawnTime * 3 / 4));
                if (Mathf.Abs((player.transform.position - transform.position).magnitude) < spawnDistence)
                {
                    slimesSpawned++;
                    SpawnSlime();
                }
			}
			else
			{
                FindPlayer();
			}
            yield return new WaitForSeconds(Random.Range(averageSlimeRespawnTime / 4, averageSlimeRespawnTime * 3 / 4));
        }
        currentlySpawning = false;
    }

    public void restartPipe()
    {
        slimesSpawned = 0;
        StopAllCoroutines();
        StartCoroutine(SpawnSlimes());
    }

    public GameObject SpawnSlime()
    {
        slimeColor = Random.Range(0, materials.Length);
        GameObject slime = Instantiate(slimePrefab, spawnPoint.transform.position, slimePrefab.transform.rotation);
        slime.GetComponent<SlimeBehavior>().slimeSpawner = this;
        slime.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = materials[slimeColor];
        slime.GetComponent<SlimeBehavior>().slimeColor = slimeColor;
        numSlimes++;
		audioManager.SpawnSlime();
        return slime;
    }
}
