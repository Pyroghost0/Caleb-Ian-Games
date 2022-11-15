/* Coded by Caleb Kahn
 * Qualms
 * Saves checkpoint to respawn to
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().currentSpawnPosition = checkpoint;
        }
    }
}
