using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    public float bounce = 30f;
    public BouncyObject bounceObject;
    public bool isBounceHelper = false;
    public bool canChange = true;
    private Animator anim;

	private void Start()
	{
        anim = transform.parent.GetComponent<Animator>();
	}
	private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isBounceHelper)
            {
                bounceObject.canChange = false;
            }
            else if (canChange)
            {
                other.GetComponent<PlayerMovement>().touchingBounce = true;
            }
            else
            {
                other.GetComponent<PlayerMovement>().touchingBounce = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isBounceHelper)
            {
                bounceObject.canChange = true;
            }
            else
            {
                other.GetComponent<PlayerMovement>().touchingBounce = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.velocity = Vector3.up * bounce;
            anim.SetTrigger("Bounce");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().touchingBounce = false;
        }
    }
}
