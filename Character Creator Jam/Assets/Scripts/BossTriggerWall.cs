using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerWall : MonoBehaviour
{
    public GameObject wall;
    private bool triggerOnce = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerOnce)
        {
            triggerOnce = true;
            wall.SetActive(true);
            GameObject.FindGameObjectWithTag("Boss").GetComponent<BossBehavior>().StartFight();
            Destroy(gameObject);
        }
    }
}
