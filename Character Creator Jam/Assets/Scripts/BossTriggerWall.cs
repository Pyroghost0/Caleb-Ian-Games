using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerWall : MonoBehaviour
{
    public GameObject wall;
    private bool triggerOnce = false;
    public AudioSource audio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerOnce)
        {
            StartCoroutine(WaitDestroy());
        }
    }
    IEnumerator WaitDestroy()
    {
        audio.Play();
        yield return new WaitForSeconds(1.5f);
        triggerOnce = true;
        wall.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        GameObject.FindGameObjectWithTag("Boss").GetComponent<BossMonologue>().StartIntro();

        Destroy(gameObject);
    }
}
