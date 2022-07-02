using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public float speed = 12f;
    private float gravity = -9.8f;
    public float gravityMultiplier = 2f;

    public GroundChecker groundChecker;
    public bool touchingBounce = false;
    public bool isGrounded;
    public float jumpHeight = 3f;
    private Vector3 move;

    public float mouseHorizontalSensitivity = 400f;
    public float mouseVirticalSensitivity = 100f;
    public float minAngle = -60f;
    public float maxAngle = 45f;
    private float verticalLookRotation = 10f;
    public GameObject cameraBasisObject;

    public Animator playerAnim;
    private bool jumped = false;
    //public bool canMove = true;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        gravity = -9.8f * gravityMultiplier;
        Cursor.lockState = CursorLockMode.Locked;
        //GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = groundChecker.inGround;
        playerAnim.SetBool("Grounded", isGrounded);

        float mouseX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseVirticalSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minAngle, maxAngle);//Cant over rotate
        playerAnim.SetFloat("Look", (verticalLookRotation * -0.0064f) + 0.44f);
        cameraBasisObject.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);//apply clamp

        if (groundChecker.onIce)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            playerAnim.SetFloat("MoveX", x);
            playerAnim.SetFloat("MoveY", z);

            float magnitude = Mathf.Sqrt(x * x + z * z);
            if (magnitude != 0)
            {
                x *= Mathf.Abs(x) / magnitude;
                z *= Mathf.Abs(z) / magnitude;
            }
            move += ((transform.right * x + transform.forward * z) - move) * Time.deltaTime / 2f;
            controller.enabled = true;
            controller.Move(move * speed * Time.deltaTime);
            controller.enabled = false;
        }
        else if (isGrounded)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            playerAnim.SetFloat("MoveX", x);
            playerAnim.SetFloat("MoveY", z);

            float magnitude = Mathf.Sqrt(x * x + z * z);
            if (magnitude != 0)
            {
                x *= Mathf.Abs(x) / magnitude;
                z *= Mathf.Abs(z) / magnitude;
            }
            move = transform.right * x + transform.forward * z;
            controller.enabled = true;
            controller.Move(move * speed * Time.deltaTime);
            controller.enabled = false;
        }
        else if (!touchingBounce)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            playerAnim.SetFloat("MoveX", x);
            playerAnim.SetFloat("MoveY", z);

            float magnitude = Mathf.Sqrt(x * x + z * z);
            if (magnitude != 0)
            {
                x *= Mathf.Abs(x) / magnitude;
                z *= Mathf.Abs(z) / magnitude;
            }
            move = transform.right * x + transform.forward * z;
            rigidbody.velocity -= new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z) * 3f * Time.deltaTime;
            rigidbody.velocity += move * speed * 3f * Time.deltaTime;
            /*
            controller.enabled = true;
            if (controller.Move(move * speed * Time.deltaTime).HasFlag(CollisionFlags.Sides)) {
                controller.Move(-move * speed * Time.deltaTime);
            }
            controller.enabled = false;*/
        }
        else
        {
            isGrounded = false;
        }


        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 velocity = new Vector3(0f, Mathf.Sqrt(jumpHeight * -2f * gravity), 0f);
                rigidbody.velocity = velocity + (move * speed);
                jumped = true;
                StartCoroutine(Jump());
            }
            else if (!jumped)
            {
                rigidbody.velocity = Vector3.down;
            }
        }
        else
        {
            rigidbody.velocity += Vector3.up * gravity * Time.deltaTime;
            StartCoroutine(JumpUpCheck());
        }
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(.4f);
        jumped = false;
    }

    IEnumerator JumpUpCheck()
    {
        Vector3 previousPosition = transform.position;
        yield return new WaitForFixedUpdate();
        Vector3 nextPosition = transform.position;
        transform.position = previousPosition;
        controller.enabled = true;
        controller.Move((rigidbody.velocity - new Vector3(0f, rigidbody.velocity.y, 0f)) * Time.deltaTime);
        controller.enabled = false;
        if (transform.position.y > previousPosition.y)
        {
            rigidbody.velocity = Vector3.down;

            Debug.Log("Previous Position: " + previousPosition + "\t\t\tRigidbody Position: " + nextPosition + "\t\t\tCortroller Position: " + transform.position);
        }
        else
        {
            //Debug.Log("F");
            transform.position = nextPosition;
        }
    }
}

