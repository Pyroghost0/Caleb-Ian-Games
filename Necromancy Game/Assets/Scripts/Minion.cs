using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public short health = 100;
    public short maxHealth = 100;
    public short defence = 10;
    public float knockbackResistence = 1f;
    public float speedAcceleration = 1f;
    public float maxSpeed = 2f;
    public float boneSpeedReductionFactor = 2f/3f;
    public float deathTime = .5f;
    public short graveBones = 25;
    public short bonesStored = 0;
    public short maxBones = 25;

    public CircleCollider2D circleCollider;
    public Attack attack;
    public Attack diggingAttack;
    public Transform attackBasisObject;
    public Transform digAttackBasisObject;

    private float usedBoneSpeedReductionFactor;
    public bool inDiggingMode;
    public bool dead = false;
    private float xGoal = 0f;
    private float xGoal2 = -2f;
    private float xBaseGoal = -4f;
    public Transform goal;
    public Grave grave;
    private PlayerBase playerBase;
    public bool inPresenceOfEnemy = false;
    private bool inPresenceOfTower = false;
    public float enemyAttackRange;
    SelectManager selectManager;
    private SpriteRenderer spriteRenderer;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Start is called before the first frame update
    void Start()
    {
        playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        inDiggingMode = selectManager.currentMinionDigStatus;
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBase.numSkeletons++;
        selectManager.troopCapacityText.text = playerBase.numSkeletons + "\n" + playerBase.maxSkeletons;
    }

    // Update is called once per frame
    void Update()
    {
        usedBoneSpeedReductionFactor = 1f - (boneSpeedReductionFactor * bonesStored / maxBones);
        if (inDiggingMode)
        {
            if (bonesStored < maxBones && goal != null)
            {
                float futureX = (rigidbody.velocity.x * speedAcceleration / (usedBoneSpeedReductionFactor * 6f)) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / (usedBoneSpeedReductionFactor * 6f)) + transform.position.y;
                Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;

                //Near Grave
                if (futureDistence.magnitude < 1f)
                {
                    //Debug.Log("X:" + futureX + "\t\t\tY: " + futureY + "\t\t\tDistence: " + futureDistence);
                    futureDistence = futureDistence.normalized;
                    if (grave.usedUp)
                    {
                        GameObject[] graves = GameObject.FindGameObjectsWithTag("Grave");
                        int num = -1;
                        float smallestDistence = 99f;
                        for (int i = 0; i < graves.Length; i++)
                        {
                            if (!graves[i].GetComponent<Grave>().usedUp && (graves[i].transform.position - transform.position).magnitude < smallestDistence)
                            {
                                num = i;
                                smallestDistence = (graves[i].transform.position - transform.position).magnitude;
                            }
                        }
                        if (num != -1)
                        {
                            goal = graves[num].transform;
                            grave = graves[num].GetComponent<Grave>();
                        }
                    }
                    if (goal != null)
                    {
                        float x = 0f;
                        bool walkX = false;
                        if (Mathf.Abs(rigidbody.velocity.x) < .1f && transform.position.x > goal.position.x - Mathf.Abs(futureDistence.x) - .1f && transform.position.x < goal.position.x + Mathf.Abs(futureDistence.x) + .1f)
                        {
                            //Debug.Log("None");
                            x = -rigidbody.velocity.x;
                        }
                        else if (rigidbody.velocity.x > 0)
                        {
                            if (futureX >= goal.position.x - Mathf.Abs(futureDistence.x) - .1f)
                            {
                                //Debug.Log("Right: Decelorate");
                                x = -speedAcceleration * 3f * usedBoneSpeedReductionFactor * Time.deltaTime;
                            }
                            else
                            {
                                //Debug.Log("Right: Walk");
                                walkX = true;
                            }
                        }
                        else /*if(rigidbody.velocity.x <= 0)*/
                        {
                            if (futureX <= goal.position.x + Mathf.Abs(futureDistence.x) + .1f)
                            {
                                //Debug.Log("Left: Decelorate");
                                x = speedAcceleration *  3f * usedBoneSpeedReductionFactor * Time.deltaTime;
                            }
                            else
                            {
                                //Debug.Log("Left: Walk");
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
                                y = -speedAcceleration * 3f * usedBoneSpeedReductionFactor * Time.deltaTime;
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
                                y = speedAcceleration * 3f * usedBoneSpeedReductionFactor * Time.deltaTime;
                            }
                            else
                            {
                                walkY = true;
                            }
                        }

                        if (walkX && walkY)
                        {
                            Vector2 distenceMoved = new Vector2(goal.position.x - transform.position.x, goal.position.y - transform.position.y).normalized * speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime;
                            rigidbody.velocity += distenceMoved;
                            if ((distenceMoved.x > 0f && rigidbody.velocity.x > 0f) || (distenceMoved.x < 0f && rigidbody.velocity.x < 0f))
                            {
                                if ((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f))
                                {
                                    if (rigidbody.velocity.magnitude > maxSpeed * usedBoneSpeedReductionFactor)
                                    {
                                        rigidbody.velocity *= maxSpeed * usedBoneSpeedReductionFactor / rigidbody.velocity.magnitude;
                                    }
                                }
                                else if (Mathf.Abs(rigidbody.velocity.x) > maxSpeed * usedBoneSpeedReductionFactor)
                                {
                                    rigidbody.velocity *= new Vector2(maxSpeed * usedBoneSpeedReductionFactor * (distenceMoved.x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                                }
                            }
                            else if (((distenceMoved.y > 0f && rigidbody.velocity.y > 0f) || (distenceMoved.y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed * usedBoneSpeedReductionFactor)
                            {
                                rigidbody.velocity *= new Vector2(1f, maxSpeed * usedBoneSpeedReductionFactor * (distenceMoved.y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                            }
                        }
                        else if (walkX)
                        {
                            x = goal.position.x - transform.position.x > 0f ? speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime : -speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime;
                            rigidbody.velocity += new Vector2(x, y);
                            if (((x > 0f && rigidbody.velocity.x > 0f) || (x < 0f && rigidbody.velocity.x < 0f)) && Mathf.Abs(rigidbody.velocity.x) > maxSpeed * usedBoneSpeedReductionFactor)
                            {
                                rigidbody.velocity *= new Vector2(maxSpeed * usedBoneSpeedReductionFactor * (x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                            }
                        }
                        else if (walkY)
                        {
                            y = goal.position.y - transform.position.y > 0f ? speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime : -speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime;
                            rigidbody.velocity += new Vector2(x, y);
                            if (((y > 0f && rigidbody.velocity.y > 0f) || (y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed * usedBoneSpeedReductionFactor)
                            {
                                rigidbody.velocity *= new Vector2(1f, maxSpeed * usedBoneSpeedReductionFactor * (y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                            }
                        }
                        else
                        {
                            rigidbody.velocity += new Vector2(x, y);
                            if (rigidbody.velocity.magnitude == 0 && !diggingAttack.currectlyAttacking)
                            {
                                digAttackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                                diggingAttack.gameObject.SetActive(true);
                                diggingAttack.StartDigAttack((bonesStored + diggingAttack.attackPower > maxBones ? (short)(maxBones - bonesStored) : diggingAttack.attackPower), grave);
                            }
                        }
                    }
                }

                //Moving to grave
                else
                {
                    if (dead)
                    {
                        //Debug.Log("Dead");
                    }
                    else if (grave.usedUp)
                    {
                        GameObject[] graves = GameObject.FindGameObjectsWithTag("Grave");
                        int num = -1;
                        float smallestDistence = 99f;
                        for (int i = 0; i < graves.Length; i++)
                        {
                            if (!graves[i].GetComponent<Grave>().usedUp && (graves[i].transform.position - transform.position).magnitude < smallestDistence)
                            {
                                num = i;
                                smallestDistence = (graves[i].transform.position - transform.position).magnitude;
                            }
                        }
                        if (num == -1)
                        {
                            goal = null;
                            grave = null;
                        }
                        else
                        {
                            goal = graves[num].transform;
                            grave = graves[num].GetComponent<Grave>();
                        }
                    }
                    else
                    {
                        //Debug.Log("Normal Speed");
                        Vector2 destination = goal.position - transform.position;
                        rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * destination.normalized * Time.deltaTime;
                        if (rigidbody.velocity.magnitude > maxSpeed * usedBoneSpeedReductionFactor)
                        {
                            rigidbody.velocity *= maxSpeed * usedBoneSpeedReductionFactor / rigidbody.velocity.magnitude;
                        }
                    }
                }
            }
            else
            {
                if (goal == null)
                {
                    GameObject[] graves = GameObject.FindGameObjectsWithTag("Grave");
                    int num = -1;
                    float smallestDistence = 99f;
                    for (int i = 0; i < graves.Length; i++)
                    {
                        if (!graves[i].GetComponent<Grave>().usedUp && (graves[i].transform.position - transform.position).magnitude < smallestDistence)
                        {
                            num = i;
                            smallestDistence = (graves[i].transform.position - transform.position).magnitude;
                        }
                    }
                    if (num != -1)
                    {
                        goal = graves[num].transform;
                        grave = graves[num].GetComponent<Grave>();
                    }
                }

                //At Base
                if (transform.position.x < xBaseGoal)
                {
                    if (bonesStored > 0)
                    {
                        playerBase.UpdateBones(bonesStored);
                        bonesStored = 0;
                    }
                    if (goal != null)
                    {
                        if (dead)
                        {
                            //Debug.Log("Dead");
                        }
                        else if (grave.usedUp)
                        {
                            //Debug.Log("Reset Grave");
                            grave = null;
                            goal = null;
                        }
                        else
                        {
                            //Debug.Log("Normal Speed");
                            Vector2 destination = goal.position - transform.position;
                            rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
                            if (rigidbody.velocity.magnitude > maxSpeed)
                            {
                                rigidbody.velocity *= rigidbody.velocity.magnitude / maxSpeed;
                            }
                        }
                    }
                    else if (rigidbody.velocity.magnitude != 0f)
                    {
                        Vector2 norm = rigidbody.velocity.normalized;
                        rigidbody.velocity -= norm * speedAcceleration * 3f * Time.deltaTime;
                        if (norm != rigidbody.velocity.normalized)
                        {
                            rigidbody.velocity = Vector2.zero;
                        }
                    }
                }

                //Full and moving back
                else
                {
                    if (Mathf.Abs(rigidbody.velocity.y) < .1f)
                    {
                        rigidbody.velocity += new Vector2(-speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime, -rigidbody.velocity.y);
                    }
                    else if (rigidbody.velocity.y > 0)
                    {
                        rigidbody.velocity += new Vector2(-speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime, -speedAcceleration * 3f * Time.deltaTime);
                    }
                    else /*if(rigidbody.velocity.y <= 0)*/
                    {
                        rigidbody.velocity += new Vector2(-speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime, speedAcceleration * 3f * Time.deltaTime);
                    }
                    if (rigidbody.velocity.x < -maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(-maxSpeed * usedBoneSpeedReductionFactor / rigidbody.velocity.x, 1f);
                    }
                }
            }
        }

        //Attack Mode
        else
        {
            if (inPresenceOfTower)
            {//Dont forget boneSpeedReductionFactor
                if (rigidbody.velocity.magnitude != 0f)
                {
                    Vector2 norm = rigidbody.velocity.normalized;
                    rigidbody.velocity -= norm * speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                            attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                            attack.gameObject.SetActive(true);
                            attack.StartAOEAttack();
                        }
                    }
                    else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                    {
                        rigidbody.velocity += ((maxSpeed * usedBoneSpeedReductionFactor / 3f) - rigidbody.velocity.magnitude) * Vector2.right;
                    }
                    else
                    {
                        rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Vector2.right * Time.deltaTime;
                    }
                }
                else if (transform.position.x > xGoal)
                {
                    if (rigidbody.velocity.magnitude < maxSpeed / 3f)
                    {
                        rigidbody.velocity += ((maxSpeed * usedBoneSpeedReductionFactor / 3f) - rigidbody.velocity.magnitude) * Vector2.right;
                    }
                    else
                    {
                        rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Vector2.right * Time.deltaTime;
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
                    rigidbody.velocity -= norm * speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                else if (goal.GetComponent<Enemy>().dead)
                {
                    //Debug.Log("They Dead");
                    inPresenceOfEnemy = false;
                    goal = null;
                }
                else if (destination.magnitude < enemyAttackRange)
                {
                    //Debug.Log("Attack");
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAOEAttack();
                    }
                }
                else if (rigidbody.velocity.magnitude < maxSpeed * usedBoneSpeedReductionFactor / 3f)
                {
                    //Debug.Log("Lowest Speed");
                    rigidbody.velocity += ((maxSpeed * usedBoneSpeedReductionFactor / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
                }
                else
                {
                    //Debug.Log("Normal Speed");
                    rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * destination.normalized * Time.deltaTime;
                }
            }
            else
            {
                if (transform.position.x >= xGoal)
                {
                    inPresenceOfTower = true;
                }
                else if (rigidbody.velocity.x < maxSpeed * usedBoneSpeedReductionFactor)
                {
                    rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * Vector2.right;
                    if (rigidbody.velocity.x >= maxSpeed * usedBoneSpeedReductionFactor)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                    }
                }
                else if (rigidbody.velocity.x > maxSpeed * usedBoneSpeedReductionFactor)
                {
                    rigidbody.velocity -= speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * 3f * Vector2.left;
                    if (rigidbody.velocity.x <= maxSpeed * usedBoneSpeedReductionFactor)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                    }
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
            if (!inPresenceOfEnemy && source != null)
            {
                inPresenceOfEnemy = true;
                goal = source;
                enemyAttackRange = attack.attackRange + circleCollider.radius + source.GetComponent<Enemy>().circleCollider.radius;
            }
            health -= (short)(damage / defence);
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
        playerBase.numSkeletons--;
        selectManager.troopCapacityText.text = playerBase.numSkeletons + "\n" + playerBase.maxSkeletons;
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGrave((short)(graveBones+bonesStored), transform.position);
        Destroy(gameObject);
    }
}
