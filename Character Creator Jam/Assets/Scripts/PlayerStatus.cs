using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    //data to save
    public bool isMale = false;
    public int headNumber = 1;
    public int skinColor = 0;
    public int hairColor = 0;
    public bool[] equipmentUnlocked;
    public bool[] setsCompleted;
    public bool[] levelsCompleted;
    private bool defeatedBoss = false;

    //other data
    public GameObject[] maleObjects;
    public GameObject[] femaleObjects;
    public GameObject[] maleFaces;
    public GameObject[] femaleFaces;

    public float health = 100f;
    public float maxHealth = 100f;
    public bool invincible = false;
    public float invincibleTime = 1.5f;
    public GameObject HealthStuff;
    public GameObject healthBar;
    private RectTransform rectHealthBar;
    private float rectHealth;
    private CharacterController characterController;
    private bool notDead = true;
    private bool notHealing = true;

    public GameObject[] selfEquipment;
    public GameObject[] equipmentPrefabs;
    public bool[] equipedEquipment;
    public GameObject[] emptyEquipment;
    public float slimeJumpStrengthMultiplier = 1f;
    private PlayerMovement playerMovement;
    private Gun gun;
    public bool takeKnockback = true;
    public float defenceMultiplier = 1f;
    public bool survivedDeathOnce = false;
    public bool passiveRegeneration = false;

    public bool isTutorial = false;
    public GameObject currentSpawnPosition;
    private Animator playerAnim;
    private AudioManager audioManager;
    public Transform truePosition;

    // Start is called before the first frame update
    void Start()
    {
        rectHealthBar = healthBar.GetComponent<RectTransform>();
        rectHealth = rectHealthBar.rect.width;
        characterController = gameObject.GetComponent<CharacterController>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        audioManager = GetComponent<AudioManager>();
    }

    public void changeCharacter()
    {
        femaleFaces[1].SetActive(false);
        if (isMale)
        {
            for (int i = 0; i < femaleObjects.Length; i++)
            {
                femaleObjects[i].SetActive(false);
            }
            for (int i = 0; i < maleObjects.Length; i++)
            {
                maleObjects[i].SetActive(true);
            }
            maleFaces[headNumber].SetActive(true);
        }
        else
        {
            femaleFaces[headNumber].SetActive(true);
        }
    }

    public void CompletedLevel(string levelName)
	{
        if (levelName == "Mech Level")
		{
            levelsCompleted[0] = true;
		}
        else if (levelName == "Deer Level")
        {
            levelsCompleted[1] = true;
        }
        else if (levelName == "Baker Level")
        {
            levelsCompleted[2] = true;
        }
        else if (levelName == "POTUS Level")
        {
            levelsCompleted[3] = true;
        }
        Save();
    }
	private void Save()
	{
        int[] data = SaveLoad.Load();
        if (data == null)
		{
            data = new int[25];
		}
        data[0] = isMale ? 1 : 0;
        data[1] = headNumber;
        data[2] = skinColor;
        data[3] = hairColor;
        for (int i = 0; i < 12; i++)
        {
            if (data[i + 4] == 0)
                data[i + 4] = equipmentUnlocked[i] ? 1 : 0;
        }
        for (int i = 0; i < 4; i++)
        {

            if (data[i + 16] == 0)
                data[i + 16] = setsCompleted[i] ? 1 : 0;
        }
        for (int i = 0; i < 4; i++)
        {

            if (data[i + 20] == 0)
                data[i + 20] = levelsCompleted[i] ? 1 : 0;
        }
        if (data[24] == 0)
            data[24] = defeatedBoss ? 1 : 0;
        SaveLoad.Save(data);
    }
	public void LoadData(bool playthrough)
	{
        int[] data = SaveLoad.Load();
        isMale = (data[0] > 0);
        headNumber = data[1];
        skinColor = data[2];
        hairColor = data[3];
        for (int i = 0; i < 12; i++)
        {
            equipmentUnlocked[i] = (data[i + 4] > 0);
        }
        for (int i = 0; i < 4; i++)
        {
            setsCompleted[i] = (data[i + 16] > 0);
        }
        for (int i = 0; i < 4; i++)
        {
            levelsCompleted[i] = (data[i + 20] > 0);
        }
        defeatedBoss = playthrough && (data[24] > 0);
    }
    public void DefeatBoss()
	{
        defeatedBoss = true;
	}
    public bool bossDefeated()
	{
        return defeatedBoss;
	}
	public void Equip(Equipment equipment)
    {
        if (equipment.clothingType == "Head")
        {
            int somethingElseEquiped = -1;
            for (int i = 0; i < equipedEquipment.Length; i+=3)
            {
                if (equipedEquipment[i])
                {
                    somethingElseEquiped = i;
                    break;
                }
            }
            if (somethingElseEquiped > -1)
            {
                Dequip(somethingElseEquiped, equipment.gameObject.transform.position);
            }
            if (equipment.clothingStyle == "POTUS")
            {
                emptyEquipment[0].SetActive(false);
                emptyEquipment[3].SetActive(false);
            }
            else if (isMale)
            {
                emptyEquipment[3].SetActive(true);
            }
            else
            {
                emptyEquipment[0].SetActive(true);
            }
            
            int id = 0;
            if (equipment.clothingStyle == "Mech")
            {
                gun.suckDistence *= 2;
                //id = 0;
            }
            else if (equipment.clothingStyle == "Deer")
            {
                slimeJumpStrengthMultiplier = .82f;
                id = 3;
            }
            else if (equipment.clothingStyle == "Baker")
            {
                passiveRegeneration = true;
                StartCoroutine(PassiveRegeneration());
                id = 6;
            }
            else if (equipment.clothingStyle == "POTUS")
            {
                //Escape Death Once
                id = 9;
            }
            equipedEquipment[id] = true;
            equipmentUnlocked[id] = true;
            if (isMale)
            {
                id += 12;
            }
            selfEquipment[id].SetActive(true);
        }


        else if (equipment.clothingType == "Body")
        {
            int somethingElseEquiped = -1;
            for (int i = 1; i < equipedEquipment.Length; i += 3)
            {
                if (equipedEquipment[i])
                {
                    somethingElseEquiped = i;
                    break;
                }
            }
            if (somethingElseEquiped > -1)
            {
                Dequip(somethingElseEquiped, equipment.gameObject.transform.position);
            }
            emptyEquipment[1].SetActive(false);
            emptyEquipment[4].SetActive(false);
            int id = 1;
            if (equipment.clothingStyle == "Mech")
            {
                gun.suckPower *= 2;
                //id = 1;
            }
            else if (equipment.clothingStyle == "Deer")
            {
                gun.powerMultiplier *= 2f;
                id = 4;
            }
            else if (equipment.clothingStyle == "Baker")
            {
                invincibleTime *= 4f;
                id = 7;
            }
            else if (equipment.clothingStyle == "POTUS")
            {
                defenceMultiplier *= 1.5f;
                id = 10;
            }
            equipedEquipment[id] = true;
            equipmentUnlocked[id] = true;
            if (isMale)
            {
                id += 12;
            }
            selfEquipment[id].SetActive(true);
        }


        else if (equipment.clothingType == "Legs")
        {
            int somethingElseEquiped = -1;
            for (int i = 2; i < equipedEquipment.Length; i += 3)
            {
                if (equipedEquipment[i])
                {
                    somethingElseEquiped = i;
                    break;
                }
            }
            if (somethingElseEquiped > -1)
            {
                Dequip(somethingElseEquiped, equipment.gameObject.transform.position);
            }
            emptyEquipment[2].SetActive(false);
            emptyEquipment[5].SetActive(false);
            int id = 2;
            if (equipment.clothingStyle == "Mech")
            {
                playerMovement.jumpHeight *= 2f;
                //id = 2;
                if (!equipmentUnlocked[2])
				{
                    for (int i = 0; i < equipmentUnlocked.Length; i++)
                    {
                        equipmentUnlocked[i] = false;
					}
				}
            }
            else if (equipment.clothingStyle == "Deer")
            {
                playerMovement.speed *= 1.5f;
                id = 5;
            }
            else if (equipment.clothingStyle == "Baker")
            {
                maxHealth *= 1.5f;
                health *= 1.5f;
                id = 8;
            }
            else if (equipment.clothingStyle == "POTUS")
            {
                takeKnockback = false;
                id = 11;
            }
            equipedEquipment[id] = true;
            equipmentUnlocked[id] = true;
            if (isMale)
            {
                id += 12;
            }
            selfEquipment[id].SetActive(true);
        }

        if (equipment.clothingStyle == "Mech" && !setsCompleted[0])
		{
            if (equipmentUnlocked[0] && equipmentUnlocked[1] && equipmentUnlocked[2])
            {
                setsCompleted[0] = true;
            }
        }
        if (equipment.clothingStyle == "Deer" && !setsCompleted[1])
        {
            if (equipmentUnlocked[3] && equipmentUnlocked[4] && equipmentUnlocked[5])
            {
                setsCompleted[1] = true;
            }
        }
        if (equipment.clothingStyle == "Baker" && !setsCompleted[2])
        {
            if (equipmentUnlocked[6] && equipmentUnlocked[7] && equipmentUnlocked[8])
            {
                setsCompleted[2] = true;
            }
        }
        if (equipment.clothingStyle == "POTUS" && !setsCompleted[3])
        {
            if (equipmentUnlocked[9] && equipmentUnlocked[10] && equipmentUnlocked[11])
            {
                setsCompleted[3] = true;
            }
        }

        Destroy(equipment.gameObject);
    }

    private void Dequip(int id, Vector3 position)
    {
        equipedEquipment[id] = false;
        selfEquipment[id].SetActive(false);
        selfEquipment[id+12].SetActive(false);
        if (id == 0)
        {//Mech Helmet
            gun.suckDistence /= 2f;
        }
        else if (id == 1)
        {//Mech Body
            gun.suckPower /= 2f;
        }
        else if (id == 2)
        {//Mech Legs
            playerMovement.jumpHeight /= 2f;
        }

        else if (id == 3)
        {//Deer Helmet
            slimeJumpStrengthMultiplier = 1f;
        }
        else if (id == 4)
        {//Deer Body
            gun.powerMultiplier /= 2f;
        }
        else if (id == 5)
        {//Deer Legs
            playerMovement.speed /= 1.5f;
        }

        else if (id == 6)
        {//Baker Helmet
            passiveRegeneration = false;
        }
        else if (id == 7)
        {//Baker Body
            invincibleTime /= 4f;
        }
        else if (id == 8)
        {//Baker Legs
            maxHealth /= 1.5f;
            health /= 1.5f;
        }

        else if (id == 9)
        {//POTUS Helmet
            //Escape Death Once;
        }
        else if (id == 10)
        {//POTUS Body
            defenceMultiplier /= 1.5f;
        }
        else if (id == 11)
        {//POTUS Legs
            takeKnockback = true;
        }

        Quaternion rotation = Quaternion.Euler(equipmentPrefabs[id].gameObject.transform.rotation.eulerAngles.x, equipmentPrefabs[id].gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
        GameObject oldEquipment = Instantiate(equipmentPrefabs[id], position, rotation);
        StartCoroutine(DelayReequiping(oldEquipment));
    }

    IEnumerator DelayReequiping(GameObject equipment)
    {
        equipment.layer = 2;//Ignore Raycast
        yield return new WaitForSeconds(1f);
        equipment.layer = 9;//Suckable Object
    }

    public void TakeDamage(float damage, Vector3 direction, float knockback)
    {
        if (!invincible)
        {
            invincible = true;
            direction.y = transform.position.y;
            direction = (transform.position - direction).normalized;
            playerAnim.SetTrigger("Hurt");
            if (takeKnockback)
            {
                StartCoroutine(Knockback(direction * knockback));
            }
            StartCoroutine(UpdateHealthBar(-damage / defenceMultiplier));
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
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }

    IEnumerator PassiveRegeneration()
    {
        while (passiveRegeneration)
        {
            yield return new WaitForSeconds(1f);
            if (!invincible && health < maxHealth)
            {
                HealHealth(1f);
            }
        }
    }

    public void HealHealth(float addedHealth)
    {
        if (health < maxHealth)
        {
            if (health + addedHealth < maxHealth)
            {
                health += addedHealth;
                rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
            }
            else
            {
                health = maxHealth;
                rectHealthBar.sizeDelta = new Vector2(rectHealth, rectHealthBar.rect.height);
            }
        }
    }
    
    IEnumerator UpdateHealthBar(float healthChange)
    {
        notDead = true;
        if (health + healthChange <= 0)
        {
            Debug.Log("Dead");
            playerAnim.SetBool("Dead", true);
            playerAnim.SetFloat("MoveX", 0);
            playerAnim.SetFloat("MoveY", 0);
            playerMovement.enabled = false;
            if (isTutorial)
            {
                GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
                for (int i = 0; i < slimes.Length; i++)
                {
                    Destroy(slimes[i]);
                }
                GameObject[] equipment = GameObject.FindGameObjectsWithTag("Equipment");
                for (int i = 0; i < equipment.Length; i++)
                {
                    Destroy(equipment[i]);
                }
                AsyncOperation ao1 = SceneManager.UnloadSceneAsync("Tutorial");
                yield return new WaitUntil(() => ao1.isDone);
                audioManager.ChangeScene("Mech Level");
                AsyncOperation ao2 = SceneManager.LoadSceneAsync("Mech Level", LoadSceneMode.Additive);
                yield return new WaitUntil(() => ao2.isDone);
                playerMovement.groundChecker.inGround = false;
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                isTutorial = false;
                health = maxHealth;
                rectHealthBar.sizeDelta = new Vector2(rectHealth, rectHealthBar.rect.height);
                invincibleTime = 1.5f;
                invincible = false;
                if (equipedEquipment[0])
                {//Mech Helmet
                    gun.suckDistence /= 2f;
                    equipedEquipment[0] = false;
                    selfEquipment[0].SetActive(false);
                    selfEquipment[12].SetActive(false);
                }
                else if (equipedEquipment[3])
                {//Deer Helmet
                    slimeJumpStrengthMultiplier = 1f;
                    equipedEquipment[3] = false;
                    selfEquipment[3].SetActive(false);
                    selfEquipment[15].SetActive(false);
                }
                playerAnim.SetBool("Dead", false);
                playerAnim.Play("Idle");
                playerMovement.enabled = true;
            }
            else
            {
                if (equipedEquipment[9] && !survivedDeathOnce)
                {
                    survivedDeathOnce = true;
                    float timer = 0f;
                    float invincibleTimeWhenHit = invincibleTime;
                    while (timer < invincibleTimeWhenHit)
                    {
                        if (timer % .5f >= .25f)
                        {
                            HealthStuff.SetActive(true);
                        }
                        else
                        {
                            HealthStuff.SetActive(false);
                        }
                        yield return new WaitForFixedUpdate();
                        timer += Time.deltaTime;
                    }
                    HealthStuff.SetActive(true);
                    invincible = false;
                }
                else
                {
                    playerAnim.SetBool("Dead", true);
                    playerMovement.enabled = false;
                    float timer = 0f;
                    healthChange = -health;
                    while (timer < 1f && notDead)
                    {
                        health += healthChange * Time.deltaTime;
                        rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
                        yield return new WaitForFixedUpdate();
                        timer += Time.deltaTime;
                    }
                    //health = 0f;
                    //rectHealthBar.sizeDelta = new Vector2(0f, rectHealthBar.rect.height);
                    invincible = false;
                    if (notDead)
                    {
                        Respawn();
                    }
                }
            }
        }
        else
        {
            float timer = 0f;
            float originalHealth = health;
            float invincibleTimeWhenHit = invincibleTime;
            while (timer < invincibleTimeWhenHit && notDead && notHealing)
            {
                if (timer % .5f >= .25f)
                {
                    HealthStuff.SetActive(true);
                }
                else
                {
                    HealthStuff.SetActive(false);
                }
                health += healthChange * Time.deltaTime / invincibleTimeWhenHit;
                rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
                yield return new WaitForFixedUpdate();
                timer += Time.deltaTime;
            }
            if (notDead)
            {
                health = originalHealth + healthChange;
                rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
            }
            while (timer < invincibleTimeWhenHit && notDead)
            {
                if (timer % .5f >= .25f)
                {
                    HealthStuff.SetActive(true);
                }
                else
                {
                    HealthStuff.SetActive(false);
                }
                yield return new WaitForFixedUpdate();
                timer += Time.deltaTime;
            }
            invincible = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health Kit"))
        {
            StartCoroutine(HealHealthKit());
            Destroy(other.gameObject);
        }
    }

    IEnumerator HealHealthKit()
    {
        gun.suckUp.Play();
        notHealing = false;
        yield return new WaitForEndOfFrame();
        notHealing = true;
        HealHealth(40f);
    }

    public void Respawn()
    {
        notDead = false;
        survivedDeathOnce = false;
        health = maxHealth;
        rectHealthBar.sizeDelta = new Vector2(rectHealth, rectHealthBar.rect.height);
        gun.rectSlimeBar.sizeDelta = new Vector2(.5f * gun.rectSlime, gun.rectSlimeBar.rect.height);
        gun.amountOfSlime = 50f;
        characterController.enabled = false;
        gameObject.transform.position = currentSpawnPosition.transform.position;
        gameObject.transform.rotation = currentSpawnPosition.transform.rotation;
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
        }
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        for (int i = 0; i < slimes.Length; i++)
        {
            slimes[i].GetComponent<SlimeBehavior>().slimeSpawner.SlimeDeath();
            Destroy(slimes[i]);
        }
        GameObject[] slimeSpawnPipe = GameObject.FindGameObjectsWithTag("Slime Spawn Pipe");
        for (int i = 0; i < slimeSpawnPipe.Length; i++)
        {
            slimeSpawnPipe[i].GetComponent<SlimeSpawner>().restartPipe();
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Single Enemy Spawner");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<SingleEnemySpawn>().RespawnEnemy();
        }
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            boss.GetComponent<BossBehavior>().RestartFight();
        }
        playerAnim.SetBool("Dead", false);
        playerAnim.Play("Idle");
        playerMovement.enabled = true;
    }
}
