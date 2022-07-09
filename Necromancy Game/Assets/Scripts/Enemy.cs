using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public short health = 100;
    public short maxHealth = 100;
    public short defence = 10;
    public float speedAcceleration = 1f;
    public float maxSpeed = 2f;
    public CircleCollider2D circleCollider;
    public Attack attack;

    private float xGoal = 0f;
    public Transform goal;
    public bool inPresenceOfSkeleton = false;
    private bool inPresenceOfTower = false;
    public float skeletonAttackRange;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody2D rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inPresenceOfSkeleton)
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
            Vector2 destination = goal.position - transform.position;
            if (destination.magnitude < skeletonAttackRange)
            {
                if (!attack.currectlyAttacking)
                {
                    attack.gameObject.SetActive(true);
                    attack.StartAttack();
                }
            }
            else if (rigidbody.velocity.magnitude < maxSpeed / 3f)
            {
                rigidbody.velocity += ((maxSpeed / 3f) - rigidbody.velocity.magnitude) * destination.normalized;
            }
            else
            {
                rigidbody.velocity += speedAcceleration * destination.normalized * Time.deltaTime;
            }
        }
        else if (inPresenceOfTower)
        {
            rigidbody.velocity = Vector2.zero;
        }
        else
        {
            if (transform.position.x <= xGoal)
            {
                inPresenceOfTower = true;
                goal = GameObject.FindGameObjectWithTag("Player Base").transform;
            }
            else if (-rigidbody.velocity.x < maxSpeed)
            {
                rigidbody.velocity += speedAcceleration * Time.deltaTime * Vector2.left;
                if (-rigidbody.velocity.x >= maxSpeed)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
            }
        }
    }
}
