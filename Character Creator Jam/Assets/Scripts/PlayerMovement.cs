using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private CharacterController controller;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Rigidbody rigidbody;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public float speed = 12f;
    private Vector3 velocity;
    private float gravity = -9.8f;
    public float gravityMultiplier = 2f;

    public GroundChecker groundChecker;
    public bool touchingBounce = false;
    public bool isGrounded;
    public float jumpHeight = 3f;
    private Vector3 move;

    public float mouseHorizontalSensitivity = 400f;
    public float mouseVirticalSensitivity = 100f;
    public float minAngle = -10f;
    public float maxAngle = 30f;
    private float verticalLookRotation = 10f;
    public GameObject cameraBasisObject;

    private Animator playerAnim;

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
        float mouseX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseVirticalSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minAngle, maxAngle);//Cant over rotate
        playerAnim.SetFloat("Look", Mathf.Pow((verticalLookRotation / -100) + 0.4f, 1.5f));
        cameraBasisObject.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);//apply clamp
        isGrounded = groundChecker.inGround;
        playerAnim.SetBool("Grounded", isGrounded);

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
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                rigidbody.velocity = velocity + (move * speed);
            }
            else if (velocity.y < 0) {
                velocity = Vector3.down;
                rigidbody.velocity = velocity;
            }
        }
        else
        {
            //float prevHeight = transform.position.y;
            /*controller.Move(velocity * Time.deltaTime);
            if (velocity.y > 0 && transform.position.y == prevHeight)
            {
                velocity.y *= -.3f;
            }*/
            rigidbody.velocity += Vector3.up * gravity * Time.deltaTime;
            velocity = rigidbody.velocity;
        }
        
    }
}

