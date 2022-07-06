using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerBehavior : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public GroundChecker groundChecker;
    private Vector3 gravity;
    private PlayerManager playerManager;
    public GameObject player;
    private PlayerStatus playerStatus;
    private CharacterController characterController;
    private Animator anim;

    public List<GameObject> faces;
    public float maxDistenceFromPlayer = 80f;
    public float seesPlayerDistence = 70f;
    private bool seesPlayer = false;

    public float gravityMultiplier = 1f;
    public float damage = 50f;
    public float knockback = 35f;
    public float selfKnockback = .3f;
    public float health = 40f;
    public float movementSpeed = 5f;
    public float turnSpeed = 1.5f;

    private bool isDead = false;
    public Transform truePosition;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public AudioSource[] audio;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public AudioClip[] hurt;

    // Start is called before the first frame update
    void Start()
    {
        faces[0].SetActive(false);
        rigidbody = gameObject.GetComponent<Rigidbody>();
        characterController = gameObject.GetComponent<CharacterController>();
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        player = playerManager.player;
        playerStatus = player.GetComponent<PlayerStatus>();
        gravity = Vector3.down * 9.8f * gravityMultiplier;
        faces.RemoveAt(player.GetComponent<PlayerStatus>().isMale ? player.GetComponent<PlayerStatus>().headNumber + 3 : player.GetComponent<PlayerStatus>().headNumber);
        faces[Random.Range(0, faces.Count)].SetActive(true);
        anim = transform.GetComponentInChildren<Animator>();
        //maxDistenceFromPlayer = slimeSpawner.spawnDistence;
        //StartCoroutine(WalkTowardPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        //rigidbody.velocity += gravity * Time.deltaTime;
        if (groundChecker.inGround & !isDead)
        {
            rigidbody.velocity = Vector3.down;
            if (seesPlayer)
            {
                if ((transform.position - player.transform.position).magnitude < maxDistenceFromPlayer)
                {
                    Vector3 direction = player.transform.position - transform.position;
                    direction.y = 0;
                    direction = direction.normalized;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);

                    characterController.enabled = true;
                    characterController.Move(transform.rotation * Vector3.forward * movementSpeed * Time.deltaTime);
                    characterController.enabled = false;
                }
                else
                {
                    seesPlayer = false;
                    anim.SetBool("isRunning", false);
                }
            }
            else if ((transform.position - player.transform.position).magnitude < seesPlayerDistence)
            {
                seesPlayer = true;
                anim.SetBool("isRunning", true);
            }

        }
    }

    /*IEnumerator WalkTowardPlayer()
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
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce(-jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(Random.Range(.2f, .4f));
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            anim.SetTrigger("Hurt");
            health -= other.GetComponent<Bullet>().power * 25;
            if (health <= 0)
            {
                for (int i = 0; i < audio.Length; i++)
				{
                    audio[i].clip = hurt[2];
                    audio[i].Play();
                }
                //slimeSpawner.SlimeDeath();
                StartCoroutine(Die());
            }
            else
            {
                for (int i = 0; i < audio.Length; i++)
                {
                    audio[i].clip = hurt[Random.Range(0, 2)];
                    audio[i].Play();
                }
                StartCoroutine(Knockback(other.transform.rotation * other.GetComponent<Bullet>().movement * other.GetComponent<Bullet>().power * selfKnockback));
            }
            Destroy(other.gameObject);
        }
    }
    public void StartDie()
	{
        StartCoroutine(Die());
	}
    IEnumerator Die()
    {

        isDead = true;
        anim.SetBool("isDead", true);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    IEnumerator Knockback(Vector3 movement)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            characterController.enabled = true;
            characterController.Move(((1f - timer) / 1f) * movement * Time.deltaTime);
            characterController.enabled = false;
            //rigidbody.AddForce(((1f - timer) / 1f) * movement * Time.deltaTime, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isDead)
        {
            anim.SetTrigger("Attack");
            player.GetComponent<PlayerStatus>().TakeDamage(damage, transform.position, knockback);
        }
    }
}
