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
    private float xBaseGoal = 2f;
    public Transform goal;
    public Vector3 stayGoal;
    public bool inPresenceOfEnemy = false;
    private bool inPresenceOfTower = false;
    public float enemyAttackRange;
    private PlayerBase playerBase;
    private SelectManager selectManager;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Start is called before the first frame update
    void Start()
    {
        spriteMultiplier = spriteBasisObject.localScale.y;
        rigidbody = GetComponent<Rigidbody2D>();
        stayGoal = transform.position;
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        skeletonMode = selectManager.currentSkeletonMode;
        if (selectManager.selectingObject && selectManager.selectedTroop == transform)
        {
            selectManager.minionStatus.sprite = skeletonMode == SkeletonMode.left ?selectManager.skeletonRun : (skeletonMode == SkeletonMode.stay ? selectManager.skeletonStay : selectManager.skeletonAttack);
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
            /*if (inPresenceOfTower)
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
                else if (transform.position.x > xGoal)
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
            else*/ if (inPresenceOfEnemy)
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
                    //Debug.Log("Attack");
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack(attackType);
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
                if (transform.position.x >= xGoal)
                {
                    //inPresenceOfTower = true;
                    stayGoal = new Vector3(xGoal, transform.position.y, 0f);
                    skeletonMode = SkeletonMode.stay;
                    selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
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

        //Stay Mode
        else if (skeletonMode == SkeletonMode.stay)
        {
            if (inPresenceOfEnemy && (goal == null || goal.GetComponent<Enemy>().dead))
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
                    //Debug.Log("Attack");
                    if (!attack.currectlyAttacking)
                    {
                        attackBasisObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2((attackBasisObject.transform.position - goal.position).y, (transform.position - goal.position).x) * 57.2958f));
                        attack.gameObject.SetActive(true);
                        attack.StartAttack(attackType);
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
                if (inPresenceOfEnemy)
                {
                    goal = null;
                    inPresenceOfEnemy = false;
                }
                float x = 0f;
                bool walkX = false;
                if (Mathf.Abs(rigidbody.velocity.x) < .1f && transform.position.x > stayGoal.x -.1f && transform.position.x < stayGoal.x + .1f)
                {
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
            }
        }

        //Left Mode
        else /*if (skeletonMode == SkeletonMode.left)*/
        {
            if (transform.position.x < xBaseGoal)
            {
                skeletonMode = SkeletonMode.stay;
                selectManager.skeletonStatus.sprite = selectManager.skeletonStay;
                stayGoal = transform.position;
            }
            else
            {
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
