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
    public short boneUpgradeAmount = 25;//-1 for maxed out
    private float upgradeFactor = 1.5f;
    [HideInInspector] public short upgradeLevel = 1;
    public short bonesStored = 0;
    public short maxBones = 25;

    public CircleCollider2D circleCollider;
    public Attack attack;
    public Attack diggingAttack;
    public Transform attackBasisObject;
    public Transform digAttackBasisObject;
    public GameObject selectBars;
    public Transform sightObject;
    public Transform spriteBasisObject;
    public SpriteRenderer[] sprite;
    public SpriteRenderer[] weaponSprite;
    public SpriteRenderer[] weaponSpriteLevel2;
    public SpriteRenderer[] weaponSpriteLevel3;
    private int[] spritePos;

    private float usedBoneSpeedReductionFactor;
    public bool inDiggingMode;
    public bool dead = false;
    private float xGoal = 37f;
    private float xBaseGoal = -4f;
    public Transform goal;
    public Grave grave;
    private PlayerBase playerBase;
    public bool inPresenceOfEnemy = false;
    public float enemyAttackRange;
    SelectManager selectManager;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < upgradeLevel; i++)
        {
            attack.attackPower = (short)(attack.attackPower * upgradeFactor);
            diggingAttack.attackPower += 4;
            if (i != 2)
            {
                boneUpgradeAmount *= 2;
            }
            else
            {
                boneUpgradeAmount = -1;
            }
        }
        playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        inDiggingMode = selectManager.currentMinionDigStatus;
        if (selectManager.selectingObject && selectManager.selectedTroop == transform)
        {
            selectManager.minionStatus.sprite = inDiggingMode ? selectManager.minionDig : selectManager.minionAttack;
            if (boneUpgradeAmount == -1)
            {
                selectManager.boneCostObject0.SetActive(false);
            }
            else
            {
                selectManager.boneCostObject0.SetActive(true);
                selectManager.boneCostValue0.text = "-" + boneUpgradeAmount.ToString();
            }
            selectManager.shovelUpgradeButton.SetActive(boneUpgradeAmount != -1);
        }
        rigidbody = GetComponent<Rigidbody2D>();
        playerBase.numSkeletons++;
        selectManager.troopCapacityText.text = playerBase.numSkeletons + "\n" + playerBase.maxSkeletons;
        spritePos = new int[sprite.Length + 3];
        for (int i = 0; i < spritePos.Length - 3; i++)
        {
            spritePos[i] = sprite[i].sortingOrder;
        }
        spritePos[spritePos.Length - 3] = weaponSprite[0].sortingOrder;
        spritePos[spritePos.Length - 2] = weaponSpriteLevel2[0].sortingOrder;
        spritePos[spritePos.Length - 1] = weaponSpriteLevel3[0].sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        usedBoneSpeedReductionFactor = 1f - (boneSpeedReductionFactor * bonesStored / maxBones);
        if (inDiggingMode)
        {
            if (bonesStored < maxBones && goal != null)
            {
                if (goal.position.x - transform.position.x > 0)
                {
                    spriteBasisObject.localScale = new Vector3(0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    spriteBasisObject.localScale = new Vector3(-0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(-1f, 1f, 1f);
                }
                float futureX = (rigidbody.velocity.x * speedAcceleration / (usedBoneSpeedReductionFactor * 6f)) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / (usedBoneSpeedReductionFactor * 6f)) + transform.position.y;
                Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;

                //Near Grave
                if (futureDistence.magnitude < 1f)
                {
                    //Debug.Log("X:" + futureX + "\t\t\tY: " + futureY + "\t\t\tDistence: " + futureDistence);
                    futureDistence = futureDistence.normalized;
                    if (grave == null || grave.usedUp)
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
                            if (selectManager.selectedTroop == transform)
                            {
                                grave.targetSelect.SetActive(true);
                            }
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
                    else if (grave == null || grave.usedUp)
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
                            if (selectManager.selectedTroop == transform)
                            {
                                grave.targetSelect.SetActive(true);
                            }
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
                        if (selectManager.selectedTroop == transform)
                        {
                            grave.targetSelect.SetActive(true);
                        }
                    }
                }

                //At Base
                if (transform.position.x < xBaseGoal)
                {
                    spriteBasisObject.localScale = new Vector3(0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(1f, 1f, 1f);
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
                    spriteBasisObject.localScale = new Vector3(-0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(-1f, 1f, 1f);
                    anim.SetBool("Running", true);
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
            if (!inPresenceOfEnemy)
            {
                spriteBasisObject.localScale = new Vector3(0.15f, 0.15f, 1f);
                sightObject.localScale = new Vector3(1f, 1f, 1f);
                if (transform.position.x >= xGoal)
                {
                    if (transform.position.x >= xGoal+3f)
                    {
                        spriteBasisObject.localScale = new Vector3(-0.15f, 0.15f, 1f);
                        sightObject.localScale = new Vector3(-1f, 1f, 1f);
                        anim.SetBool("Running", true);
                        if (rigidbody.velocity.x > -maxSpeed)
                        {
                            rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * Vector2.left;
                            if (rigidbody.velocity.x <= -maxSpeed * usedBoneSpeedReductionFactor)
                            {
                                rigidbody.velocity = new Vector2(-maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                            }
                        }
                        else if (rigidbody.velocity.x < -maxSpeed * usedBoneSpeedReductionFactor)
                        {
                            rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * 3f * Vector2.right;
                            if (rigidbody.velocity.x >= -maxSpeed * usedBoneSpeedReductionFactor)
                            {
                                rigidbody.velocity = new Vector2(-maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                            }
                        }
                    }
                    else
                    {
                        anim.SetBool("Running", false);
                        if (rigidbody.velocity.magnitude != 0f)
                        {
                            Vector2 norm = rigidbody.velocity.normalized;
                            rigidbody.velocity -= norm * speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
                            if (norm != rigidbody.velocity.normalized)
                            {
                                rigidbody.velocity = Vector2.zero;
                            }
                        }
                    }
                }
                else if (rigidbody.velocity.x < maxSpeed * usedBoneSpeedReductionFactor)
                {
                    anim.SetBool("Running", true);
                    rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * Vector2.right;
                    if (rigidbody.velocity.x >= maxSpeed * usedBoneSpeedReductionFactor)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                    }
                }
                else if (rigidbody.velocity.x > maxSpeed * usedBoneSpeedReductionFactor)
                {
                    anim.SetBool("Running", true);
                    rigidbody.velocity += speedAcceleration * usedBoneSpeedReductionFactor * Time.deltaTime * 3f * Vector2.left;
                    if (rigidbody.velocity.x <= maxSpeed * usedBoneSpeedReductionFactor)
                    {
                        rigidbody.velocity = new Vector2(maxSpeed * usedBoneSpeedReductionFactor, rigidbody.velocity.y);
                    }
                }
            }
            else if (goal == null)
            {
                goal = null;
                inPresenceOfEnemy = false;
            }
            else if ((transform.position - goal.position).magnitude < enemyAttackRange)
            {
                if (goal.position.x - transform.position.x > 0)
                {
                    spriteBasisObject.localScale = new Vector3(0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    spriteBasisObject.localScale = new Vector3(-0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(-1f, 1f, 1f);
                }
                if (rigidbody.velocity.magnitude != 0f)
                {
                    Vector2 norm = rigidbody.velocity.normalized;
                    rigidbody.velocity -= norm * speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                if (!attack.currectlyAttacking)
                {
                    anim.SetTrigger("Attack");
                    attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                    attack.gameObject.SetActive(true);
                    attack.StartSingleAttack();
                }
            }
            else /*if (inPresenceOfEnemy)*/
            {
                float futureX = (rigidbody.velocity.x * speedAcceleration / (6f * usedBoneSpeedReductionFactor)) + transform.position.x;
                float futureY = (rigidbody.velocity.y * speedAcceleration / (6f * usedBoneSpeedReductionFactor)) + transform.position.y;
                Vector2 futureDistence = new Vector3(futureX, futureY, 0f) - goal.position;
                if (goal.position.x - transform.position.x > 0)
                {
                    spriteBasisObject.localScale = new Vector3(0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    spriteBasisObject.localScale = new Vector3(-0.15f, 0.15f, 1f);
                    sightObject.localScale = new Vector3(-1f, 1f, 1f);
                }

                //Near Enemy
                if (futureDistence.magnitude >= (transform.position - goal.position).magnitude / 2f)
                {
                    futureDistence = futureDistence.normalized * (enemyAttackRange - .3f);
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
                            x = -speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                            x = speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                            y = -speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
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
                            y = speedAcceleration * usedBoneSpeedReductionFactor * 3f * Time.deltaTime;
                        }
                        else
                        {
                            walkY = true;
                        }
                    }

                    anim.SetBool("Running", true);
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
                        anim.SetBool("Running", false);
                        rigidbody.velocity += new Vector2(x, y);
                    }
                }
                else
                {
                    anim.SetBool("Running", true);
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
            }
        }
        for (int i = 0; i < spritePos.Length - 3; i++)
        {
            sprite[i].sortingOrder = (int)(transform.position.y * -100) + spritePos[i];
        }
        weaponSprite[0].sortingOrder = (int)(transform.position.y * -100) + spritePos[spritePos.Length - 3];
        weaponSpriteLevel2[0].sortingOrder = (int)(transform.position.y * -100) + spritePos[spritePos.Length - 2];
        weaponSpriteLevel3[0].sortingOrder = (int)(transform.position.y * -100) + spritePos[spritePos.Length - 1];
    }

    public void Upgrade()
    {
        playerBase.UpdateBones((short)-boneUpgradeAmount);
        upgradeLevel++;
        attack.attackPower = (short)(attack.attackPower * upgradeFactor);
        diggingAttack.attackPower += 4;
        if (upgradeLevel != 3)
        {
            weaponSprite[0].gameObject.SetActive(false);
            weaponSpriteLevel2[0].gameObject.SetActive(true);

            boneUpgradeAmount *= 2;
            selectManager.boneCostValue0.text = "-" + boneUpgradeAmount.ToString();
        }
        else
        {
            weaponSpriteLevel2[0].gameObject.SetActive(false);
            weaponSpriteLevel3[0].gameObject.SetActive(true);

            boneUpgradeAmount = -1;
            selectManager.boneCostObject0.SetActive(false);
            selectManager.shovelUpgradeButton.SetActive(false);
        }
    }

    public void Hit(Vector3 attackCenter, Transform source, float knockback, short damage)
    {
        if (health > 0)
        {
            StartCoroutine(Knockback(attackCenter, knockback));
            if (!inPresenceOfEnemy && source != null && !inDiggingMode)
            {
                inPresenceOfEnemy = true;
                goal = source;
                if (selectManager.selectedTroop == transform)
                {
                    source.GetComponent<Enemy>().targetSelect.SetActive(true);
                }
                enemyAttackRange = attack.attackRange + circleCollider.radius + source.GetComponent<Enemy>().circleCollider.radius;
            }
            health -= (short)(damage / defence);
            if (health <= 0)
            {
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
        float knockbackTime = Mathf.Pow((knockback / knockbackResistence), .25f) / 4f;
        while (timer < knockbackTime)
        {
            yield return new WaitForFixedUpdate();
            rigidbody.velocity += knockbackVector * ((knockbackTime - timer) * Time.deltaTime);
            timer += Time.deltaTime;
        }
    }

    IEnumerator Death()
    {
        if (selectManager.selectedTroop == transform)
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
        GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().SpawnCoffin((short)(graveBones+bonesStored), transform.position);
        Destroy(gameObject);
    }
}
