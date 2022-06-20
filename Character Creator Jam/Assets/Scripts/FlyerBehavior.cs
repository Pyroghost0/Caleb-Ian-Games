﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerBehavior : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    private PlayerManager playerManager;
    public GameObject player;
    private PlayerStatus playerStatus;
    public Transform truePosition;
    private Vector3 forwardDirection;

    public List<GameObject> faces;
    public float maxDistenceFromPlayer = 80f;
    public float seesPlayerDistence = 70f;
    private bool seesPlayer = false;

    public float damage = 25f;
    public float knockback = 15f;
    public float health = 20f;
    public float movementSpeed = 5f;
    public float turnSpeed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        faces[0].SetActive(false);
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        player = playerManager.player;
        playerStatus = player.GetComponent<PlayerStatus>();
        forwardDirection = new Vector3(Random.Range(-.5f, .5f), Random.Range(.1f, .3f), 1f).normalized;
        faces.RemoveAt(player.GetComponent<PlayerStatus>().isMale ? player.GetComponent<PlayerStatus>().headNumber + 3 : player.GetComponent<PlayerStatus>().headNumber);
        faces[Random.Range(0, faces.Count)].SetActive(true);
        //maxDistenceFromPlayer = slimeSpawner.spawnDistence;
        //StartCoroutine(WalkTowardPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (seesPlayer)
        {
            if ((truePosition.position - player.transform.position).magnitude < maxDistenceFromPlayer)
            {
                Vector3 direction = player.transform.position - truePosition.position;
                direction = direction.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);

                transform.Translate(forwardDirection * movementSpeed * Time.deltaTime);
            }
            else
            {
                seesPlayer = false;
            }
        }
        else if ((truePosition.position - player.transform.position).magnitude < seesPlayerDistence)
        {
            seesPlayer = true;
        }
        Vector3 normal = rigidbody.velocity.normalized;
        rigidbody.velocity -= normal * Time.deltaTime * 3f;
        if (rigidbody.velocity.normalized != normal)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    /*IEnumerator WalkTowardPlayer()
    {
        while (true)
        {
            yield return new WaitUntil(() => (groundChecker.inGround));
            if ((player.transform.position - transform.position).magnitude < maxDistenceFromPlayer)
            {
                Vector3 xzDirection = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, player.transform.position.z - gameObject.transform.position.z).normalized;
                Vector3 jumpForce = Random.Range(averageJumpStrengthMin, averageJumpStrengthMax) * (xzDirection + (Random.Range(averageJumpHeightStrengthMin, averageJumpHeightStrengthMax) * Vector3.up));
                rigidbody.AddForce(jumpForce * playerStatus.slimeJumpStrengthMultiplier, ForceMode.Impulse);
            }
            else
            {
                slimeSpawner.SlimeDeath();
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(.3f);
            yield return new WaitUntil(() => (groundChecker.inGround));
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce(-jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(Random.Range(.2f, .4f));
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= other.GetComponent<Bullet>().power * 25;
            if (health <= 0)
            {
                //slimeSpawner.SlimeDeath();
                Destroy(gameObject);
            }
            else
            {
                //Quaternion.FromToRotation

                Vector3 direction = truePosition.position;//Definitly not what you are meant to so
                Quaternion originalRotation = transform.rotation;
                transform.rotation = other.transform.rotation;
                transform.Translate(other.GetComponent<Bullet>().movement);
                direction = truePosition.position - direction;
                transform.Translate(-other.GetComponent<Bullet>().movement);
                transform.rotation = originalRotation;

                StartCoroutine(Knockback(direction * other.GetComponent<Bullet>().power * .3f));
            }
            Destroy(other.gameObject);
        }
    }

    IEnumerator Knockback(Vector3 movement)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            transform.position += ((1f - timer) / 1f) * movement * Time.deltaTime;
            //rigidbody.AddForce(((1f - timer) / 1f) * movement * Time.deltaTime, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerStatus>().TakeDamage(damage, truePosition.position, knockback);
        }
    }
}
