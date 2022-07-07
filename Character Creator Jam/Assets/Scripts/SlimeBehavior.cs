using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
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
    public Animator anim;
    public Animator animInner;

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

    private bool isDead = false;
    public int slimeColor = 0;

    public AudioSource sound;
    public AudioClip[] jump;
    public AudioClip hurt;
    public AudioClip dead;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        FindPlayer();
        gravity = Vector3.down * 9.8f * gravityMultiplier;
        maxDistenceFromPlayer = slimeSpawner.spawnDistence;
        averageJumpHeightStrengthMin = averageJumpHeightStrength * .75f;
        averageJumpHeightStrengthMax = averageJumpHeightStrength * 1.25f;
        averageJumpStrengthMin = averageJumpStrength * .9f;
        averageJumpStrengthMax = averageJumpStrength * 1.11f;
        StartCoroutine(ConstantJump());
    }
    private void FindPlayer()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        if (playerManager.player != null && playerManager.gun != null)
        {
            player = playerManager.player;
            playerStatus = player.GetComponent<PlayerStatus>();
            gun = playerManager.gun;
            suckSpot = gun.suckSpot;
        }
    }
    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity += gravity * Time.deltaTime;
    }

    IEnumerator ConstantJump()
    {
        while (!isDead)
        {
            if (player != null)
			{
                yield return new WaitUntil(() => (groundChecker.inGround));
                if ((player.transform.position - transform.position).magnitude < maxDistenceFromPlayer)
                {
                    Vector3 xzDirection = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, player.transform.position.z - gameObject.transform.position.z).normalized;
                    Vector3 jumpForce = Random.Range(averageJumpStrengthMin, averageJumpStrengthMax) * (xzDirection + (Random.Range(averageJumpHeightStrengthMin, averageJumpHeightStrengthMax) * Vector3.up));
                    rigidbody.AddForce(jumpForce * playerStatus.slimeJumpStrengthMultiplier, ForceMode.Impulse);
                    anim.SetBool("isGrounded", false);
                    animInner.SetBool("isGrounded", false);
                }
                else
                {
                    slimeSpawner.SlimeDeath();
                    Destroy(gameObject);
                }
                yield return new WaitForSeconds(.3f);
                yield return new WaitUntil(() => (groundChecker.inGround));
                if (!groundChecker.onIce)
                {
                    rigidbody.velocity = Vector3.zero;
                }
                else
                {
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
                }
                anim.SetBool("isGrounded", true);
                animInner.SetBool("isGrounded", true);

                sound.clip = jump[Random.Range(0, 2)];
                sound.Play();
				//rigidbody.AddForce(-jumpForce, ForceMode.Impulse);
			}
			else
			{
                FindPlayer();
			}

            yield return new WaitForSeconds(Random.Range(.2f, .4f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            anim.SetTrigger("Hurt");
            sound.clip = hurt;
            sound.Play();
            health -= other.GetComponent<Bullet>().power * 25;
            if (health <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                StartCoroutine(Knockback(other.transform.rotation * other.GetComponent<Bullet>().movement * other.GetComponent<Bullet>().power * 50f));
            }
            Destroy(other.gameObject);
        }
    }
    public void StartDie()
    {
        StartCoroutine(Die());
    }
    public IEnumerator Die()
    {
        slimeSpawner.SlimeDeath();
        isDead = true;
        anim.SetBool("isDead", true);
        animInner.SetBool("isDead", true);
        sound.clip = dead;
        sound.Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    IEnumerator Knockback(Vector3 movement)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            rigidbody.AddForce(((1f - timer) / 1f) * movement * Time.deltaTime, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !sucked && !isDead)
        {
            other.GetComponent<PlayerStatus>().TakeDamage(damage, transform.position, knockback);
        }
        if (other.gameObject == suckSpot && gun.isSucking && !sucked)
        {
            if (gun == null) FindPlayer();
            //Debug.Log("Slime Destroyed");
            sucked = true;
            sound.clip = dead;
            sound.Play();
            slimeSpawner.SlimeDeath();
            gun.SuckedSlime(slimeColor);
            Destroy(gameObject);
        }
    }
}
