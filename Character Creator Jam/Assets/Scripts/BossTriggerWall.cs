using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerWall : MonoBehaviour
{
    public GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.SetActive(true);
            Destroy(gameObject);
        }
    }
}
