using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightObject : MonoBehaviour
{
    public Enemy enemy;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Skeleton") || collision.CompareTag("Minion")) && !enemy.inPresenceOfSkeleton)
        {
            enemy.goal = collision.transform;
            enemy.inPresenceOfSkeleton = true;
            enemy.skeletonAttackRange = enemy.attack.attackRange + enemy.circleCollider.radius + .5f;//Change Later
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enemy.goal != null && collision.transform == enemy.goal)
        {
            enemy.goal = null;
            enemy.inPresenceOfSkeleton = false;
        }
    }
}
