using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveAssistant : MonoBehaviour
{
    public GraveManager graveManager;

    // Start is called before the first frame update
    /*void Start()
    {
        graveManager = GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>();
    }*/

    public void SpawnGrave()
    {
        int prefabNum = Random.Range(0, graveManager.gravePrefabs.Length);
        GameObject grave = Instantiate(graveManager.gravePrefabs[prefabNum], transform);
        grave.GetComponent<Grave>().graveAssistant = this;
    }
}
