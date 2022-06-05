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
        if (!groundChecker.inGround)
        {
            rigidbody.velocity += gravity * Time.deltaTime;
        }
    }

    IEnumerator ConstantJump()
    {
        while (true)
        {
            if (!groundChecker.inGround)
            {
                yield return new WaitUntil(() => (groundChecker.inGround));
            }
            rigidbody.velocity = Vector3.zero;
            yield return new WaitForSeconds(Random.Range(.5f, 1f));
            rigidbody.AddForce(3f * (new Vector3(player.player.transform.position.x - gameObject.transform.position.x, 0, player.player.transform.position.z - gameObject.transform.position.z).normalized + (3 * Vector3.up)), ForceMode.Impulse);
        }
    }
}
