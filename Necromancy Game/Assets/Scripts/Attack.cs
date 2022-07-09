using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public short attackPower = 50;
    public float attackSpeed = 1f;
    public float attackCooldown = .4f;
    public float attackRange = 1f;
    public bool affectsEnemies = false;
    public bool affectsSkeletons = false;

    public bool currectlyAttacking = false;
    private List<GameObject> targets = new List<GameObject>();

    public void StartAttack()
    {
        StartCoroutine(StartAttackCorutine());
    }

    IEnumerator StartAttackCorutine()
    {
        currectlyAttacking = true;
        targets.Clear();
        yield return new WaitForSeconds(attackSpeed);
        yield return new WaitForSeconds(attackCooldown);
        currectlyAttacking = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Skeleton") || collision.CompareTag("Minion"))
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

            }
        }
    }
}
