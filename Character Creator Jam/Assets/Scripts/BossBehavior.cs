using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public SlimeSpawner[] slimeSpawners;
    private Vector3 pipeDistence = new Vector3(0f, 1.5f, 0f);
    private Vector3 spawnPosition;
    private CharacterController characterController;
    public float movementSpeed = 8f;
    public float turnSpeed = 5f;
    private GameObject player;
    public float health = 500f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        characterController = GetComponent<CharacterController>();
        for (int i = 0; i < slimeSpawners.Length; i++)
        {
            StartCoroutine(PipeSpawnSlime(3f, slimeSpawners[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != spawnPosition)
        {
            Vector3 direction = spawnPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);

            characterController.enabled = true;
            characterController.Move(transform.rotation * Vector3.forward * movementSpeed * Time.deltaTime);
            characterController.enabled = false;

            Vector3 direction2 = (spawnPosition - transform.position).normalized;

            if ((direction.x > 0 && direction2.x <= 0) || (direction.x < 0 && direction2.x >= 0) || (direction.z > 0 && direction2.z <= 0) || (direction.z < 0 && direction2.z >= 0))
            {//Overstepped
                transform.position = spawnPosition;
            }
        }
        else
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            direction = direction.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);
        }
    }

    IEnumerator PipeSpawnSlime(float seconds, SlimeSpawner slimeSpawner)
    {
        Transform pipe = slimeSpawner.transform;
        float timer = 0f;
        while (timer < seconds / 3f)
        {
            yield return new WaitForFixedUpdate();
            pipe.position += pipeDistence * Time.deltaTime * 3f / seconds;
            timer += Time.deltaTime;
        }
        pipe.position = new Vector3(pipe.transform.position.x, -.5f, pipe.position.z);
        timer = 0f;
        yield return new WaitForSeconds(seconds / 6f);
        slimeSpawner.SpawnSlime();
        yield return new WaitForSeconds(seconds / 6f);
        while (timer < seconds / 3f)
        {
            yield return new WaitForFixedUpdate();
            pipe.position -= pipeDistence * Time.deltaTime * 3f / seconds;
            timer += Time.deltaTime;
        }
        pipe.position = new Vector3(pipe.position.x, -2f, pipe.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= other.GetComponent<Bullet>().power * 25;
            if (health <= 0)
            {
                //slimeSpawner.SlimeDeath();
                //isDead = true;
                //anim.SetBool("isDead", true);
                //StartCoroutine(Die());
            }
            else
            {
                Vector3 direction = other.transform.rotation * other.GetComponent<Bullet>().movement;
                direction.y = 0f;
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
            characterController.enabled = true;
            characterController.Move(((1f - timer) / 1f) * movement * Time.deltaTime);
            characterController.enabled = false;
            //rigidbody.AddForce(((1f - timer) / 1f) * movement * Time.deltaTime, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }
}
