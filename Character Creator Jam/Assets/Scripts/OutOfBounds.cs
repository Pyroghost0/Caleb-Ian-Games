/* Coded by Caleb Kahn
 * Qualms
 * Respawns player and other objects when hitting trigger
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public GameObject[] orderedEquipment;
    private Vector3[] orderedSpawnPosition = new Vector3[3];

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            orderedSpawnPosition[i] = orderedEquipment[i].transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().Respawn();
        }
        else if (other.CompareTag("Equipment"))
        {
            string equipment = other.GetComponent<Equipment>().clothingType;
            if (equipment == "Head")
            {
                other.gameObject.transform.position = orderedSpawnPosition[0];
            }
            else if (equipment == "Body")
            {
                other.gameObject.transform.position = orderedSpawnPosition[1];
            }
            else if (equipment == "Legs")
            {
                other.gameObject.transform.position = orderedSpawnPosition[2];
            }
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if (other.CompareTag("Slime"))
        {
            other.GetComponent<SlimeBehavior>().slimeSpawner.SlimeDeath();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Walker"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Health Kit"))
        {
            Destroy(other.gameObject);
        }
    }
}
