using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    private Rigidbody rigidbody;
    private PlayerManager player;
    public GroundChecker groundChecker;
    public float gravityMultiplier = 3f;
    private Vector3 gravity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();
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
        yield return new WaitUntil(() => (groundChecker.inGround));
        while (true)
        {
            Vector3 jumpForce = Random.Range(3.5f, 4.5f) * ((new Vector3(player.player.transform.position.x - gameObject.transform.position.x, 0, player.player.transform.position.z - gameObject.transform.position.z).normalized) + (Random.Range(2.5f, 3.25f) * Vector3.up));
            rigidbody.AddForce(jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(.3f);
            yield return new WaitUntil(() => (groundChecker.inGround));
            rigidbody.velocity = Vector3.zero;
            //rigidbody.AddForce(-jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(Random.Range(.2f, .4f));
        }
    }
}
