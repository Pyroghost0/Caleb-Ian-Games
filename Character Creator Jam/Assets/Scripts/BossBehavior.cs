using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public SlimeSpawner[] slimeSpawners;
    private Vector3 pipeDistence = new Vector3(0f, 1.5f, 0f);
    private Vector3 abovePosition = Vector3.up * 20f;
    private Vector3 spawnPosition;
    private CharacterController characterController;
    public float movementSpeed = 8f;
    public float turnSpeed = 5f;
    private GameObject player;
    public float health = 500f;
    public bool knockBack = true;
    private bool inCenter = true;
    public Transform[] sucktionSpots;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != spawnPosition && knockBack)
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
                inCenter = true;
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

    public void StartFight()
    {
        StartCoroutine(BossActive());
    }

    IEnumerator BossActive()
    {
        while (health > 0)
        {
            for (int i = 0; i < slimeSpawners.Length; i++)
            {
                StartCoroutine(PipeSpawnSlime(3f, slimeSpawners[i]));
            }
            yield return new WaitForSeconds(2f);
            bool slimeAttack = true;
            if (slimeAttack)
            {
                knockBack = false;
                float timer = 0f;
                float seconds = 2f;
                while (timer < seconds)
                {
                    yield return new WaitForFixedUpdate();
                    transform.position += abovePosition * Time.deltaTime / seconds;
                    timer += Time.deltaTime;
                }
                transform.position = new Vector3(transform.position.x, spawnPosition.y + 20f, transform.position.z);
                yield return new WaitForSeconds(3f);
                GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
                List<Transform> assignedSlimesTransform = new List<Transform>();
                List<Rigidbody> assignedSlimesRigidbody = new List<Rigidbody>();
                List<float> assignedSlimesOriginalDistence = new List<float>();
                for (int i = 0; i < slimes.Length && assignedSlimesTransform.Count < sucktionSpots.Length; i++)
                {
                    if (Random.value < .7f)
                    {//70% chance
                        assignedSlimesTransform.Add(slimes[i].transform);
                        assignedSlimesRigidbody.Add(slimes[i].GetComponent<Rigidbody>());
                        assignedSlimesOriginalDistence.Add((sucktionSpots[assignedSlimesOriginalDistence.Count].position - slimes[i].transform.position).magnitude);
                    }
                }
                timer = 0f;
                while (timer < 5f)
                {
                    for (int i = 0; i < assignedSlimesTransform.Count; i++)
                    {
                        if (assignedSlimesTransform[i] != null)
                        {
                            Vector3 distence = sucktionSpots[i].position - assignedSlimesTransform[i].position;
                            if (distence.magnitude > assignedSlimesOriginalDistence[i] / 8f)
                            {
                                assignedSlimesRigidbody[i].velocity -= assignedSlimesRigidbody[i].velocity * Time.deltaTime;
                                assignedSlimesRigidbody[i].AddForce(distence.normalized * 5000f * Time.deltaTime, ForceMode.Force);
                            }
                            /*else if (distence.magnitude > assignedSlimesOriginalDistence[i] / 8f)
                            {
                                assignedSlimesRigidbody[i].velocity -= assignedSlimesRigidbody[i].velocity * Time.deltaTime;
                                assignedSlimesRigidbody[i].AddForce(distence.normalized * 2000f * Time.deltaTime, ForceMode.Force);
                            }*/
                            else
                            {
                                assignedSlimesRigidbody[i].AddForce(distence * 1000f * Time.deltaTime, ForceMode.Force);
                                assignedSlimesOriginalDistence[i] = 1000f;
                            }
                        }
                    }
                    yield return new WaitForFixedUpdate();
                    timer += Time.deltaTime;
                }
                yield return new WaitForSeconds(3f);
                timer = 0f;
                while (timer < seconds)
                {
                    yield return new WaitForFixedUpdate();
                    transform.position -= abovePosition * Time.deltaTime / seconds;
                    timer += Time.deltaTime;
                }
                transform.position = new Vector3(transform.position.x, spawnPosition.y, transform.position.z);
                knockBack = true;
            }
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
            else if (knockBack)
            {
                inCenter = false;
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
