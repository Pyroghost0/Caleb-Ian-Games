using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerWalls : MonoBehaviour
{
    public bool isSlimeTriggerWall = false;
    public SlimeSpawner slimeSpawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSlimeTriggerWall)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerStatus>().invincible = true;
                slimeSpawner.enabled = true;
            }
            else if (other.CompareTag("Slime"))
            {
                StartCoroutine(KillSlimeInSeconds(other.gameObject, 2.5f));
            }
        }
    }

    IEnumerator KillSlimeInSeconds(GameObject slime, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (slime != null)
        {
            slimeSpawner.SlimeDeath();
            Destroy(slime);
        }
    }
}
