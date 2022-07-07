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
    public float health = 1000f;
    public float maxHealth = 1000f;
    public bool knockBack = true;
    public Transform[] sucktionSpots;
    public GameObject walkerPrefab;
    public GameObject flyerPrefab;
    public GameObject bulletPrefab;
    public Transform bulletSpawnSpot;
    public GameObject singleSuckParticles;
    public GameObject wideSuckParticles;
    public Animator bossAnim;
    public Transform truePosition;

    private Vector3 pipeDistence = new Vector3(0f, 1.5f, 0f);
    private Vector3 otherPipeDistence = new Vector3(0f, 3.5f, 0f);
    private Vector3 abovePosition = Vector3.up * 20f;
    private Vector3 otherAbovePosition = Vector3.up * 6f;
    private Vector3 spawnPosition;
    private CharacterController characterController;
    private CharacterController playerController;
    private GameObject player;
    private Transform playerTruePosition;
    private bool inCenter = true;
    private RectTransform rectHealthBar;
    private float rectHealth;
    private BossMonologue bossMonologue;
    private bool killed = false;
    List<Transform> wideSlimesTransform = new List<Transform>();
    List<Rigidbody> wideSlimesRigidbody = new List<Rigidbody>();
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player;
        playerTruePosition = player.GetComponent<PlayerStatus>().truePosition;
        audioManager = player.GetComponent<AudioManager>();
        playerController = player.GetComponent<CharacterController>();
        characterController = GetComponent<CharacterController>();
        bossMonologue = GetComponent<BossMonologue>();
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
        GameObject bossBar = GameObject.FindGameObjectWithTag("Boss Health Bar");
        for (int i =0; i < bossBar.transform.childCount; i++)
        {
            bossBar.transform.GetChild(i).gameObject.SetActive(true);
        }
#pragma warning disable CS0618 // Type or member is obsolete
        rectHealthBar = bossBar.transform.FindChild("Health Bar").GetComponent<RectTransform>();
#pragma warning restore CS0618 // Type or member is obsolete
        rectHealth = rectHealthBar.rect.width;
    }

    public void RestartFight()
    {
        if (!killed)
        {
            StopAllCoroutines();
            bossAnim.SetTrigger("Restart");
            GameObject[] walkers = GameObject.FindGameObjectsWithTag("Walker");
            for (int i = 0; i < walkers.Length; i++)
            {
                Destroy(walkers[i]);
            }
            GameObject[] flyers = GameObject.FindGameObjectsWithTag("Flyer");
            for (int i = 0; i < flyers.Length; i++)
            {
                Destroy(flyers[i]);
            }
            health = maxHealth;
            rectHealthBar.sizeDelta = new Vector2(rectHealth, rectHealthBar.rect.height);
            inCenter = true;
            knockBack = true;
            transform.position = spawnPosition;
            singleSuckParticles.SetActive(false);
            wideSuckParticles.SetActive(false);
            for (int i = 0; i < slimeSpawners.Length; i++)
            {
                slimeSpawners[i].transform.position = new Vector3(slimeSpawners[i].transform.position.x, -2f, slimeSpawners[i].transform.position.z);
            }
            for (int i = 0; i < walkerSpawners.Length; i++)
            {
                walkerSpawners[i].transform.position = new Vector3(walkerSpawners[i].transform.position.x, -4f, walkerSpawners[i].transform.position.z);
            }
            for (int i = 0; i < flyerSpawners.Length; i++)
            {
                flyerSpawners[i].transform.position = new Vector3(flyerSpawners[i].transform.position.x, -4f, flyerSpawners[i].transform.position.z);
            }
            StartCoroutine(BossActive());
        }
    }

    IEnumerator BossActive()
    {
        while (health > 0)
        {
            //Chose action & Number of shots & Slime spawn time depending on health
            int actionChoice = Random.Range(0, 5);
            int numShots = 0;
            float shotDelayMin = 1f;
            float shotDelayMax = 1f;
            float slimeSpawnSecondsMin = 1f;
            float slimeSpawnSecondsMax = 1f;
            if (health / maxHealth > .825f)
            {
                if (actionChoice == 0 || actionChoice == 1)
                {
                    numShots = Random.Range(1, 4);
                    shotDelayMin = 2f;
                    shotDelayMax = 3f;
                    slimeSpawnSecondsMin = 3f;
                    slimeSpawnSecondsMax = 5f;
                }
                else if (actionChoice == 2 || actionChoice == 3)
                {
                    numShots = Random.Range(2, 5);
                    shotDelayMin = 1.5f;
                    shotDelayMax = 2.5f;
                    slimeSpawnSecondsMin = 2.5f;
                    slimeSpawnSecondsMax = 4f;
                }
                else if (actionChoice == 4)
                {
                    numShots = Random.Range(5, 9);
                    shotDelayMin = 1.25f;
                    shotDelayMax = 2f;
                    slimeSpawnSecondsMin = 2.5f;
                    slimeSpawnSecondsMax = 3f;
                }
            }
            else if (health / maxHealth > .5f)
            {
                if (actionChoice == 0 || actionChoice == 1)
                {
                    numShots = Random.Range(1, 3);
                    shotDelayMin = 1f;
                    shotDelayMax = 2f;
                    slimeSpawnSecondsMin = 2f;
                    slimeSpawnSecondsMax = 3f;
                }
                else if (actionChoice == 2 || actionChoice == 3)
                {
                    numShots = Random.Range(2, 4);
                    shotDelayMin = .9f;
                    shotDelayMax = 1.6f;
                    slimeSpawnSecondsMin = 1.8f;
                    slimeSpawnSecondsMax = 2.5f;
                }
                else if (actionChoice == 4)
                {
                    numShots = Random.Range(5, 8);
                    shotDelayMin = .75f;
                    shotDelayMax = 1.5f;
                    slimeSpawnSecondsMin = 2f;
                    slimeSpawnSecondsMax = 2.4f;
                }
            }
            else if (health / maxHealth > .175)
            {
                if (actionChoice == 0 || actionChoice == 1)
                {
                    numShots = Random.Range(0, 3);
                    shotDelayMin = 1f;
                    shotDelayMax = 1.25f;
                    slimeSpawnSecondsMin = 1f;
                    slimeSpawnSecondsMax = 2f;
                }
                else if (actionChoice == 2 || actionChoice == 3)
                {
                    numShots = Random.Range(0, 4);
                    shotDelayMin = .75f;
                    shotDelayMax = 1.2f;
                    slimeSpawnSecondsMin = 2.5f;
                    slimeSpawnSecondsMax = 3f;
                }
                else if (actionChoice == 4)
                {
                    numShots = Random.Range(7, 11);
                    shotDelayMin = .5f;
                    shotDelayMax = 1f;
                    slimeSpawnSecondsMin = 2f;
                    slimeSpawnSecondsMax = 2.5f;
                }
            }
            else
            {
                if (actionChoice == 0 || actionChoice == 1)
                {
                    numShots = 1;
                    shotDelayMin = .1f;
                    shotDelayMax = .2f;
                    slimeSpawnSecondsMin = .8f;
                    slimeSpawnSecondsMax = 1.2f;
                }
                else if (actionChoice == 2 || actionChoice == 3)
                {
                    numShots = Random.Range(1, 4);
                    shotDelayMin = .5f;
                    shotDelayMax = .8f;
                    slimeSpawnSecondsMin = 2f;
                    slimeSpawnSecondsMax = 2.5f;
                }
                else if (actionChoice == 4)
                {
                    numShots = Random.Range(10, 16);
                    shotDelayMin = .35f;
                    shotDelayMax = .5f;
                    slimeSpawnSecondsMin = 1.5f;
                    slimeSpawnSecondsMax = 2f;
                }
            }

            //Spawn slimes and wait if above half
            
            for (int i = 0; i < slimeSpawners.Length; i++)
            {
                //bossAnim.SetTrigger("ChooseSummon");
                StartCoroutine(PipeSpawnSlime(Random.Range(slimeSpawnSecondsMin, slimeSpawnSecondsMax), slimeSpawners[i]));
            }
            if (health/maxHealth > .5f)
            {
                yield return new WaitForSeconds(Random.Range(slimeSpawnSecondsMax, slimeSpawnSecondsMax*1.5f));
            }

            //Actions
            if (actionChoice == 0)
            {
                yield return new WaitUntil(() => inCenter);
                Debug.Log("Slime Sucking");
                bossAnim.SetTrigger("ChooseSuck");
                yield return new WaitForSeconds(1.5f);
                if (health / maxHealth > .75)
                {
                    StartCoroutine(SingleSlimeSuck(3f, 1f, 5f, .5f));
                    yield return new WaitForSeconds(13f);
                }
                else if (health / maxHealth > .5)
                {
                    StartCoroutine(SingleSlimeSuck(2.5f, .5f, 4f, .65f));
                    yield return new WaitForSeconds(10f);
                }
                else if (health / maxHealth > .25)
                {
                    StartCoroutine(SingleSlimeSuck(slimeSpawnSecondsMax, .25f, 4f, .8f));
                    yield return new WaitForSeconds(slimeSpawnSecondsMax + 1f);
                    if (Random.value > .5f)
                    {
                        StartCoroutine(PipeSpawnOther(1.5f, walkerSpawners[Random.Range(0, walkerSpawners.Length)], walkerPrefab));
                    }
                    else
                    {
                        StartCoroutine(PipeSpawnOther(1.5f, flyerSpawners[Random.Range(0, flyerSpawners.Length)], flyerPrefab));
                    }
                    yield return new WaitForSeconds(slimeSpawnSecondsMax + 3.5f);
                }
                else
                {
                    StartCoroutine(SingleSlimeSuck(slimeSpawnSecondsMax*.85f, slimeSpawnSecondsMax*.15f, 6f, 1f));
                    yield return new WaitForSeconds(slimeSpawnSecondsMax);
                    if (Random.value > .5f)
                    {
                        StartCoroutine(PipeSpawnOther(1.2f, walkerSpawners[Random.Range(0, walkerSpawners.Length)], walkerPrefab));
                    }
                    else
                    {
                        StartCoroutine(PipeSpawnOther(1.2f, flyerSpawners[Random.Range(0, flyerSpawners.Length)], flyerPrefab));
                    }
                    yield return new WaitForSeconds(3f);
                    if (Random.value > .5f)
                    {
                        StartCoroutine(PipeSpawnOther(1.2f, walkerSpawners[Random.Range(0, walkerSpawners.Length)], walkerPrefab));
                    }
                    else
                    {
                        StartCoroutine(PipeSpawnOther(1.2f, flyerSpawners[Random.Range(0, flyerSpawners.Length)], flyerPrefab));
                    }
                    yield return new WaitForSeconds(slimeSpawnSecondsMax + 3f);
                }
            }
            else if (actionChoice == 1)
            {
                yield return new WaitUntil(() => inCenter);
                Debug.Log("Wide Range Suck");
                bossAnim.SetTrigger("ChooseSuck");
                yield return new WaitForSeconds(1.5f);
                if (health / maxHealth > .75)
                {
                    StartCoroutine(WideSuck(1.25f, 1f, 3.5f));
                    yield return new WaitForSeconds(8f);
                }
                else if (health / maxHealth > .5)
                {
                    StartCoroutine(WideSuck(1f, .5f, 5f));
                    List<int> chosenNums = new List<int>();
                    chosenNums.Add(0);
                    chosenNums.Add(1);
                    chosenNums.Add(2);
                    chosenNums.Add(3);
                    chosenNums.Add(4);
                    chosenNums.Add(5);
                    for (int i = 0; i < 3; i++)
                    {
                        chosenNums.RemoveAt(Random.Range(0, chosenNums.Count));
                    }
                    yield return new WaitForSeconds(2f);
                    for (int i = 0; i < 3; i++)
                    {
                        int num = Random.Range(0, chosenNums.Count);
                        StartCoroutine(PipeSpawnSlime(1.25f, slimeSpawners[chosenNums[num]]));
                        chosenNums.RemoveAt(num);
                        yield return new WaitForSeconds(1f);
                    }
                    yield return new WaitForSeconds(3f);
                }
                else if (health / maxHealth > .25)
                {
                    StartCoroutine(WideSuck(slimeSpawnSecondsMax/2f, slimeSpawnSecondsMax/2f, 8f));
                    List<int> chosenNums = new List<int>();
                    chosenNums.Add(0);
                    chosenNums.Add(1);
                    chosenNums.Add(2);
                    chosenNums.Add(3);
                    chosenNums.Add(4);
                    chosenNums.Add(5);
                    yield return new WaitForSeconds(slimeSpawnSecondsMax);
                    for (int i = 0; i < 6; i++)
                    {
                        int num = Random.Range(0, chosenNums.Count);
                        StartCoroutine(PipeSpawnSlime(1.25f, slimeSpawners[chosenNums[num]]));
                        chosenNums.RemoveAt(num);
                        yield return new WaitForSeconds(1f);
                    }
                    yield return new WaitForSeconds(slimeSpawnSecondsMax + 2f);
                }
                else
                {
                    StartCoroutine(WideSuck(slimeSpawnSecondsMax / 2f, slimeSpawnSecondsMax / 4f, 12f));
                    List<int> prechosenNums = new List<int>();
                    List<int> chosenNums = new List<int>();
                    prechosenNums.Add(0);
                    prechosenNums.Add(1);
                    prechosenNums.Add(2);
                    prechosenNums.Add(3);
                    prechosenNums.Add(4);
                    prechosenNums.Add(5);
                    for (int i = 0; i < 6; i++)
                    {
                        int num = Random.Range(0, prechosenNums.Count);
                        chosenNums.Add(prechosenNums[num]);
                        prechosenNums.RemoveAt(num);
                    }
                    yield return new WaitForSeconds(slimeSpawnSecondsMax);
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < chosenNums.Count; j++)
                        {
                            int num = Random.Range(0, chosenNums.Count);
                            StartCoroutine(PipeSpawnSlime(1f, slimeSpawners[chosenNums[num]]));
                            chosenNums.RemoveAt(num);
                            yield return new WaitForSeconds(.75f);
                        }
                    }
                    yield return new WaitForSeconds((slimeSpawnSecondsMax/2f) + 3f);
                }
            }
            else if (actionChoice == 2)
            {
                Debug.Log("Walker Spawn");
                bossAnim.SetTrigger("ChooseSummon");
                if (health / maxHealth > .75)
                {
                    StartCoroutine(PipeSpawnOther(2f, walkerSpawners[Random.Range(0, walkerSpawners.Length)], walkerPrefab));
                    yield return new WaitForSeconds(2f);
                }
                else if (health / maxHealth > .5)
                {
                    if (Random.value > .5f)
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[0], walkerPrefab));
                        StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[1], walkerPrefab));
                    }
                    else
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[1], walkerPrefab));
                        StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[0], walkerPrefab));
                    }
                    yield return new WaitForSeconds(3.5f);
                }
                else if (health / maxHealth > .25)
                {
                    for (int i = 0; i < walkerSpawners.Length; i++)
                    {
                        StartCoroutine(PipeSpawnOther(2.5f, walkerSpawners[i], walkerPrefab));
                    }
                    yield return new WaitForSeconds(2.5f);
                }
                else
                {
                    for (int i = 0; i < walkerSpawners.Length; i++)
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[i], walkerPrefab));
                    }
                    yield return new WaitForSeconds(1.75f);
                    StartCoroutine(PipeSpawnOther(1.75f, walkerSpawners[Random.Range(0, walkerSpawners.Length)], walkerPrefab));
                    yield return new WaitForSeconds(1.75f);
                }
            }
            else if (actionChoice == 3)
            {
                Debug.Log("Flyer Spawn");
                bossAnim.SetTrigger("ChooseSummon");
                if (health / maxHealth > .75)
                {
                    StartCoroutine(PipeSpawnOther(2f, flyerSpawners[Random.Range(0, flyerSpawners.Length)], flyerPrefab));
                    yield return new WaitForSeconds(2f);
                }
                else if (health / maxHealth > .5)
                {
                    if (Random.value > .5f)
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[0], flyerPrefab));
                        StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[1], flyerPrefab));
                    }
                    else
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[1], flyerPrefab));
                        StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[0], flyerPrefab));
                    }
                    yield return new WaitForSeconds(3.5f);
                }
                else if (health / maxHealth > .25)
                {
                    for (int i = 0; i < flyerSpawners.Length; i++)
                    {
                        StartCoroutine(PipeSpawnOther(2.5f, flyerSpawners[i], flyerPrefab));
                    }
                    yield return new WaitForSeconds(2.5f);
                }
                else
                {
                    for (int i = 0; i < flyerSpawners.Length; i++)
                    {
                        StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[i], flyerPrefab));
                    }
                    yield return new WaitForSeconds(1.75f);
                    StartCoroutine(PipeSpawnOther(1.75f, flyerSpawners[Random.Range(0, flyerSpawners.Length)], flyerPrefab));
                    yield return new WaitForSeconds(1.75f);
                }
            }
            else
            {
                Debug.Log("Extra Shooting Gun");
                
            }
            bossAnim.SetTrigger("ChooseShoot");
            yield return new WaitForSeconds(1.5f);
            for (int i = 0; i < numShots; i++)
            {
                bossAnim.SetTrigger("Shoot");
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnSpot.position, Quaternion.LookRotation(playerTruePosition.position - bulletSpawnSpot.position));
                yield return new WaitForSeconds(Random.Range(shotDelayMin, shotDelayMax));
            }
            bossAnim.SetTrigger("EndAction");
        }
    }

    IEnumerator SingleSlimeSuck(float riseSeconds, float delaySeconds, float suckingSeconds, float slimeChance)
    {
        knockBack = false;
        float timer = 0f;
        bossAnim.SetTrigger("Rise");
        while (timer < riseSeconds)
        {
            yield return new WaitForFixedUpdate();
            transform.position += abovePosition * Time.deltaTime / riseSeconds;
            timer += Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, spawnPosition.y + 20f, transform.position.z);
        yield return new WaitForSeconds(delaySeconds);
        bossAnim.SetTrigger("SlimeSuck");
        singleSuckParticles.SetActive(true);
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        List<Transform> assignedSlimesTransform = new List<Transform>();
        List<Rigidbody> assignedSlimesRigidbody = new List<Rigidbody>();
        List<float> assignedSlimesOriginalDistence = new List<float>();
        for (int i = 0; i < slimes.Length && assignedSlimesTransform.Count < sucktionSpots.Length; i++)
        {
            if (Random.value < slimeChance)
            {//70% chance
                assignedSlimesTransform.Add(slimes[i].transform);
                assignedSlimesRigidbody.Add(slimes[i].GetComponent<Rigidbody>());
                assignedSlimesOriginalDistence.Add((sucktionSpots[assignedSlimesOriginalDistence.Count].position - slimes[i].transform.position).magnitude);
            }
        }
        timer = 0f;
        while (timer < suckingSeconds)
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
        singleSuckParticles.SetActive(false);
        
        yield return new WaitForSeconds(delaySeconds);
        timer = 0f;
        bossAnim.SetTrigger("EndSuck");
        while (timer < riseSeconds)
        {
            yield return new WaitForFixedUpdate();
            transform.position -= abovePosition * Time.deltaTime / riseSeconds;
            timer += Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, spawnPosition.y, transform.position.z);
        knockBack = true;
        bossAnim.SetTrigger("EndAction");
    }

    IEnumerator WideSuck(float riseSeconds, float delaySeconds, float suckingSeconds)
    {
        knockBack = false;
        float timer = 0f;
        bossAnim.SetTrigger("Rise");
        while (timer < riseSeconds)
        {
            yield return new WaitForFixedUpdate();
            transform.position += otherAbovePosition * Time.deltaTime / riseSeconds;
            timer += Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, spawnPosition.y + 6f, transform.position.z);
        yield return new WaitForSeconds(delaySeconds);
        wideSuckParticles.SetActive(true);
        bossAnim.SetTrigger("WideSuck");
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        wideSlimesTransform = new List<Transform>();
        wideSlimesRigidbody = new List<Rigidbody>();
        for (int i = 0; i < slimes.Length; i++)
        {
            wideSlimesTransform.Add(slimes[i].transform);
            wideSlimesRigidbody.Add(slimes[i].GetComponent<Rigidbody>());
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
        while (timer < suckingSeconds)
        {
            for (int i = 0; i < wideSlimesTransform.Count; i++)
            {
                if (wideSlimesTransform[i] != null)
                {
                    wideSlimesRigidbody[i].AddForce((spawnPosition - wideSlimesTransform[i].position).normalized * 1500f * Time.deltaTime, ForceMode.Force);
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
            playerController.Move((spawnPosition - player.transform.position).normalized * 11f * Time.deltaTime);
            playerController.enabled = false;
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
        wideSuckParticles.SetActive(false);
        
        yield return new WaitForSeconds(delaySeconds);
        timer = 0f;
        bossAnim.SetTrigger("EndSuck");
        while (timer < riseSeconds)
        {
            yield return new WaitForFixedUpdate();
            transform.position -= otherAbovePosition * Time.deltaTime / riseSeconds;
            timer += Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, spawnPosition.y, transform.position.z);
        knockBack = true;
        bossAnim.SetTrigger("EndAction");
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
        GameObject slime = slimeSpawner.SpawnSlime();
        wideSlimesTransform.Add(slime.transform);
        wideSlimesRigidbody.Add(slime.GetComponent<Rigidbody>());
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
        audioManager.SpawnSlime();
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
        if (other.CompareTag("Bullet") && !other.GetComponent<Bullet>().bossBullet && !killed)
        {
            health -= other.GetComponent<Bullet>().power * 25;
            rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
            if (health <= 0 && !killed)
            {
                killed = true;
                bossAnim.SetBool("isDead", true);
                singleSuckParticles.SetActive(false);
                wideSuckParticles.SetActive(false);
                truePosition.localPosition = Vector3.zero;
                StopAllCoroutines();

                for (int i = 0; i < slimeSpawners.Length; i++)
                {
                    slimeSpawners[i].transform.position = new Vector3(slimeSpawners[i].transform.position.x, -2f, slimeSpawners[i].transform.position.z);
                }
                GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
                for (int i = 0; i < slimes.Length; i++)
                {
                    Destroy(slimes[i]);
                }
                GameObject[] walkers = GameObject.FindGameObjectsWithTag("Walker");
                for (int i = 0; i < walkers.Length; i++)
                {
                    Destroy(walkers[i]);
                }
                GameObject[] flyers = GameObject.FindGameObjectsWithTag("Flyer");
                for (int i = 0; i < flyers.Length; i++)
                {
                    Destroy(flyers[i]);
                }
                transform.position = spawnPosition;
                inCenter = true;
                bossMonologue.StartOutro();
            }
            else if (knockBack)
            {
                inCenter = false;
                Vector3 direction = other.transform.rotation * other.GetComponent<Bullet>().movement;
                direction.y = 0f;
                StartCoroutine(Knockback(direction * other.GetComponent<Bullet>().power * .1f));
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
