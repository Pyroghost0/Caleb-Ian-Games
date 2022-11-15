/* Coded by Caleb Kahn
 * Qualms
 * Bouncy objects that act like a trampoline
 */
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
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    public AudioSource bounceSound;

	private void Start()
	{
        anim = transform.parent.GetComponent<Animator>();}
    private void FindPlayer()
	{
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        if (playerManager.player != null) playerMovement = playerManager.player.GetComponent<PlayerMovement>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (playerMovement == null) FindPlayer();
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
        if (playerMovement == null) FindPlayer();
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
        if (playerMovement == null) FindPlayer();
        if (collision.gameObject.CompareTag("Player") && !playerMovement.isGrounded)
        {
            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.velocity = Vector3.up * bounce;
            anim.SetTrigger("Bounce");
            bounceSound.Play();
        }
        if (collision.gameObject.CompareTag("Slime"))
        {
            Rigidbody slime = collision.gameObject.GetComponent<Rigidbody>();
            if (slime.velocity.y > -1f)
            {
                slime.velocity = Vector3.up * bounce / 2f;
                anim.SetTrigger("Bounce");
                bounceSound.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (playerMovement == null) FindPlayer();
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().touchingBounce = false;
        }
    }
}
