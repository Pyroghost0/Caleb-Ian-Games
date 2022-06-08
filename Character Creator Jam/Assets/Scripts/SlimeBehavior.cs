using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    private Rigidbody rigidbody;
    public GroundChecker groundChecker;
    public float gravityMultiplier = 3f;
    private Vector3 gravity;
    private PlayerManager playerManager;
    public GameObject player;
    private Gun gun;
    private GameObject suckSpot;
    private bool sucked = false;
    public float damage = 30f;
    public float knockback = 15f;
    public float health = 25f;
    public SlimeSpawner slimeSpawner;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        player = playerManager.player;
        gun = playerManager.gun;
        suckSpot = gun.suckSpot;
        gravity = Vector3.down * 9.8f * gravityMultiplier;
        StartCoroutine(ConstantJump());
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity += gravity * Time.deltaTime;
    }

    IEnumerator ConstantJump()
    {
        while (true)
        {
            yield return new WaitUntil(() => (groundChecker.inGround));
            Vector3 xzDirection = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, player.transform.position.z - gameObject.transform.position.z).normalized;
            Vector3 jumpForce = Random.Range(3.5f, 4.5f) * (xzDirection + (Random.Range(2.5f, 3.25f) * Vector3.up));
            rigidbody.AddForce(jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(.3f);
            yield return new WaitUntil(() => (groundChecker.inGround));
            rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce(-jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(Random.Range(.2f, .4f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= other.GetComponent<Bullet>().power * 25;
            Destroy(other.gameObject);
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !sucked)
        {
            player.GetComponent<PlayerStatus>().TakeDamage(damage, transform.position, knockback);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == suckSpot && gun.isSucking && !sucked)
        {
            //Debug.Log("Slime Destroyed");
            sucked = true;
            slimeSpawner.SlimeDeath();
            gun.SuckedSlime();
            Destroy(gameObject);
        }
    }
}
