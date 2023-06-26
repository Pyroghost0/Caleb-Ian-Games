using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public short health = 100;
    public short maxHealth = 100;
    public short defence = 10;
    public float knockbackResistence = 1f;
    public float speedAcceleration = 1f;
    public float maxSpeed = 2f;
    public float deathTime = .5f;
    public short enemyValue = 5;
    public AttackType attackType = AttackType.AOE;

    public GameObject targetSelect;
    public GameObject corpsePrefab;
    public CircleCollider2D circleCollider;
    public Attack attack;
    public Transform attackBasisObject;
    public Transform sightObject;
    public Transform spriteBasisObject;
    private int[] spritePos;
    public SpriteRenderer[] sprite;

    private float spriteMultiplier;
    public bool reversedSprite = false;
    //public bool dead = false;
    private float slopeGoal = 1.6f;
    private float posYGoal = 8f;
    public Transform goal;
    public bool inPresenceOfSkeleton = false;
    public bool inPresenceOfTower = false;
    public float skeletonAttackRange;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        spriteMultiplier = spriteBasisObject.localScale.y;
        rigidbody = GetComponent<Rigidbody2D>();
        spritePos = new int[sprite.Length];
        for (int i = 0; i < sprite.Length; i++)
        {
            spritePos[i] = sprite[i].sortingOrder;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inPresenceOfTower)
        {
            anim.SetBool("Running", false);
            if (rigidbody.velocity.magnitude != 0f)
            {
                Vector2 norm = rigidbody.velocity.normalized;
                rigidbody.velocity -= norm * speedAcceleration * 3f * Time.deltaTime;
                if (norm != rigidbody.velocity.normalized)
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
            /*if (dead)
            {
                
            }
            else*/ if (transform.position.y > (slopeGoal * (transform.position.x - attack.attackRange)) + posYGoal)
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? -spriteMultiplier : spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? -1f : 1f, 1f, 1f);
                if (!attack.currectlyAttacking)
                {
                    anim.SetTrigger("Attack");
                    attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    attack.gameObject.SetActive(true);
                    attack.StartAttack(attackType);
                }
            }
            else
            {
                inPresenceOfTower = false;
            }
        }
        else if (!inPresenceOfSkeleton)
        {
            if (transform.position.y > (slopeGoal * (transform.position.x - attack.attackRange)) + posYGoal)
            {
                inPresenceOfTower = true;
            }
            else
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? -spriteMultiplier : spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? -1f : 1f, 1f, 1f);
                anim.SetBool("Running", true);
                if (rigidbody.velocity.x > -maxSpeed)
                {
                    rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.left;
                    if (rigidbody.velocity.x <= -maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                    }
                }
                else if (rigidbody.velocity.x < -maxSpeed)
                {
                    rigidbody.velocity += speedAcceleration * Time.deltaTime * 3f * Vector2.right;
                    if (rigidbody.velocity.x >= -maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                    }
                }
            }
        }
        else if (goal == null || (goal.CompareTag("Skeleton") && goal.GetComponent<Skeleton>().dead) || (goal.CompareTag("Minion") && goal.GetComponent<Minion>().dead))
        {
            goal = null;
            inPresenceOfSkeleton = false;
        }
        else if ((transform.position - goal.position).magnitude < skeletonAttackRange)
        {
            if ((transform.position - goal.position).x < 0)
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? spriteMultiplier : -spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? 1f : -1f, 1f, 1f);
            }
            else
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? -spriteMultiplier : spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? -1f : 1f, 1f, 1f);
            }
            if (rigidbody.velocity.magnitude != 0f)
            {
                Vector2 norm = rigidbody.velocity.normalized;
                rigidbody.velocity -= norm * speedAcceleration * 3f * Time.deltaTime;
                if (norm != rigidbody.velocity.normalized)
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
            anim.SetBool("Running", false);
            /*if (dead)
            {
                //Debug.Log("Dead");
            }
            else*/ if (!attack.currectlyAttacking)
            {
                anim.SetTrigger("Attack");
                attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                attack.gameObject.SetActive(true);
                attack.StartAttack(attackType);
            }
        }
        else /*if (inPresenceOfSkeleton)*/
        {
            if ((transform.position - goal.position).x < 0)
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? spriteMultiplier : -spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? 1f : -1f, 1f, 1f);
            }
            else
            {
                spriteBasisObject.localScale = new Vector3(reversedSprite ? -spriteMultiplier : spriteMultiplier, spriteMultiplier, 1f);
                sightObject.localScale = new Vector3(reversedSprite ? -1f : 1f, 1f, 1f);
            }
            float futureX = (rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x;
            float futureY = (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y;
            Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;

            //Near Enemy
            if (futureDistence.magnitude >= (transform.position - goal.position).magnitude / 2f)
            {
                futureDistence = futureDistence.normalized * (skeletonAttackRange - .3f);
                float x = 0f;
                bool walkX = false;
                if (Mathf.Abs(rigidbody.velocity.x) < .1f && transform.position.x > goal.position.x - Mathf.Abs(futureDistence.x) - .1f && transform.position.x < goal.position.x + Mathf.Abs(futureDistence.x) + .1f)
                {
                    x = -rigidbody.velocity.x;
                }
                else if (rigidbody.velocity.x > 0)
                {
                    if (futureX >= goal.position.x - Mathf.Abs(futureDistence.x) - .1f)
                    {
                        x = -speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        walkX = true;
                    }
                }
                else /*if(rigidbody.velocity.x <= 0)*/
                {
                    if (futureX <= goal.position.x + Mathf.Abs(futureDistence.x) + .1f)
                    {
                        x = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        walkX = true;
                    }
                }

                float y = 0f;
                bool walkY = false;
                if (Mathf.Abs(rigidbody.velocity.y) < .1f && transform.position.y > goal.position.y - Mathf.Abs(futureDistence.y) - .1f && transform.position.y < goal.position.y + Mathf.Abs(futureDistence.y) + .1f)
                {
                    y = -rigidbody.velocity.y;
                }
                else if (rigidbody.velocity.y > 0)
                {
                    if (futureY >= goal.position.y - Mathf.Abs(futureDistence.y) - .1f)
                    {
                        y = -speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        walkY = true;
                    }
                }
                else /*if(rigidbody.velocity.y <= 0)*/
                {
                    if (futureY <= goal.position.y + Mathf.Abs(futureDistence.y) + .1f)
                    {
                        y = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        walkY = true;
                    }
                }

                anim.SetBool("Running", true);
                if (walkX && walkY)
                {
                    Vector2 distenceMoved = new Vector2(goal.position.x - transform.position.x, goal.position.y - transform.position.y).normalized * speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += distenceMoved;
                    if ((distenceMoved.x > 0f && rigidbody.velocity.x > 0f) || (distenceMoved.x < 0f && rigidbody.velocity.x < 0f))
                    {
                        if ((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f))
                        {
                            if (rigidbody.velocity.magnitude > maxSpeed)
                            {
                                rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
                            }
                        }
                        else if (Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
                        {
                            rigidbody.velocity *= new Vector2(maxSpeed * (distenceMoved.x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                        }
                    }
                    else if (((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(1f, maxSpeed * (distenceMoved.y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                    }
                }
                else if (walkX)
                {
                    x = goal.position.x - transform.position.x > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += new Vector2(x, y);
                    if (((x > 0f && rigidbody.velocity.x > 0f) || (x < 0f && rigidbody.velocity.x < 0f)) && Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(maxSpeed * (x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                    }
                }
                else if (walkY)
                {
                    y = goal.position.y - transform.position.y > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += new Vector2(x, y);
                    if (((y > 0f && rigidbody.velocity.y > 0f) || (y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(1f, maxSpeed * (y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                    }
                }
                else
                {
                    anim.SetBool("Running", false);
                    rigidbody.velocity += new Vector2(x, y);
                }
            }
            else
            {
                anim.SetBool("Running", true);
                Vector2 distenceMoved = new Vector2(goal.position.x - transform.position.x, goal.position.y - transform.position.y).normalized * speedAcceleration * Time.deltaTime;
                rigidbody.velocity += distenceMoved;
                if ((distenceMoved.x > 0f && rigidbody.velocity.x > 0f) || (distenceMoved.x < 0f && rigidbody.velocity.x < 0f))
                {
                    if ((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f))
                    {
                        if (rigidbody.velocity.magnitude > maxSpeed)
                        {
                            rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
                        }
                    }
                    else if (Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(maxSpeed * (distenceMoved.x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                    }
                }
                else if (((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed)
                {
                    rigidbody.velocity *= new Vector2(1f, maxSpeed * (distenceMoved.y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                }
            }
            /*if (rigidbody.velocity.magnitude != 0f)
            {
                Vector2 norm = rigidbody.velocity.normalized;
                rigidbody.velocity -= norm * speedAcceleration * 3f * Time.deltaTime;
                if (norm != rigidbody.velocity.normalized)
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
            Vector2 destination = goal != null ? goal.position - transform.position : Vector2.zero;
            if (dead)
            {
                //Debug.Log("Dead");
            }
            else if (goal == null || goal.GetComponent<Enemy>().dead)
            {
                //Debug.Log("They Dead");
                inPresenceOfEnemy = false;
                goal = null;
            }
            else if (destination.magnitude < enemyAttackRange)
            {
                anim.SetBool("Running", false);
                //Debug.Log("Attack");
                if (!attack.currectlyAttacking)
                {
                    anim.SetTrigger("Attack");
                    attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                    attack.gameObject.SetActive(true);
                    attack.StartAttack(attackType);
                }
            }
            else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
            {
                anim.SetBool("Running", true);
                //Debug.Log("Lowest Speed");
                rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
            }
            else
            {
                anim.SetBool("Running", true);
                //Debug.Log("Normal Speed");
                rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
            }*/
        }
        /*else if (inPresenceOfSkeleton)
        {
            if (rigidbody.velocity.magnitude != 0f)
            {
                Vector2 norm = rigidbody.velocity.normalized;
                rigidbody.velocity -= norm * speedAcceleration * 3f * Time.deltaTime;
                if (norm != rigidbody.velocity.normalized)
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
            Vector2 destination = goal != null ? goal.position - transform.position : Vector2.zero; ;
            if (dead)
            {
                //Debug.Log("Dead");
            }
            else if (goal == null || (goal.CompareTag("Skeleton") && goal.GetComponent<Skeleton>().dead) || (goal.CompareTag("Minion") && goal.GetComponent<Minion>().dead))
            {
                //Debug.Log("They Dead");
                inPresenceOfSkeleton = false;
                goal = null;
            }
            else if (destination.magnitude < skeletonAttackRange)
            {
                anim.SetBool("Running", false);
                //Debug.Log("Attack");
                if (!attack.currectlyAttacking)
                {
                    anim.SetTrigger("Attack");
                    attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                    attack.gameObject.SetActive(true);
                    attack.StartAttack(attackType);
                }
            }
            else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
            {
                anim.SetBool("Running", true);
                //Debug.Log("Lowest Speed");
                rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
            }
            else
            {
                anim.SetBool("Running", true);
                //Debug.Log("Normal Speed");
                rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
            }
        }
        else
        {
            if (transform.position.y > (slopeGoal * (transform.position.x - attack.attackRange)) + posYGoal)
            {
                anim.SetBool("Running", false);
                inPresenceOfTower = true;
            }
            else if (-rigidbody.velocity.x < maxSpeed)
            {
                anim.SetBool("Running", true);
                rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.left;
                if (-rigidbody.velocity.x >= maxSpeed)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
            }
            else if (-rigidbody.velocity.x > maxSpeed)
            {
                anim.SetBool("Running", true);
                rigidbody.velocity -= speedAcceleration * Time.deltaTime * 3f * Vector2.right;
                if (rigidbody.velocity.x <= maxSpeed)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
            }
        }*/
        for (int i = 0; i < sprite.Length; i++)
        {
            sprite[i].sortingOrder = (int)(transform.position.y * -100) + spritePos[i];
        }
    }

    public void Hit(Vector3 attackCenter, Transform source, float knockback, short damage)
    {
        if (health > 0)
        {
            StartCoroutine(Knockback(attackCenter, knockback));
            if (!inPresenceOfSkeleton && source != null)
            {
                inPresenceOfSkeleton = true;
                goal = source;
                skeletonAttackRange = attack.attackRange + (source.CompareTag("Skeleton") ? source.GetComponent<Skeleton>().circleCollider.radius : source.GetComponent<Minion>().circleCollider.radius);
            }
            health -= (short) (damage / defence);
            if (health <= 0)
            {
                //dead = true;
                targetSelect.SetActive(false);
                Instantiate(corpsePrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Knockback(Vector3 attackCenter, float knockback)
    {
        float timer = 0f;
        Vector2 knockbackVector = new Vector2(transform.position.x - attackCenter.x, transform.position.y - attackCenter.y) * (knockback / knockbackResistence);
        float knockbackTime = Mathf.Pow((knockback / knockbackResistence), .25f) / 4f;
        while (timer < knockbackTime)
        {
            yield return new WaitForFixedUpdate();
            rigidbody.velocity += knockbackVector * ((knockbackTime - timer) * Time.deltaTime);
            timer += Time.deltaTime;
        }
    }

    /*IEnumerator Death()
    {
        targetSelect.SetActive(false);
        yield return new WaitForSeconds(deathTime);
        Instantiate(corpsePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }*/
}
