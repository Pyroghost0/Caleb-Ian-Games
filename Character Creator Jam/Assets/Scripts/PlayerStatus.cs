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

    // Start is called before the first frame update
    void Start()
    {
        rectHealthBar = healthBar.GetComponent<RectTransform>();
        rectHealth = rectHealthBar.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (!invincible)
        {
            invincible = true;
            StartCoroutine(UpdateHealthBar(-damage));
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
