using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public short attackPower = 50;
    public float attackSpeed = 1f;
    public float attackCooldown = .4f;
    public float attackRange = 1f;
    public float knockback = 1f;
    public bool affectsEnemies = false;
    public bool affectsSkeletons = false;

    public GameObject source;
    public bool currectlyAttacking = false;
    private List<GameObject> targets = new List<GameObject>();

    public void StartAOEAttack()
    {
        StartCoroutine(StartAOEAttackCorutine());
    }

    IEnumerator StartAOEAttackCorutine()
    {
        currectlyAttacking = true;
        targets.Clear();
        yield return new WaitForSeconds(attackSpeed);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
            {

            }
            else if (targets[i].CompareTag("Enemy"))
            {
                if (source.CompareTag("Enemy"))
                {
                    targets[i].GetComponent<Enemy>().Hit(transform.position, null, knockback, attackPower);
                }
                else
                {
                    targets[i].GetComponent<Enemy>().Hit(transform.position, source.transform, knockback, attackPower);
                }
            }
            else if (targets[i].CompareTag("Skeleton"))
            {
                if (source.CompareTag("Skeleton"))
                {
                    targets[i].GetComponent<Skeleton>().Hit(transform.position, null, knockback, attackPower);
                }
                else
                {
                    targets[i].GetComponent<Skeleton>().Hit(transform.position, source.transform, knockback, attackPower);
                }
            }
            else if (targets[i].CompareTag("Minion"))
            {
                if (source.CompareTag("Minion"))
                {
                    targets[i].GetComponent<Minion>().Hit(transform.position, null, knockback, attackPower);
                }
                else
                {
                    targets[i].GetComponent<Minion>().Hit(transform.position, source.transform, knockback, attackPower);
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
        //targets.Clear();
        yield return new WaitForSeconds(attackSpeed);
        if (target != null)
        {
            source.GetComponent<Minion>().bonesStored += target.DigBones(bonesNeeded);
        }
        yield return new WaitForSeconds(attackCooldown);
        currectlyAttacking = false;
        gameObject.SetActive(false);
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
