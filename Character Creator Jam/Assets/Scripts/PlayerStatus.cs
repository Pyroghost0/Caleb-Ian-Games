using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{

    public bool isMale;
    public int headNumber;
    public int skinColor;
    public int hairColor;

    public float health = 100f;
    public float maxHealth = 100f;
    public bool invincible = false;
    public float invincibleTime = 1.5f;
    public GameObject healthBar;
    private RectTransform rectHealthBar;
    private float rectHealth;
    private CharacterController characterController;
    private bool notDead = true;

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

    // Start is called before the first frame update
    void Start()
    {
        rectHealthBar = healthBar.GetComponent<RectTransform>();
        rectHealth = rectHealthBar.rect.width;
        characterController = gameObject.GetComponent<CharacterController>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
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
            emptyEquipment[0].SetActive(false);
            emptyEquipment[3].SetActive(false);
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
            if (isMale)
            {
                id += 12;
            }
            selfEquipment[id].SetActive(true);
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
                if (health +1f < maxHealth)
                {
                    HealHealth(1f);
                }
                else
                {
                    HealHealth(maxHealth-health);
                }
            }
        }
    }

    public void HealHealth(float addedHealth)
    {
        health += addedHealth;
        rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
    }
    
    IEnumerator UpdateHealthBar(float healthChange)
    {
        notDead = true;
        if (health + healthChange <= 0)
        {
            Debug.Log("Dead");
            if (isTutorial)
            {
                AsyncOperation ao1 = SceneManager.LoadSceneAsync("Mech Level", LoadSceneMode.Additive);
                AsyncOperation ao2 = SceneManager.UnloadSceneAsync("Tutorial");
                yield return new WaitUntil(() => ao1.isDone && ao2.isDone);
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
                if (isMale)
                {
                    emptyEquipment[3].SetActive(true);
                }
                else
                {
                    emptyEquipment[0].SetActive(true);
                }
            }
            else
            {
                if (equipedEquipment[9] && !survivedDeathOnce)
                {
                    survivedDeathOnce = true;
                    yield return new WaitForSeconds(invincibleTime);
                    invincible = false;
                }
                else
                {
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
            while (timer < invincibleTimeWhenHit && notDead)
            {
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
            invincible = false;
        }
    }

    public void Respawn()
    {
        notDead = false;
        survivedDeathOnce = false;
        health = maxHealth;
        rectHealthBar.sizeDelta = new Vector2(rectHealth, rectHealthBar.rect.height);
        characterController.enabled = false;
        gameObject.transform.position = currentSpawnPosition.transform.position;
        gameObject.transform.rotation = currentSpawnPosition.transform.rotation;
        GameObject[] slimes = GameObject.FindGameObjectsWithTag("Slime");
        for (int i = 0; i < slimes.Length; i++)
        {
            slimes[i].GetComponent<SlimeBehavior>().slimeSpawner.SlimeDeath();
            Destroy(slimes[i]);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Single Enemy Spawner");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<SingleEnemySpawn>().RespawnEnemy();
        }
    }
}
