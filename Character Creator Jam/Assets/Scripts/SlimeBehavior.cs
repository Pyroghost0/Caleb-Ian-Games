using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    private Rigidbody rigidbody;
    public GroundChecker groundChecker;
    private Vector3 gravity;
    private PlayerManager playerManager;
    public GameObject player;
    private PlayerStatus playerStatus;
    private Gun gun;
    private GameObject suckSpot;
    private bool sucked = false;
    public SlimeSpawner slimeSpawner;
    private float maxDistenceFromPlayer = 70f+10f;

    public float gravityMultiplier = 3f;
    public float damage = 30f;
    public float knockback = 25f;
    public float health = 25f;
    public float averageJumpHeightStrength = 3.5f;
    public float averageJumpStrength = 5f;
    private float averageJumpHeightStrengthMin = 4f;
    private float averageJumpHeightStrengthMax = 4f;
    private float averageJumpStrengthMin = 5f;
    private float averageJumpStrengthMax = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        player = playerManager.player;
        playerStatus = player.GetComponent<PlayerStatus>();
        gun = playerManager.gun;
        suckSpot = gun.suckSpot;
        gravity = Vector3.down * 9.8f * gravityMultiplier;
        maxDistenceFromPlayer = slimeSpawner.spawnDistence;
        averageJumpHeightStrengthMin = averageJumpHeightStrength * .75f;
        averageJumpHeightStrengthMax = averageJumpHeightStrength * 1.25f;
        averageJumpStrengthMin = averageJumpStrength * .9f;
        averageJumpStrengthMax = averageJumpStrength * 1.11f;
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
            if ((player.transform.position - transform.position).magnitude < maxDistenceFromPlayer)
            {
                Vector3 xzDirection = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, player.transform.position.z - gameObject.transform.position.z).normalized;
                Vector3 jumpForce = Random.Range(averageJumpStrengthMin, averageJumpStrengthMax) * (xzDirection + (Random.Range(averageJumpHeightStrengthMin, averageJumpHeightStrengthMax) * Vector3.up));
                rigidbody.AddForce(jumpForce * playerStatus.slimeJumpStrengthMultiplier, ForceMode.Impulse);
            }
            else
            {
                slimeSpawner.SlimeDeath();
                Destroy(gameObject);
            }
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
