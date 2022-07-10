using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonMode
{
    left = 0,
    stay = 1,
    right = 2
}

public class Skeleton : MonoBehaviour
{
    public short health = 100;
    public short maxHealth = 100;
    public short defence = 10;
    public float knockbackResistence = 1f;
    public float speedAcceleration = 1f;
    public float maxSpeed = 2f;
    public float deathTime = .5f;

    public CircleCollider2D circleCollider;
    public Attack attack;
    public Transform attackBasisObject;

    public SkeletonMode skeletonMode = SkeletonMode.stay;
    public bool dead = false;
    private float xGoal = 60f;
    private float xGoal2 = 62f;
    private float xBaseGoal = 2f;
    private float xBaseGoal2 = 0f;
    public Transform goal;
    public Vector3 stayGoal;
    public bool inPresenceOfEnemy = false;
    private bool inPresenceOfTower = false;
    public float enemyAttackRange;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        stayGoal = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (skeletonMode == SkeletonMode.right)
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
                else if (transform.position.x > xGoal2)
                {
                    //Vector2 destination = goal.position - transform.position;
                    //if (destination.magnitude < skeletonAttackRange)
                    if (true)
                    {
                        if (!attack.currectlyAttacking)
                        {
                            attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                            attack.gameObject.SetActive(true);
                            attack.StartAttack();
                        }
                    }
                    else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                    {
                        rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * Vector2.right;
                    }
                    else
                    {
                        rigidbody.velocity += speedAcceleration * Vector2.right * Time.deltaTime;
                    }
                }
                else if (transform.position.x > xGoal)
                {
                    if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                    {
                        rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * Vector2.right;
                    }
                    else
                    {
                        rigidbody.velocity += speedAcceleration * Vector2.right * Time.deltaTime;
                    }
                }
                else
                {
                    inPresenceOfTower = false;
                }
            }
            else if (inPresenceOfEnemy)
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

                }
                else if (!goal.GetComponent<Enemy>().dead)
                {
                    inPresenceOfEnemy = false;
                    goal = null;
                }
                else if (destination.magnitude < enemyAttackRange)
                {
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack();
                    }
                }
                else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                {
                    rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
                }
                else
                {
                    rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
                }
            }
            else
            {
                if (transform.position.x >= xGoal)
                {
                    inPresenceOfTower = true;
                }
                else if (rigidbody.velocity.x < maxSpeed)
                {
                    rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.right;
                    if (rigidbody.velocity.x >= maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
                    }
                }
                else if (rigidbody.velocity.x > maxSpeed)
                {
                    rigidbody.velocity -= speedAcceleration * Time.deltaTime * 3f * Vector2.left;
                    if (rigidbody.velocity.x <= maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
                    }
                }
            }
        }


        else if (skeletonMode == SkeletonMode.stay)
        {
            if (inPresenceOfEnemy)
            {

                /*if (rigidbody.velocity.magnitude != 0f)
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

                }
                else if ((goal.CompareTag("Skeleton") && !goal.GetComponent<Skeleton>().dead) || false)
                {
                    inPresenceOfEnemy = false;
                    goal = null;
                }
                else if (destination.magnitude < enemyAttackRange)
                {
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack();
                    }
                }
                else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                {
                    rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
                }
                else
                {
                    rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
                }*/
            }
            else
            {
                bool unUsed = false;
                float x = 0f;
                bool walkX = false;
                if (Mathf.Abs(rigidbody.velocity.x) < .1f && transform.position.x > stayGoal.x -.1f && transform.position.x < stayGoal.x + .1f)
                {
                    //Debug.Log("None");
                    unUsed = true;
                    x = -rigidbody.velocity.x;
                }
                else if (rigidbody.velocity.x > 0)
                {
                    if ((rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x - stayGoal.x > -.1f)
                    {
                        //Debug.Log("Right: Decelorate");
                        x = -speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        //Debug.Log("Right: Walk");
                        walkX = true;
                    }
                }
                else /*if(rigidbody.velocity.x <= 0)*/
                {
                    if ((rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x - stayGoal.x < .1f)
                    {
                        //Debug.Log("Left: Decelorate");
                        x = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        //Debug.Log("Left: Walk");
                        walkX = true;
                    }
                }

                float y = 0f;
                bool walkY = false;
                if (Mathf.Abs(rigidbody.velocity.y) < .1f && transform.position.y > stayGoal.y - .1f && transform.position.y < stayGoal.y + .1f)
                {
                    unUsed = true;
                    y = -rigidbody.velocity.y;
                }
                else if (rigidbody.velocity.y > 0)
                {
                    if ((rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y - stayGoal.y > -.1f)
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
                    if ((rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y - stayGoal.y < .1f)
                    {
                        y = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        walkY = true;
                    }
                }

                if (walkX && walkY)
                {
                    rigidbody.velocity += new Vector2(stayGoal.x - transform.position.x, stayGoal.y - transform.position.y).normalized * speedAcceleration * Time.deltaTime;
                }
                else if (walkX)
                {
                    rigidbody.velocity += new Vector2(stayGoal.x - transform.position.x > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime, y);
                }
                else if (walkY)
                {
                    rigidbody.velocity += new Vector2(x, stayGoal.y - transform.position.y > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime);
                }
                else if (unUsed)
                {
                    rigidbody.velocity += new Vector2(x, y);
                }
                else
                {
                    rigidbody.velocity += new Vector2((rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x - stayGoal.x, (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y - stayGoal.y).normalized * 3f * -speedAcceleration * Time.deltaTime;
                }
            }
        }
        else /*if (skeletonMode == SkeletonMode.left)*/
        {
            if (transform.position.x <= xGoal)
            {
                inPresenceOfTower = true;
            }
            else if (inPresenceOfEnemy)
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

                }
                else if ((goal.CompareTag("Skeleton") && !goal.GetComponent<Skeleton>().dead) || false)
                {
                    inPresenceOfEnemy = false;
                    goal = null;
                }
                else if (destination.magnitude < enemyAttackRange)
                {
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack();
                    }
                }
                else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                {
                    rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
                }
                else
                {
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
            }
        }
    }

    public bool Hit(Vector3 attackCenter, Transform source, float knockback, short damage)
    {
        if (health > 0)
        {
            StartCoroutine(Knockback(attackCenter, knockback));
            if (!inPresenceOfEnemy && source != null)
            {
                inPresenceOfEnemy = true;
                goal = source;
            }
            health -= (short)(damage / defence);
            if (health <= 0)
            {
                StartCoroutine(Death());
                return true;
            }
            return false;
        }
        return true;
    }

    IEnumerator Knockback(Vector3 attackCenter, float knockback)
    {
        float timer = 0f;
        Vector2 knockbackVector = new Vector2((transform.position - attackCenter).x, (transform.position - attackCenter).y) * (knockback / knockbackResistence);
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
        Destroy(gameObject);
    }
}
