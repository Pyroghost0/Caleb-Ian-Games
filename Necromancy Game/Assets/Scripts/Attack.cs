using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    AOE = 0,
    Projectile = 1,
    Single = 2,
    Dig = 3,
    RangedAOE = 4
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
    private float posYBase = 6f;

    private void Update()
    {
        spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
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
        else /*if (attackType == AttackType.Dig)*/
        {
            //StartCoroutine(StartDigAttackCorutine());
        }
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
                    targets[i].GetComponent<Enemy>().Hit(transform.position, source.CompareTag("Enemy") ? null : source.transform, knockback, attackPower);
                    done = true;
                    break;
                }
                else if (affectsSkeletons)
                {
                    if (targets[i].CompareTag("Skeleton"))
                    {
                        targets[i].GetComponent<Skeleton>().Hit(transform.position, source.CompareTag("Skeleton") ? null : source.transform, knockback, attackPower);
                        done = true;
                        break;
                    }
                    else if (targets[i].CompareTag("Minion"))
                    {
                        targets[i].GetComponent<Minion>().Hit(transform.position, source.CompareTag("Minion") ? null : source.transform, knockback, attackPower);
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
            Destroy(transform.parent);
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
                    targets[i].GetComponent<Minion>().Hit(transform.position, source.CompareTag("Minion") ? null : source.transform, knockback, attackPower);
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
                if (target != null && (target.position - transform.position).magnitude < attackRange + .1f)
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
            if (target != null && (target.position - transform.position).magnitude < attackRange + .1f)
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
            if (target != null && (target.position - transform.position).magnitude < attackRange + .1f)
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
        currectlyAttacking = true;
        targets.Clear();
        if (source.CompareTag("Enemy"))
        {
            if (source.GetComponent<Enemy>().inPresenceOfTower)
            {
                GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().Hit(attackPower);
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
                    targets[i].GetComponent<Minion>().Hit(transform.position, source.CompareTag("Minion") ? null : source.transform, knockback, attackPower);
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
}
