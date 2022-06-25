using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float power;
    private float lifeTime = 3f;
    public Vector3 movement;
    public bool bossBullet = false;

    // Start is called before the first frame update
    void Start()
    {
        power = transform.localScale.x / .75f;
        lifeTime *= power;
        if (!bossBullet)
        {
            movement = Vector3.forward * power * 75f;
        }
        else
        {
            movement = Vector3.forward * 75f;
        }
        StartCoroutine(DieInSeconds(lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(movement * Time.deltaTime);
    }

    IEnumerator DieInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bossBullet)
        {
            other.GetComponent<PlayerStatus>().TakeDamage(30f, transform.position, 50f);
        }
    }
}
