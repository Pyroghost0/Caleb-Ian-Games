using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public bool invincible = false;
    public float invincibleTime = 2f;
    public GameObject healthBar;
    private RectTransform rectHealthBar;
    private float rectHealth;
    private CharacterController characterController;

    public GameObject[] selfEquipment;
    public GameObject[] equipmentPrefabs;
    public bool[] equipedEquipment;
    public GameObject[] emptyEquipment;
    private PlayerMovement playerMovement;
    private Gun gun;

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
            if (equipment.clothingStyle == "Mech")
            {
                gun.suckDistence *= 1.5f;
                equipedEquipment[0] = true;
                selfEquipment[0].SetActive(true);
            }
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
            if (equipment.clothingStyle == "Mech")
            {
                gun.suckPower *= 1.5f;
                equipedEquipment[1] = true;
                selfEquipment[1].SetActive(true);
            }
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
            if (equipment.clothingStyle == "Mech")
            {
                playerMovement.jumpHeight *= 2f;
                equipedEquipment[2] = true;
                selfEquipment[2].SetActive(true);
            }
        }
        Destroy(equipment.gameObject);
    }

    private void Dequip(int id, Vector3 position)
    {
        equipedEquipment[id] = false;
        selfEquipment[id].SetActive(false);
        if (id == 0)
        {//Mech Helmet
            gun.suckDistence /= 1.5f;
        }
        else if (id == 1)
        {//Mech Body
            gun.suckPower /= 1.5f;
        }
        else if (id == 2)
        {//Mech Legs
            playerMovement.jumpHeight /= 2f;
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
            StartCoroutine(Knockback(direction * knockback));
            StartCoroutine(UpdateHealthBar(-damage));
        }
    }

    IEnumerator Knockback(Vector3 movement)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            characterController.Move(((1f - timer) / 1f) * movement * Time.deltaTime);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }

    public void HealHealth(float health)
    {
        health += health;
    }
    
    IEnumerator UpdateHealthBar(float healthChange)
    {
        health += healthChange;
        rectHealthBar.sizeDelta = new Vector2((health / maxHealth) * rectHealth, rectHealthBar.rect.height);
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }

    public void Respawn()
    {
        characterController.enabled = false;
        gameObject.transform.position = currentSpawnPosition.transform.position;
        gameObject.transform.rotation = currentSpawnPosition.transform.rotation;
        characterController.enabled = true;
    }
}
