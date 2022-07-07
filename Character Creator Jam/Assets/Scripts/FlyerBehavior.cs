using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerBehavior : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    private PlayerManager playerManager;
    public GameObject player;
    private PlayerStatus playerStatus;
    public Transform truePosition;
    private Vector3 forwardDirection;
    private Animator anim;

    public List<GameObject> faces;
    public float maxDistenceFromPlayer = 80f;
    public float seesPlayerDistence = 70f;
    private bool seesPlayer = false;

    public float damage = 25f;
    public float knockback = 15f;
    public float selfKnockback = .5f;
    public float health = 20f;
    public float movementSpeed = 5f;
    public float turnSpeed = 2.5f;

    private bool isDead = false;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public AudioSource audio;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public AudioClip[] hurt;

    // Start is called before the first frame update
    void Start()
    {
        faces[0].SetActive(false);
        rigidbody = gameObject.GetComponent<Rigidbody>();
        FindPlayer();
        forwardDirection = new Vector3(Random.Range(-.5f, .5f), Random.Range(.1f, .3f), 1f).normalized;
        anim = transform.GetComponentInChildren<Animator>();
        //maxDistenceFromPlayer = slimeSpawner.spawnDistence;
        //StartCoroutine(WalkTowardPlayer());
    }
    private void FindPlayer()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
        if (playerManager.player != null)
        {
            player = playerManager.player;
            playerStatus = player.GetComponent<PlayerStatus>();
            faces.RemoveAt(player.GetComponent<PlayerStatus>().isMale ? player.GetComponent<PlayerStatus>().headNumber + 3 : player.GetComponent<PlayerStatus>().headNumber);
            faces[Random.Range(0, faces.Count)].SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
		{
            if (player != null)
			{
                if (seesPlayer)
                {
                    if ((truePosition.position - playerStatus.truePosition.position).magnitude < maxDistenceFromPlayer)
                    {
                        Vector3 direction = playerStatus.truePosition.position - truePosition.position;
                        direction = direction.normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);

                        transform.Translate(forwardDirection * movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        seesPlayer = false;
                        anim.SetBool("isRunning", false);
                    }
                }
                else if ((truePosition.position - playerStatus.truePosition.position).magnitude < seesPlayerDistence)
                {
                    seesPlayer = true;
                    anim.SetBool("isRunning", true);
                }
                Vector3 normal = rigidbody.velocity.normalized;
                rigidbody.velocity -= normal * Time.deltaTime * 3f;
                if (rigidbody.velocity.normalized != normal)
                {
                    rigidbody.velocity = Vector3.zero;
                }
			}
			else
			{
                FindPlayer();
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
                audio.clip = hurt[2];
                audio.Play();
                //slimeSpawner.SlimeDeath();
                StartCoroutine(Die());
            }
            else
            {
                audio.clip = hurt[Random.Range(0, 2)];
                audio.Play();
                StartCoroutine(Knockback(other.transform.rotation * other.GetComponent<Bullet>().movement * other.GetComponent<Bullet>().power * selfKnockback));
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
            transform.position += ((1f - timer) / 1f) * movement * Time.deltaTime;
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
            other.GetComponent<PlayerStatus>().TakeDamage(damage, truePosition.position, knockback);
        }
    }
}
