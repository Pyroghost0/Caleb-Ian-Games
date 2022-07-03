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
    private PlayerMovement playerMovement;

	private void Start()
	{
        anim = transform.parent.GetComponent<Animator>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
	}
	private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isBounceHelper)
            {
                bounceObject.canChange = false;
            }
            else
            {
                playerMovement.nearBounce = true;
                if (canChange)
                {
                    playerMovement.touchingBounce = true;
                }
                else
                {
                    playerMovement.touchingBounce = false;
                }
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
                playerMovement.nearBounce = false;
                playerMovement.touchingBounce = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerMovement.isGrounded)
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
