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

    // Start is called before the first frame update
    void Start()
    {
        rectHealthBar = healthBar.GetComponent<RectTransform>();
        rectHealth = rectHealthBar.rect.width;
        characterController = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
