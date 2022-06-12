using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerWalls : MonoBehaviour
{
    public bool isSlimeTriggerWall = false;
    public SlimeSpawner slimeSpawner;

    public bool isTargetObject = false;
    private bool gateMoved = false;
    public GameObject gateWall;

    public bool isFinalTriggerWall = false;
    public GameObject ground;

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

        else if (isTargetObject)
        {
            if (other.CompareTag("Bullet") && !gateMoved)
            {
                gateMoved = true;
                StartCoroutine(MoveGateUp());
                Destroy(other.gameObject);
            }
        }

        else if (isFinalTriggerWall)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerStatus>().invincible = false;
                other.GetComponent<PlayerStatus>().invincibleTime = .25f;
                other.GetComponent<PlayerMovement>().groundChecker.inGround = false;
                ground.SetActive(false);
            }
        }
    }

    IEnumerator MoveGateUp()
    {
        float timer = 0f;
        while (timer < 2.5f)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            gateWall.transform.position += Vector3.up * 8f / 2.5f *Time.deltaTime;
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
