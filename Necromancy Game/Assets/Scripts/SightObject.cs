using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightObject : MonoBehaviour
{
    public Enemy enemy;
    public Skeleton skeleton;
    public Minion minion;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (enemy != null)
        {
            if (((collision.CompareTag("Skeleton") && !collision.GetComponent<Skeleton>().dead) || collision.CompareTag("Minion")) && !enemy.inPresenceOfSkeleton)
            {
                enemy.goal = collision.transform;
                enemy.inPresenceOfSkeleton = true;
                enemy.skeletonAttackRange = enemy.attack.attackRange + enemy.circleCollider.radius + (collision.CompareTag("Skeleton") ? collision.GetComponent<Skeleton>().circleCollider.radius : collision.GetComponent<Minion>().circleCollider.radius);
            }
        }
        else if (skeleton != null)
        {
            if ((collision.CompareTag("Enemy") && !collision.GetComponent<Enemy>().dead) && !skeleton.inPresenceOfEnemy)
            {
                skeleton.goal = collision.transform;
                skeleton.inPresenceOfEnemy = true;
                skeleton.enemyAttackRange = skeleton.attack.attackRange + skeleton.circleCollider.radius + collision.GetComponent<Enemy>().circleCollider.radius;
            }
        }
        else /*if (minion != null)*/
        {
            if ((collision.CompareTag("Enemy") && !collision.GetComponent<Enemy>().dead) && !minion.inDiggingMode && !minion.inPresenceOfEnemy)
            {
                minion.goal = collision.transform;
                minion.inPresenceOfEnemy = true;
                minion.enemyAttackRange = minion.attack.attackRange + minion.circleCollider.radius + collision.GetComponent<Enemy>().circleCollider.radius;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enemy != null)
        {
            if (enemy.goal != null && collision.transform == enemy.goal)
            {
                enemy.goal = null;
                enemy.inPresenceOfSkeleton = false;
            }
        }
        else if (skeleton != null)
        {
            if (skeleton.goal != null && collision.transform == skeleton.goal)
            {
                skeleton.goal = null;
                skeleton.inPresenceOfEnemy = false;
            }
        }
        else /*if (minion != null)*/
        {
            if (minion.goal != null && collision.transform == minion.goal)
            {
                minion.goal = null;
                minion.inPresenceOfEnemy = false;
            }
        }
    }
}
