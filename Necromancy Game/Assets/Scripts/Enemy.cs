using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public short health = 100;
    public short maxHealth = 100;
    public short defence = 10;
    public float knockbackResistence = 1f;
    public float speedAcceleration = 1f;
    public float maxSpeed = 2f;
    public float deathTime = .5f;

    public GameObject corpsePrefab;
    public CircleCollider2D circleCollider;
    public Attack attack;
    public Transform attackBasisObject;

    public bool dead = false;
    private float xGoal = 0f;
    private float xGoal2 = -2f;
    public Transform goal;
    public bool inPresenceOfSkeleton = false;
    private bool inPresenceOfTower = false;
    public float skeletonAttackRange;
    private SpriteRenderer spriteRenderer;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inPresenceOfTower)
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
            if (dead)
            {
                
            }
            else if (transform.position.x < xGoal2)
            {
                //Vector2 destination = goal.position - transform.position;
                //if (destination.magnitude < skeletonAttackRange)
                if (true)
                {
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                        attack.gameObject.SetActive(true);
                        attack.StartAOEAttack();
                    }
                }
                else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                {
                    rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * Vector2.left;
                }
                else
                {
                    rigidbody.velocity += speedAcceleration * Vector2.left * Time.deltaTime;
                }
            }
            else if (transform.position.x < xGoal)
            {
                if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                {
                    rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * Vector2.left;
                }
                else
                {
                    rigidbody.velocity += speedAcceleration * Vector2.left * Time.deltaTime;
                }
            }
            else
            {
                inPresenceOfTower = false;
            }
        }
        else if (inPresenceOfSkeleton)
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
            Vector2 destination = goal.position - transform.position;
            if (dead)
            {
                //Debug.Log("Dead");
            }
            else if ((goal.CompareTag("Skeleton") && goal.GetComponent<Skeleton>().dead) || (goal.CompareTag("Minion") && goal.GetComponent<Minion>().dead))
            {
                //Debug.Log("They Dead");
                inPresenceOfSkeleton = false;
                goal = null;
            }
            else if (destination.magnitude < skeletonAttackRange)
            {
                //Debug.Log("Attack");
                if (!attack.currectlyAttacking)
                {
                    attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                    attack.gameObject.SetActive(true);
                    attack.StartAOEAttack();
                }
            }
            else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
            {
                //Debug.Log("Lowest Speed");
                rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
            }
            else
            {
                //Debug.Log("Normal Speed");
                rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
            }
        }
        else
        {
            if (transform.position.x <= xGoal)
            {
                inPresenceOfTower = true;
            }
            else if (-rigidbody.velocity.x < maxSpeed)
            {
                rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.left;
                if (-rigidbody.velocity.x >= maxSpeed)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
            }
            else if (-rigidbody.velocity.x > maxSpeed)
            {
                rigidbody.velocity -= speedAcceleration * Time.deltaTime * 3f * Vector2.right;
                if (rigidbody.velocity.x <= maxSpeed)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
            }
        }
        spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
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
                skeletonAttackRange = attack.attackRange + circleCollider.radius + (source.CompareTag("Skeleton") ? source.GetComponent<Skeleton>().circleCollider.radius : source.GetComponent<Minion>().circleCollider.radius);
        }
            health -= (short) (damage / defence);
            if (health <= 0)
            {
                StartCoroutine(Death());
            }
        }
    }

    IEnumerator Knockback(Vector3 attackCenter, float knockback)
    {
        float timer = 0f;
        Vector2 knockbackVector = new Vector2(transform.position.x - attackCenter.x, transform.position.y - attackCenter.y) * (knockback / knockbackResistence);
        float knockbackTime = knockbackVector.magnitude / 2f;
        while (timer < knockbackTime)
        {
            yield return new WaitForFixedUpdate();
            rigidbody.velocity += knockbackVector * ((knockbackTime - timer) * Time.deltaTime);
            timer += Time.deltaTime;
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(deathTime);
        Instantiate(corpsePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
