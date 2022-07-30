using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    AOE = 0,
    Projectile = 1,
    Single = 2,
    Dig = 3,
    RangedAOE = 4,
    ArrowBarrage = 5,
    SpecialGoblinArrow = 6,
    SpecialWolfShadowMovement = 7,
    SpecialWitchGravityAttack = 8,
    SpecialOrcUpgrade = 9,
    SpecialOgreMultiattack = 10
}

public class Attack : MonoBehaviour
{
    public short attackPower = 50;
    public float attackTime = 1f;
    public float attackSpeed = 4f;
    public float attackCooldown = .4f;
    public float attackRange = 1f;
    public float knockback = 1f;
    public bool affectsEnemies = false;
    public bool affectsSkeletons = false;
    //public bool projectilePierces = false;

    public GameObject source;
    public bool currectlyAttacking = false;
    public SpriteRenderer spriteRenderer;
    private List<GameObject> targets = new List<GameObject>();

    private float slopeBase = 1.6f;
    private float posYBase = 8f;
    private bool sort = true;

    private void Update()
    {
        if (sort)
        {
            spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
        }
    }

    public void StartSuperAttack(AttackType attackType)
    {
        if (attackType == AttackType.SpecialGoblinArrow)
        {
            StartCoroutine(StartSpecialGoblinArrowAttack());
        }
        /*else if (attackType == AttackType.SpecialWolfShadowMovement)
        {
            StartCoroutine(());
        }*/
        else if (attackType == AttackType.SpecialWitchGravityAttack)
        {
            StartCoroutine(StartSpecialWitchGravityAttack());
        }
        else if (attackType == AttackType.SpecialOrcUpgrade)
        {
            StartCoroutine(StartSpecialOrcAttack());
        }
        else /*if (attackType == AttackType.SpecialOgreMultiattack)*/
        {
            StartCoroutine(StartSpecialOgreAttack());
        }
    }

    public void StartAttack(AttackType attackType)
    {
        if (attackType == AttackType.AOE)
        {
            StartCoroutine(StartAOEAttackCorutine());
        }
        else if (attackType == AttackType.Projectile)
        {
            StartCoroutine(StartProjectileAttackCorutine());
        }
        else if (attackType == AttackType.Single)
        {
            StartCoroutine(StartSingleAttackCorutine());
        }
        else if (attackType == AttackType.RangedAOE)
        {
            StartCoroutine(StartRangedAOEAttackCorutine());
        }
        /*else if (attackType == AttackType.Dig)
        {
            //StartCoroutine(StartDigAttackCorutine());
        }
        else if (attackType == AttackType.ArrowBarrage)
        {

        }*/
    }

    public void StartProjectileAttack()
    {
        StartCoroutine(StartProjectileAttackCorutine());
    }

    IEnumerator StartProjectileAttackCorutine()
    {
        Vector3 startPosition = transform.parent.localPosition;
        gameObject.SetActive(true);
        spriteRenderer.enabled = true;
        if (GetComponent<Animator>())
		{
            GetComponent<Animator>().SetTrigger("Attack");
		}
        transform.parent.parent = null;
        currectlyAttacking = true;
        targets.Clear();
        float timer = 0f;
        bool done = false;
        while (timer < attackTime && !done)
        {
            timer += Time.deltaTime;
            transform.position += transform.right * -attackSpeed * Time.deltaTime;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {

                }
                else if (affectsEnemies && targets[i].CompareTag("Enemy"))
                {
                    targets[i].GetComponent<Enemy>().Hit(transform.position, source == null || source.CompareTag("Enemy") ? null : source.transform, knockback, attackPower);
                    done = true;
                    break;
                }
                else if (affectsSkeletons)
                {
                    if (targets[i].CompareTag("Skeleton"))
                    {
                        targets[i].GetComponent<Skeleton>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Minion"))
                    {
                        targets[i].GetComponent<Minion>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Player Base"))
                    {
                        targets[i].GetComponent<PlayerBase>().Hit(attackPower);
                        done = true;
                        break;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        spriteRenderer.enabled = false;
        if (source != null)
        {
            transform.parent.parent = source.transform;
            transform.parent.transform.localPosition = startPosition;
            transform.localPosition = Vector3.zero;
            if (timer < attackTime)
            {
                yield return new WaitForSeconds(attackTime - timer);
            }
            yield return new WaitForSeconds(attackCooldown);
            currectlyAttacking = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void StartAOEAttack()
    {
        StartCoroutine(StartAOEAttackCorutine());
    }

    IEnumerator StartAOEAttackCorutine()
    {
        currectlyAttacking = true;
        targets.Clear();
        yield return new WaitForSeconds(attackTime);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
            {

            }
            else if (affectsEnemies && targets[i].CompareTag("Enemy"))
            {
                targets[i].GetComponent<Enemy>().Hit(transform.position, source.CompareTag("Enemy") ? null : source.transform, knockback, attackPower);
            }
            else if (affectsSkeletons)
            {
                if (targets[i].CompareTag("Skeleton"))
                {
                    targets[i].GetComponent<Skeleton>().Hit(transform.position, source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                }
                else if (targets[i].CompareTag("Minion"))
                {
                    targets[i].GetComponent<Minion>().Hit(transform.position, source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                }
                else if (targets[i].CompareTag("Player Base"))
                {
                    targets[i].GetComponent<PlayerBase>().Hit(attackPower);
                }
            }
        }
        yield return new WaitForSeconds(attackCooldown);
        currectlyAttacking = false;
        gameObject.SetActive(false);
    }

    public void StartSingleAttack()
    {
        StartCoroutine(StartSingleAttackCorutine());
    }

    IEnumerator StartSingleAttackCorutine()
    {
        currectlyAttacking = true;
        //targets.Clear();
        yield return new WaitForSeconds(attackTime);
        if (source.CompareTag("Enemy"))
        {
            if (source.GetComponent<Enemy>().inPresenceOfTower)
            {
                GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().Hit(attackPower);
            }
            else
            {
                Transform target = source.GetComponent<Enemy>().goal;
                //Debug.Log(target.name + "\t\t\tMagnitude: " + (target.position - transform.position).magnitude + "\t\t\tRamge: " + (attackRange + (target.CompareTag("Skeleton") ? target.GetComponent<Skeleton>().circleCollider.radius : target.GetComponent<Minion>().circleCollider.radius) + .1f));
                if (target != null && (target.position - transform.position).magnitude < attackRange + (target.CompareTag("Skeleton") ? target.GetComponent<Skeleton>().circleCollider.radius : target.GetComponent<Minion>().circleCollider.radius) + .1f)
                {
                    if (target.CompareTag("Skeleton"))
                    {
                        target.GetComponent<Skeleton>().Hit(transform.position, source.transform, knockback, attackPower);
                    }
                    else if (target.CompareTag("Minion"))
                    {
                        target.GetComponent<Minion>().Hit(transform.position, source.transform, knockback, attackPower);
                    }
                }
            }
        }
        else if (source.CompareTag("Skeleton"))
        {
            Transform target = source.GetComponent<Skeleton>().goal;
            if (target != null && (target.position - transform.position).magnitude < attackRange + target.GetComponent<Enemy>().circleCollider.radius + .1f)
            {
                if (target.CompareTag("Enemy"))
                {
                    target.GetComponent<Enemy>().Hit(transform.position, source.transform, knockback, attackPower);
                }
            }
        }
        else /*if (source.CompareTag("Minion"))*/
        {
            Transform target = source.GetComponent<Minion>().goal;
            if (target != null && (target.position - transform.position).magnitude < attackRange + target.GetComponent<Enemy>().circleCollider.radius + .1f)
            {
                if (target.CompareTag("Enemy"))
                {
                    target.GetComponent<Enemy>().Hit(transform.position, source.transform, knockback, attackPower);
                }
            }
        }
        yield return new WaitForSeconds(attackCooldown);
        currectlyAttacking = false;
    }

    public void StartRangedAOEAttack()
    {
        StartCoroutine(StartRangedAOEAttackCorutine());
    }

    IEnumerator StartRangedAOEAttackCorutine()
    {
        Vector3 startPosition = transform.parent.localPosition;
        gameObject.SetActive(true);
        transform.parent.parent = null;
        currectlyAttacking = true;
        targets.Clear();
        if (source.CompareTag("Enemy"))
        {
            if (source.GetComponent<Enemy>().inPresenceOfTower)
            {
                transform.position = new Vector3((source.transform.position.y - posYBase) / slopeBase, source.transform.position.y, 0f);
            }
            else
            {
                Transform target = source.GetComponent<Enemy>().goal;
                if (target != null)
                {
                    transform.position = target.position;
                }
            }
        }
        else if (source.CompareTag("Skeleton"))
        {
            Transform target = source.GetComponent<Skeleton>().goal;
            if (target != null)
            {
                transform.position = target.position;
            }
        }
        else /*if (source.CompareTag("Minion"))*/
        {
            Transform target = source.GetComponent<Minion>().goal;
            if (target != null)
            {
                transform.position = target.position;
            }
        }

        yield return new WaitForSeconds(attackTime);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
            {

            }
            else if (affectsEnemies && targets[i].CompareTag("Enemy"))
            {
                targets[i].GetComponent<Enemy>().Hit(transform.position, source == null || source.CompareTag("Enemy") ? null : source.transform, knockback, attackPower);
            }
            else if (affectsSkeletons)
            {
                if (targets[i].CompareTag("Skeleton"))
                {
                    targets[i].GetComponent<Skeleton>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                }
                else if (targets[i].CompareTag("Minion"))
                {
                    targets[i].GetComponent<Minion>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                }
                else if (targets[i].CompareTag("Player Base"))
                {
                    targets[i].GetComponent<PlayerBase>().Hit(attackPower);
                }
            }
        }
        yield return new WaitForSeconds(attackCooldown);
        if (source != null)
        {
            transform.parent.parent = source.transform;
            transform.parent.transform.localPosition = startPosition;
            transform.localPosition = Vector3.zero;
            currectlyAttacking = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void StartDigAttack(short bonesNeeded, Grave target)
    {
        StartCoroutine(StartDigAttackCorutine(bonesNeeded, target));
    }

    IEnumerator StartDigAttackCorutine(short bonesNeeded, Grave target)
    {
        currectlyAttacking = true;
        source.GetComponent<Minion>().anim.SetBool("Digging", true);
        //targets.Clear();
        yield return new WaitForSeconds(attackTime);
        if (target != null)
        {
            source.GetComponent<Minion>().bonesStored += target.DigBones(bonesNeeded);
        }
        yield return new WaitForSeconds(attackCooldown);
        currectlyAttacking = false;
        source.GetComponent<Minion>().anim.SetBool("Digging", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (affectsSkeletons)
        {
            if ((collision.CompareTag("Skeleton") || collision.CompareTag("Minion") || collision.CompareTag("Player Base")) && collision.gameObject != source)
            {
                bool isNew = true;
                for (int i = 0; i < targets.Count; i++)
                {
                    if (collision.gameObject == targets[i])
                    {
                        isNew = false;
                        break;
                    }
                }
                if (isNew)
                {
                    targets.Add(collision.gameObject);
                }
            }
        }
        if (affectsEnemies)
        {
            if ((collision.CompareTag("Enemy") || collision.CompareTag("Enemy Base")) && collision.gameObject != source)
            {
                bool isNew = true;
                for (int i = 0; i < targets.Count; i++)
                {
                    if (collision.gameObject == targets[i])
                    {
                        isNew = false;
                        break;
                    }
                }
                if (isNew)
                {
                    targets.Add(collision.gameObject);
                }
            }
        }
    }

    public void StartArrowBarrageAttack()
    {
        gameObject.SetActive(true);
        StartCoroutine(StartArrowBarrageAttackCorutine());
    }

    IEnumerator StartArrowBarrageAttackCorutine()
    {
        Vector3 startPosition = transform.position;
        sort = false;
        spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(40f, 80f));
        float rotation = Random.Range(40f, 80f);
        attackSpeed = Random.Range(2.5f, 3.5f);
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        //spriteRenderer.enabled = true;
        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().SetTrigger("Attack");
        }
        currectlyAttacking = true;
        targets.Clear();
        bool done = false;
        while (!done && transform.position.y >= startPosition.y)
        {
            transform.Translate(Vector2.right * attackSpeed * Time.deltaTime);
            if (transform.rotation.eulerAngles.z > -80f)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - (rotation * Time.deltaTime));
            }
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {

                }
                else if (affectsEnemies && targets[i].CompareTag("Enemy") && targets[i].transform.position.y < startPosition.y+3f && transform.position.y > startPosition.y - 1.5f)
                {
                    targets[i].GetComponent<Enemy>().Hit(transform.position, null, knockback, attackPower);
                    done = true;
                    break;
                }
                /*else if (affectsSkeletons)
                {
                    if (targets[i].CompareTag("Skeleton"))
                    {
                        targets[i].GetComponent<Skeleton>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Minion"))
                    {
                        targets[i].GetComponent<Minion>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    /*else if (targets[i].CompareTag("Player Base"))
                    {
                        targets[i].GetComponent<PlayerBase>().Hit(attackPower);
                        done = true;
                        break;
                    }*/
                //}
            }
            yield return new WaitForFixedUpdate();
        }
        if (!done)
        {
            yield return new WaitForSeconds(.5f);
        }
        //spriteRenderer.enabled = false;
        transform.position = startPosition;
        currectlyAttacking = false;
        gameObject.SetActive(false);
    }

    IEnumerator StartSpecialGoblinArrowAttack()
    {
        source.GetComponent<Skeleton>().attack.currectlyAttacking = true;
        Vector3 startPosition = transform.parent.localPosition;
        gameObject.SetActive(true);
        spriteRenderer.enabled = true;
        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().SetTrigger("Attack");
        }
        transform.parent.parent = null;
        targets.Clear();
        float timer = 0f;
        bool done = false;
        while (timer < attackTime && !done)
        {
            timer += Time.deltaTime;
            transform.position += transform.right * -attackSpeed * Time.deltaTime;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {

                }
                else if (affectsEnemies && targets[i].CompareTag("Enemy"))
                {
                    targets[i].GetComponent<Enemy>().Hit(transform.position, source == null || source.CompareTag("Enemy") ? null : source.transform, knockback, attackPower);
                    done = true;
                    break;
                }
                else if (affectsSkeletons)
                {
                    if (targets[i].CompareTag("Skeleton"))
                    {
                        targets[i].GetComponent<Skeleton>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Minion"))
                    {
                        targets[i].GetComponent<Minion>().Hit(transform.position, source == null || source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Player Base"))
                    {
                        targets[i].GetComponent<PlayerBase>().Hit(attackPower);
                        done = true;
                        break;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        spriteRenderer.enabled = false;
        if (source != null)
        {
            transform.parent.parent = source.transform;
            transform.parent.transform.localPosition = startPosition;
            transform.localPosition = Vector3.zero;
            if (timer < attackTime)
            {
                yield return new WaitForSeconds(attackTime - timer);
            }
            yield return new WaitForSeconds(attackCooldown);
            source.GetComponent<Skeleton>().attack.currectlyAttacking = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    IEnumerator StartSpecialWitchGravityAttack()
    {
        Vector3 startPosition = transform.parent.localPosition;
        gameObject.SetActive(true);
        transform.parent.parent = null;
        source.GetComponent<Skeleton>().attack.currectlyAttacking = true;
        targets.Clear();
        Transform target = source.GetComponent<Skeleton>().goal;
        if (target != null)
        {
            transform.position = source.transform.position + ((target.position - source.transform.position).normalized * ((target.position - source.transform.position).magnitude + 2f));
        }
        float timer = 0f;
        while (timer < attackTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {

                }
                else
                {
                    Vector3 targetPosition = targets[i].transform.position;
                    Rigidbody2D targetRigidbody = targets[i].GetComponent<Rigidbody2D>();
                    Vector2 curentDirection = new Vector2(targetPosition.x  - transform.position.x, targetPosition.y - transform.position.y);
                    Vector2 futureDirection = curentDirection + targetRigidbody.velocity;//Position in 1s
                    if (futureDirection.y < (-curentDirection.x / curentDirection.y) * futureDirection.x || futureDirection.magnitude < .5f)
                    {
                        targetRigidbody.velocity -= curentDirection.normalized * 4.5f * targets[i].GetComponent<Enemy>().speedAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        targetRigidbody.velocity -= curentDirection.normalized * .5f * targets[i].GetComponent<Enemy>().speedAcceleration * Time.deltaTime;
                    }
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        if (source != null)
        {
            transform.parent.parent = source.transform;
            transform.parent.transform.localPosition = startPosition;
            transform.localPosition = Vector3.zero;
            source.GetComponent<Skeleton>().attack.currectlyAttacking = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    IEnumerator StartSpecialOrcAttack()
    {
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator StartSpecialOgreAttack()
    {
        yield return new WaitForSeconds(.5f);
    }
}
