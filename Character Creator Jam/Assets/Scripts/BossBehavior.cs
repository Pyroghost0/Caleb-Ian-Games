using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public SlimeSpawner[] slimeSpawners;
    public SlimeSpawner[] walkerSpawners;
    public SlimeSpawner[] flyerSpawners;
    public float movementSpeed = 8f;
    public float turnSpeed = 5f;
    public float health = 500f;
    public bool knockBack = true;
    public Transform[] sucktionSpots;
    public GameObject walkerPrefab;
    public GameObject flyerPrefab;
    public GameObject bulletPrefab;
    public Transform bulletSpawnSpot;

    private Vector3 pipeDistence = new Vector3(0f, 1.5f, 0f);
    private Vector3 otherPipeDistence = new Vector3(0f, 3.5f, 0f);
    private Vector3 abovePosition = Vector3.up * 20f;
    private Vector3 otherAbovePosition = Vector3.up * 6f;
    private Vector3 spawnPosition;
    private CharacterController characterController;
    private CharacterController playerController;
    private GameObject player;
    private bool inCenter = true;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        playerController = player.GetComponent<CharacterController>();
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
            bool slimeAttack = false;



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


            bool suckAttack = true;
            if (suckAttack)
            {
                knockBack = false;
                float timer = 0f;
                float seconds = 1f;
                while (timer < seconds)
                {
                    yield return new WaitForFixedUpdate();
                    transform.position += otherAbovePosition * Time.deltaTime / seconds;
                    timer += Time.deltaTime;
                }
                transform.position = new Vector3(transform.position.x, spawnPosition.y + 6f, transform.position.z);
                yield return new WaitForSeconds(3f);
                GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
                List<Transform> slimesTransform = new List<Transform>();
                List<Rigidbody> slimesRigidbody = new List<Rigidbody>();
                for (int i = 0; i < slimes.Length; i++)
                {
                    slimesTransform.Add(slimes[i].transform);
                    slimesRigidbody.Add(slimes[i].GetComponent<Rigidbody>());
                }
                GameObject[] walkers = GameObject.FindGameObjectsWithTag("Walker");
                List<Transform> walkersTransform = new List<Transform>();
                List<CharacterController> walkersController = new List<CharacterController>();
                for (int i = 0; i < walkers.Length; i++)
                {
                    walkersTransform.Add(walkers[i].transform);
                    walkersController.Add(walkers[i].GetComponent<CharacterController>());
                }
                GameObject[] flyers = GameObject.FindGameObjectsWithTag("Flyer");
                List<Transform> flyersTransform = new List<Transform>();
                List<Transform> flyersPosition = new List<Transform>();
                for (int i = 0; i < flyers.Length; i++)
                {
                    flyersTransform.Add(flyers[i].transform);
                    flyersPosition.Add(flyers[i].GetComponent<FlyerBehavior>().truePosition);
                }
                timer = 0f;
                while (timer < 5f)
                {
                    for (int i = 0; i < slimes.Length; i++)
                    {
                        if (slimesTransform[i] != null)
                        {
                            slimesRigidbody[i].AddForce((spawnPosition - slimesTransform[i].position).normalized * 1500f * Time.deltaTime, ForceMode.Force);
                        }
                    }
                    for (int i = 0; i < walkers.Length; i++)
                    {
                        if (walkersTransform[i] != null)
                        {
                            walkersController[i].enabled = true;
                            walkersController[i].Move((spawnPosition - walkersTransform[i].position).normalized * 8f * Time.deltaTime);
                            walkersController[i].enabled = false;
                        }
                    }
                    for (int i = 0; i < flyers.Length; i++)
                    {
                        if (flyersTransform[i] != null)
                        {
                            flyersTransform[i].position += (spawnPosition - flyersPosition[i].position).normalized * 8f * Time.deltaTime;
                        }
                    }
                    playerController.enabled = true;
                    playerController.Move((spawnPosition - player.transform.position).normalized * 15f * Time.deltaTime);
                    playerController.enabled = false;
                    yield return new WaitForFixedUpdate();
                    timer += Time.deltaTime;
                }
                yield return new WaitForSeconds(3f);
                timer = 0f;
                while (timer < seconds)
                {
                    yield return new WaitForFixedUpdate();
                    transform.position -= otherAbovePosition * Time.deltaTime / seconds;
                    timer += Time.deltaTime;
                }
                transform.position = new Vector3(transform.position.x, spawnPosition.y, transform.position.z);
                knockBack = true;
            }



            yield return new WaitForSeconds(1f);
            for (int i = 0; i < walkerSpawners.Length; i++)
            {
                StartCoroutine(PipeSpawnOther(3f, walkerSpawners[i], walkerPrefab));
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < flyerSpawners.Length; i++)
            {
                StartCoroutine(PipeSpawnOther(3f, flyerSpawners[i], flyerPrefab));
            }
            yield return new WaitForSeconds(4f);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnSpot.position, Quaternion.LookRotation(player.transform.position - bulletSpawnSpot.position));
            //bullet.GetComponent<Bullet>().bossBullet = true;
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

    IEnumerator PipeSpawnOther(float seconds, SlimeSpawner slimeSpawner, GameObject prefab)
    {
        Transform pipe = slimeSpawner.transform;
        float timer = 0f;
        while (timer < seconds / 3f)
        {
            yield return new WaitForFixedUpdate();
            pipe.position += otherPipeDistence * Time.deltaTime * 3f / seconds;
            timer += Time.deltaTime;
        }
        pipe.position = new Vector3(pipe.transform.position.x, -.5f, pipe.position.z);
        timer = 0f;
        yield return new WaitForSeconds(seconds / 6f);
        Instantiate(prefab, slimeSpawner.spawnPoint.transform.position + prefab.transform.position, prefab.transform.rotation);
        yield return new WaitForSeconds(seconds / 6f);
        while (timer < seconds / 3f)
        {
            yield return new WaitForFixedUpdate();
            pipe.position -= otherPipeDistence * Time.deltaTime * 3f / seconds;
            timer += Time.deltaTime;
        }
        pipe.position = new Vector3(pipe.position.x, -4f, pipe.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !other.GetComponent<Bullet>().bossBullet)
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
