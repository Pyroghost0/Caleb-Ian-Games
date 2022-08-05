using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveManager : MonoBehaviour
{
    public List<Transform> graveAssistants = new List<Transform>();
    public GameObject[] gravePrefabs;
    public GameObject[] coffinPrefabs;
    public short startGraveAmount = 6;

    // Start is called before the first frame update
    void Start()
    {
        SpawnGraves(startGraveAmount);
    }

    public void SpawnGraves(int numGravesAdded)
    {
        for (int i = 0; i < numGravesAdded && graveAssistants.Count > 0; i++)
        {
            int graveNum = Random.Range(0, graveAssistants.Count);
            int prefabNum = Random.Range(0, gravePrefabs.Length);
            GameObject grave = Instantiate(gravePrefabs[prefabNum], graveAssistants[graveNum]);
            grave.GetComponent<Grave>().graveAssistant = graveAssistants[graveNum];
            graveAssistants.RemoveAt(graveNum);
        }
    }

    public void SpawnCoffin(short customBoneAmount, Vector3 position)
    {
        int prefabNum = Random.Range(0, coffinPrefabs.Length);
        GameObject coffin = Instantiate(coffinPrefabs[prefabNum], position, transform.rotation);
        coffin.GetComponent<Grave>().bones = customBoneAmount;
    }
}
