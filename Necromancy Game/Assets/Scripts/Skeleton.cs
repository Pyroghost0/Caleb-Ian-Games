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
    public short graveBones = 15;
    public short attackBoneUpgradeAmount = 25;//-1 for maxed out
    public short defenceBoneUpgradeAmount = 25;
    private float attackUpgradeFactor = 1.5f;
    private float defenceUpgradeFactor = 1.5f;
    private short attackLevel = 1;
    private short armorLevel = 1;
    public string skeletonName = "Skeleton";
    public AttackType attackType = AttackType.AOE;

    public CircleCollider2D circleCollider;
    public Attack attack;
    public Transform attackBasisObject;
    public GameObject selectBars;
    public Transform sightObject;
    public Transform spriteBasisObject;
    public SpriteRenderer[] sprite;
    private int[] spritePos;

    private float spriteMultiplier;
    public SkeletonMode skeletonMode = SkeletonMode.stay;
    public bool dead = false;
    private float xGoal = 37f;
    private float xBaseGoal = -3f;
    public Transform goal;
    public Vector3 stayGoal;
    public bool inPresenceOfEnemy = false;
    public float enemyAttackRange;
    private PlayerBase playerBase;
    private SelectManager selectManager;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Animator anim;

    public float superWaitTime = 25f;
    public float currentSuperWaitTime = 0f;
    public Attack[] superAttack;
    public bool instantSuper;
    public AttackType specialAttackType;
    private bool nextAttackIsSuper;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("Skeleton", true);
        spriteMultiplier = spriteBasisObject.localScale.y;
        rigidbody = GetComponent<Rigidbody2D>();
        stayGoal = transform.position;
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        skeletonMode = selectManager.currentSkeletonMode;
        if (selectManager.selectingObject && selectManager.selectedTroop == transform)
        {
            selectManager.minionStatus.sprite = skeletonMode == SkeletonMode.left ?selectManager.skeletonRun : (skeletonMode == SkeletonMode.stay ? selectManager.skeletonStay : selectManager.skeletonAttack);
            selectManager.stayPositionMarker.position = stayGoal;
        }
        playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        playerBase.numSkeletons++;
        selectManager.troopCapacityText.text = playerBase.numSkeletons + "\n" + playerBase.maxSkeletons;
        spritePos = new int[sprite.Length];
        for (int i = 0; i < sprite.Length; i++)
        {
            spritePos[i] = sprite[i].sortingOrder;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move Right Mode
        if (skeletonMode == SkeletonMode.right)
        {
            if (!inPresenceOfEnemy)
            {
                if (transform.position.x >= xGoal)
                {
                    anim.SetBool("Running", false);
                    //inPresenceOfTower = true;
                    stayGoal = new Vector3(xGoal, transform.position.y, 0f);
                    skeletonMode = SkeletonMode.stay;
                    selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
                    if (selectManager.selectingObject == transform)
                    {
                        selectManager.stayPositionMarker.gameObject.SetActive(true);
                        selectManager.stayPositionMarker.position = transform.position;
                    }
                }
                else if (rigidbody.velocity.x < maxSpeed)
                {
                    anim.SetBool("Running", true);
                    rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.right;
                    if (rigidbody.velocity.x >= maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
                    }
                }
                else if (rigidbody.velocity.x > maxSpeed)
                {
                    anim.SetBool("Running", true);
                    rigidbody.velocity -= speedAcceleration * Time.deltaTime * 3f * Vector2.left;
                    if (rigidbody.velocity.x <= maxSpeed)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
                    }
                }
            }
            else if (goal == null || goal.GetComponent<Enemy>().dead)
            {
                goal = null;
                inPresenceOfEnemy = false;
            }
            else if ((transform.position - goal.position).magnitude < attack.attackRange)
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
                anim.SetBool("Running", false);
                if (dead)
                {
                    //Debug.Log("Dead");
                }
                else if (!attack.currectlyAttacking)
                {
                    if (nextAttackIsSuper)
                    {
                        anim.SetTrigger("Attack");
                        superAttack[0].transform.parent.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((superAttack[0].transform.parent.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        nextAttackIsSuper = false;
                        StartCoroutine(SpecialAttackCoroutineCooldown());
                        for (int i = 0; i < superAttack.Length; i++)
                        {
                            superAttack[i].gameObject.SetActive(true);
                            superAttack[i].StartSuperAttack(specialAttackType);
                        }
                    }
                    else
                    {
                        anim.SetTrigger("Attack");
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack(attackType);
                    }
                }
            }
            else /*if (inPresenceOfEnemy)*/
            {
                float futureX = (rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y;
                Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;

                //Near Enemy
                if (futureDistence.magnitude >= (transform.position - goal.position).magnitude / 2f)
                {
                    futureDistence = futureDistence.normalized * (attack.attackRange - .3f);
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
        }

        //Stay Mode
        else if (skeletonMode == SkeletonMode.stay)
        {
            if (!inPresenceOfEnemy)
            {
                float futureX = (rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y;

                float x = 0f;
                bool walkX = false;
                if (Mathf.Abs(rigidbody.velocity.x) < .2f && transform.position.x > stayGoal.x - .3f && transform.position.x < stayGoal.x + .3f)
                {
                    x = -rigidbody.velocity.x;
                }
                else if (rigidbody.velocity.x > 0)
                {
                    if (futureX >= stayGoal.x - .3f)
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
                    if (futureX <= stayGoal.x + .3f)
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
                if (Mathf.Abs(rigidbody.velocity.y) < .2f && transform.position.y > stayGoal.y - .3f && transform.position.y < stayGoal.y + .3f)
                {
                    y = -rigidbody.velocity.y;
                }
                else if (rigidbody.velocity.y > 0)
                {
                    if (futureY >= stayGoal.y - .3f)
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
                    if (futureY <= stayGoal.y + .3f)
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
                    Vector2 distenceMoved = new Vector2(stayGoal.x - transform.position.x, stayGoal.y - transform.position.y).normalized * speedAcceleration * Time.deltaTime;
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
                    x = stayGoal.x - transform.position.x > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += new Vector2(x, y);
                    if (((x > 0f && rigidbody.velocity.x > 0f) || (x < 0f && rigidbody.velocity.x < 0f)) && Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(maxSpeed * (x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                    }
                }
                else if (walkY)
                {
                    y = stayGoal.y - transform.position.y > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
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
            else if (goal == null || goal.GetComponent<Enemy>().dead || Mathf.Abs(goal.position.x - transform.position.x) > 8f)
            {
                goal = null;
                inPresenceOfEnemy = false;
            }
            else if ((transform.position - goal.position).magnitude < attack.attackRange)
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
                anim.SetBool("Running", false);
                if (dead)
                {
                    //Debug.Log("Dead");
                }
                else if (!attack.currectlyAttacking)
                {
                    if (nextAttackIsSuper)
                    {
                        anim.SetTrigger("Attack");
                        superAttack[0].transform.parent.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((superAttack[0].transform.parent.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        nextAttackIsSuper = false;
                        StartCoroutine(SpecialAttackCoroutineCooldown());
                        for (int i = 0; i < superAttack.Length; i++)
                        {
                            superAttack[i].gameObject.SetActive(true);
                            superAttack[i].StartSuperAttack(specialAttackType);
                        }
                    }
                    else
                    {
                        anim.SetTrigger("Attack");
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack(attackType);
                    }
                }
            }
            else /*if (inPresenceOfEnemy)*/
            {
                float futureX = (rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y;
                Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;

                //Near Enemy
                if (futureDistence.magnitude >= (transform.position - goal.position).magnitude / 2f)
                {
                    futureDistence = futureDistence.normalized * (attack.attackRange - .3f);
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
            }

            /*if (inPresenceOfEnemy && (goal == null || goal.GetComponent<Enemy>().dead))
            {
                goal = null;
                inPresenceOfEnemy = false;
            }
            else if (inPresenceOfEnemy && Mathf.Abs(goal.position.x - transform.position.x) < 8f)
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
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
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
                if (inPresenceOfEnemy)
                {
                    goal = null;
                    inPresenceOfEnemy = false;
                }
                float x = 0f;
                bool walkX = false;
                if (Mathf.Abs(rigidbody.velocity.x) < .1f && transform.position.x > stayGoal.x -.1f && transform.position.x < stayGoal.x + .1f)
                {
                    anim.SetBool("Running", false);
                    //Debug.Log("None");
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
                        anim.SetBool("Running", true);
                        //Debug.Log("Right: Walk");
                        walkX = true;
                    }
                }
                else /*if(rigidbody.velocity.x <= 0)*/
                /*{
                    if ((rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x - stayGoal.x < .1f)
                    {
                        //Debug.Log("Left: Decelorate");
                        x = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        anim.SetBool("Running", true);
                        //Debug.Log("Left: Walk");
                        walkX = true;
                    }
                }

                float y = 0f;
                bool walkY = false;
                if (Mathf.Abs(rigidbody.velocity.y) < .1f && transform.position.y > stayGoal.y - .1f && transform.position.y < stayGoal.y + .1f)
                {
                    anim.SetBool("Running", false);
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
                        anim.SetBool("Running", true);
                        walkY = true;
                    }
                }
                else /*if(rigidbody.velocity.y <= 0)*/
                /*{
                    if ((rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y - stayGoal.y < .1f)
                    {
                        y = speedAcceleration * 3f * Time.deltaTime;
                    }
                    else
                    {
                        anim.SetBool("Running", true);
                        walkY = true;
                    }
                }

                if (walkX && walkY)
                {
                    Vector2 distenceMoved = new Vector2(stayGoal.x - transform.position.x, stayGoal.y - transform.position.y).normalized * speedAcceleration * Time.deltaTime;
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
                    x = stayGoal.x - transform.position.x > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += new Vector2(x, y);
                    if (((x > 0f && rigidbody.velocity.x > 0f) || (x < 0f && rigidbody.velocity.x < 0f)) && Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(maxSpeed * (x > 0 ? 1f : -1f) / rigidbody.velocity.x, 1f);
                    }
                }
                else if (walkY)
                {
                    y = stayGoal.y - transform.position.y > 0f ? speedAcceleration * Time.deltaTime : -speedAcceleration * Time.deltaTime;
                    rigidbody.velocity += new Vector2(x, y);
                    if (((y > 0f && rigidbody.velocity.y > 0f) || (y < 0f && rigidbody.velocity.y < 0f)) && Mathf.Abs(rigidbody.velocity.y) > maxSpeed)
                    {
                        rigidbody.velocity *= new Vector2(1f, maxSpeed * (y > 0 ? 1f : -1f) / rigidbody.velocity.y);
                    }
                }
                else
                {
                    rigidbody.velocity += new Vector2(x, y);
                }
                /*else
                {
                    rigidbody.velocity += new Vector2((rigidbody.velocity.x * speedAcceleration / 6f) + transform.position.x - stayGoal.x, (rigidbody.velocity.y * speedAcceleration / 6f) + transform.position.y - stayGoal.y).normalized * 3f * -speedAcceleration * Time.deltaTime;
                }*/
            //}
        }

        //Left Mode
        else /*if (skeletonMode == SkeletonMode.left)*/
        {
            if (transform.position.x < xBaseGoal)
            {
                anim.SetBool("Running", false);
                skeletonMode = SkeletonMode.stay;
                selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
                stayGoal = transform.position;
                if (selectManager.selectingObject == transform)
                {
                    selectManager.stayPositionMarker.gameObject.SetActive(true);
                    selectManager.stayPositionMarker.position = transform.position;
                }
            }
            else
            {
                anim.SetBool("Running", true);
                if (Mathf.Abs(rigidbody.velocity.y) < .1f)
                {
                    rigidbody.velocity += new Vector2(-speedAcceleration * Time.deltaTime, -rigidbody.velocity.y);
                }
                else if (rigidbody.velocity.y > 0)
                {
                    rigidbody.velocity += new Vector2(-speedAcceleration * Time.deltaTime, -speedAcceleration * 3f * Time.deltaTime);
                }
                else /*if(rigidbody.velocity.y <= 0)*/
                {
                    rigidbody.velocity += new Vector2(-speedAcceleration * Time.deltaTime, speedAcceleration * 3f * Time.deltaTime);
                }
                if (rigidbody.velocity.x < -maxSpeed)
                {
                    rigidbody.velocity *= new Vector2(-maxSpeed / rigidbody.velocity.x, 1f);
                }
            }
        }

        if (rigidbody.velocity.x > 0)
        {
            spriteBasisObject.localScale = new Vector3(spriteMultiplier, spriteMultiplier, 1f);
            sightObject.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rigidbody.velocity.x < 0)
        {
            spriteBasisObject.localScale = new Vector3(-spriteMultiplier, spriteMultiplier, 1f);
            sightObject.localScale = new Vector3(-1f, 1f, 1f);
        }
        for (int i = 0; i < sprite.Length; i++)
        {
            sprite[i].sortingOrder = (int)(transform.position.y * -10) + spritePos[i];
        }
    }

    public void SpecialAttack()
    {
        if ((specialAttackType == AttackType.SpecialGoblinArrow && !selectManager.specialGoblin) || (specialAttackType == AttackType.SpecialWolfShadowMovement && !selectManager.specialWolf) || (specialAttackType == AttackType.SpecialWitchGravityAttack && !selectManager.specialWitch) || (specialAttackType == AttackType.SpecialOrcUpgrade && !selectManager.specialOrc) || (specialAttackType == AttackType.SpecialOgreMultiattack && !selectManager.specialOgre))
        {

        }
        else if (currentSuperWaitTime == 0f)
        {
            selectManager.rectSpecialCooldownBar.gameObject.SetActive(true);
            selectManager.rectSpecialCooldownBar.sizeDelta = new Vector2(selectManager.rectSpecialCooldown, selectManager.rectSpecialCooldownBar.rect.height);
            currentSuperWaitTime = superWaitTime;
            if (instantSuper)
            {
                if (specialAttackType == AttackType.SpecialWolfShadowMovement)
                {
                    StartCoroutine(WolfSuper());
                }
                else /*if(specialAttackType == AttackType.SpecialOrcUpgrade)*/
                {
                    GameObject[] graves = GameObject.FindGameObjectsWithTag("Grave");
                    int closestGraveIndex = 0;
                    for (int i = 1; i < graves.Length; i++)
                    {
                        if ((graves[i].transform.position - transform.position).magnitude < (graves[closestGraveIndex].transform.position - transform.position).magnitude)
                        {
                            closestGraveIndex = i;
                        }
                    }
                    if (graves.Length > 0 && (graves[closestGraveIndex].transform.position - transform.position).magnitude < 3.5f)
                    {
                        graves[closestGraveIndex].GetComponent<Grave>().DestroyGrave();
                        if (attackLevel == 1)
                        {
                            attack.attackPower += 10;
                        }
                        else if (attackLevel == 2)
                        {
                            attack.attackPower += 15;
                        }
                        else /*if (attackLevel == 3)*/
                        {
                            attack.attackPower += 23;
                        }
                        if (armorLevel == 1)
                        {
                            defence += 1;
                        }
                        else if (armorLevel == 2)
                        {
                            defence += 1;
                        }
                        else /*if (armorLevel == 3)*/
                        {
                            defence += 2;
                        }
                    }
                }
                StartCoroutine(SpecialAttackCoroutineCooldown());
            }
            else
            {
                nextAttackIsSuper = true;
                StartCoroutine(NextAttackIsSuperWait());
            }
        }
        else
        {
            InvalidNotice notice = Instantiate(selectManager.impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Skeleton Super In Cooldown";
            notice.textPosition.anchoredPosition = new Vector2(265f, 150f);
        }
    }

    IEnumerator NextAttackIsSuperWait()
    {
        yield return new WaitForSeconds(5f + attack.attackCooldown);
        if (nextAttackIsSuper)
        {
            nextAttackIsSuper = false;
            StartCoroutine(SpecialAttackCoroutineCooldown());
        }
    }

    IEnumerator SpecialAttackCoroutineCooldown()
    {
        for (int i = 0; i < superWaitTime * 2; i++)
        {
            yield return new WaitForSeconds(.5f);
            currentSuperWaitTime -= .5f;
            if (selectManager.selectedTroop == transform)
            {
                selectManager.rectSpecialCooldownBar.sizeDelta = new Vector2(((float)currentSuperWaitTime / superWaitTime) * selectManager.rectSpecialCooldown, selectManager.rectSpecialCooldownBar.rect.height);
            }
        }
        if (selectManager.selectedTroop == transform)
        {
            selectManager.rectSpecialCooldownBar.gameObject.SetActive(false);
        }
    }

    IEnumerator WolfSuper()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            for (int i = 0; i < sprite.Length-1; i++)
            {
                sprite[i].color = new Color(1f - timer, 1f - timer, 1f - timer, 1f - timer);
            }
            sprite[sprite.Length-1].color = new Color(1f - timer, 1f - timer, 1f - timer, (1f - timer)/2f);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int leftmostEnemyIndex = 0;
        for (int i = 1; i < enemies.Length; i++)
        {
            if (enemies[i].transform.position.x < enemies[leftmostEnemyIndex].transform.position.x)
            {
                leftmostEnemyIndex = i;
            }
        }
        if (enemies.Length != 0)
        {
            goal = enemies[leftmostEnemyIndex].transform;
            transform.position = new Vector3(goal.transform.position.x + goal.GetComponent<Enemy>().circleCollider.radius + circleCollider.radius + .5f, goal.position.y, 0f);
        }

        timer = 0f;
        while (timer < 1f)
        {
            for (int i = 0; i < sprite.Length-1; i++)
            {
                sprite[i].color = new Color(timer, timer, timer, timer);
            }
            sprite[sprite.Length - 1].color = new Color(timer, timer, timer, timer / 2f);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
        for (int i = 0; i < sprite.Length-1; i++)
        {
            sprite[i].color = Color.white;
        }
        sprite[sprite.Length - 1].color = new Color(1f, 1f, 1f, .5f);
    }

    public void UpgradeDefence()
    {
        playerBase.UpdateBones((short)-defenceBoneUpgradeAmount);
        armorLevel++;
        defence =(short) (defence * defenceUpgradeFactor);
        if (armorLevel != 3)
        {
            defenceBoneUpgradeAmount *= 2;
            selectManager.boneCostValue1.text = "-" + defenceBoneUpgradeAmount.ToString();
        }
        else
        {
            defenceBoneUpgradeAmount = -1;
            selectManager.boneCostObject1.SetActive(false);
            selectManager.defenceUpgradeButton.SetActive(false);
        }
    }

    public void TurnIntoTombstone()
    {
        dead = true;
        StartCoroutine(Death());
    }

    public void UpgradeAttack()
    {
        playerBase.UpdateBones((short)-attackBoneUpgradeAmount);
        attackLevel++;
        attack.attackPower = (short)(attack.attackPower * attackUpgradeFactor);
        if (armorLevel != 3)
        {
            attackBoneUpgradeAmount *= 2;
            selectManager.boneCostValue2.text = "-" + defenceBoneUpgradeAmount.ToString();
        }
        else
        {
            attackBoneUpgradeAmount = -1;
            selectManager.boneCostObject2.SetActive(false);
            selectManager.attackUpgradeButton.SetActive(false);
        }
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
                if (selectManager.selectedTroop == transform)
                {
                    source.GetComponent<Enemy>().targetSelect.SetActive(true);
                }
                enemyAttackRange = attack.attackRange + source.GetComponent<Enemy>().circleCollider.radius;
            }
            health -= (short)(damage / defence);
            if (health <= 0)
            {
                health = 0;
                StartCoroutine(Death());
            }
            if (selectManager.selectedTroop == transform)
            {
                selectManager.rectHealthBar.sizeDelta = new Vector2(((float)health / maxHealth) * selectManager.rectHealth, selectManager.rectHealthBar.rect.height);
                selectManager.healthValue.text = health + "\n" + maxHealth;
            }
        }
    }

    IEnumerator Knockback(Vector3 attackCenter, float knockback)
    {
        float timer = 0f;
        Vector2 knockbackVector = new Vector2(transform.position.x - attackCenter.x, transform.position.y - attackCenter.y) * (knockback / knockbackResistence);
        //Debug.Log("Attack Center: " + attackCenter + "\t\t\tKnockback: " + knockback + "\t\t\tKnockback Vector: " + knockbackVector);
        float knockbackTime = knockbackVector.magnitude / 2f;
        while (timer < knockbackTime)
        {
            yield return new WaitForFixedUpdate();
            rigidbody.velocity += knockbackVector * ((knockbackTime - timer) * Time.deltaTime);
            timer += Time.deltaTime;
            //Debug.Log("Knockback Time: " + knockbackTime + "\t\t\tTimer: " + timer + "Velocity: " + rigidbody.velocity);
        }
    }

    IEnumerator Death()
    {
        if (selectManager.selectedTroop  == transform)
        {
            selectManager.SelectedTroopDestroyed();
        }
        else
        {
            for (int i = 0; i < selectManager.selectableObjects.Count; i++)
            {
                if (selectManager.selectableObjects[i] == transform)
                {
                    selectManager.selectableObjects.RemoveAt(i);
                    break;
                }
            }
        }
        yield return new WaitForSeconds(deathTime);
        playerBase.numSkeletons--;
        selectManager.troopCapacityText.text = playerBase.numSkeletons + "\n" + playerBase.maxSkeletons;
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnGrave((short)(graveBones * (1f + (health / maxHealth))), transform.position);
        Destroy(gameObject);
    }
}
