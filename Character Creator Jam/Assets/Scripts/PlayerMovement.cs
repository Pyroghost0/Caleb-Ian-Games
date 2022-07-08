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
    public Transform center;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public Transform camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public float autoAimPower = 1000f;

    public Animator playerAnim;
    private bool jumped = false;
    private bool leftGround = false;
    public bool nearBounce = false;

    public AudioSource playerAudio;
    public AudioSource walk;
    public AudioClip[] walks;
    public AudioClip[] maleJump;
    public AudioEchoFilter echo;
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

        Ray centerCameraRay = new Ray(center.transform.position, center.transform.forward);
        RaycastHit centerCameraHit;
        Physics.Raycast(centerCameraRay, out centerCameraHit);
        //Debug.Log("Camera: " + (centerCameraHit.collider == null ? "Nothing" : centerCameraHit.collider.name));
        //Debug.DrawRay(center.transform.position, center.transform.forward.normalized*20f, Color.red);
        float vertRotation = 0f;
        float horiRotation = 0f;
        if (centerCameraHit.collider != null)
        {
            if (centerCameraHit.collider.CompareTag("Slime"))
            {
                Vector3 rotation = Quaternion.LookRotation(center.position - centerCameraHit.collider.transform.position).eulerAngles - new Vector3(-verticalLookRotation+5f, transform.rotation.eulerAngles.y-6f, 0f);
                vertRotation = (rotation.x < 180f ? rotation.x : rotation.x -360f) /90f;
                horiRotation = (rotation.y > 0? rotation.y - 180f : rotation.y + 180f)/360f;
                //Debug.Log("Object Position: " + centerCameraHit.collider.transform.position + "\t\t\tCamera Position: " + center.position + "\nRotation: " +
                //Quaternion.LookRotation(center.position - centerCameraHit.collider.transform.position).eulerAngles + "\t\t\tSelf Angle: " + new Vector3(-verticalLookRotation+5f, transform.rotation.eulerAngles.y-6f, 0f));
                //Debug.Log("Rotation: " + rotation + "\t\t\tVerticle: " + vertRotation + "\t\t\tHorizontal: " + horiRotation);
            }
            else if (centerCameraHit.collider.CompareTag("Flyer"))
            {
                Vector3 rotation = Quaternion.LookRotation(center.position - centerCameraHit.collider.GetComponent<FlyerBehavior>().truePosition.position).eulerAngles - new Vector3(-verticalLookRotation + 5f, transform.rotation.eulerAngles.y - 6f, 0f);
                vertRotation = (rotation.x < 180f ? rotation.x : rotation.x - 360f) / 90f;
                horiRotation = (rotation.y > 0 ? rotation.y - 180f : rotation.y + 180f) / 90;
            }
            else if (centerCameraHit.collider.CompareTag("Walker"))
            {
                Vector3 rotation = Quaternion.LookRotation(center.position - centerCameraHit.collider.GetComponent<WalkerBehavior>().truePosition.position).eulerAngles - new Vector3(-verticalLookRotation + 5f, transform.rotation.eulerAngles.y - 6f, 0f);
                vertRotation = (rotation.x < 180f ? rotation.x : rotation.x - 360f) / 360f;
                horiRotation = (rotation.y > 0 ? rotation.y - 180f : rotation.y + 180f) / 360f;
            }
            else if (centerCameraHit.collider.CompareTag("Boss"))
            {
                Vector3 rotation = Quaternion.LookRotation(center.position - centerCameraHit.collider.GetComponent<BossBehavior>().truePosition.position).eulerAngles - new Vector3(-verticalLookRotation + 5f, transform.rotation.eulerAngles.y - 6f, 0f);
                vertRotation = (rotation.x < 180f ? rotation.x : rotation.x - 360f) / 360f;
                horiRotation = (rotation.y > 0 ? rotation.y - 180f : rotation.y + 180f) / 90f;
            }
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseVirticalSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * (mouseX + (horiRotation * autoAimPower * Time.deltaTime)));
        verticalLookRotation -= mouseY + (vertRotation * autoAimPower * Time.deltaTime);
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minAngle, maxAngle);//Cant over rotate
        playerAnim.SetFloat("Look", (verticalLookRotation * -0.0064f) + 0.44f);
        cameraBasisObject.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);//apply clamp

        if (groundChecker.onIce)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            playerAnim.SetFloat("MoveX", x);
            playerAnim.SetFloat("MoveY", z);
            if (z > 0)
			{
                walk.clip = walks[0];
			}
            else if (z < 0)
            {
                walk.clip = walks[1];
            }
            else
            {
                walk.clip = walks[2];
            }
            float magnitude = Mathf.Sqrt(x * x + z * z);
            if (magnitude != 0)
            {
                x *= Mathf.Abs(x) / magnitude;
                z *= Mathf.Abs(z) / magnitude;
                if (!walk.isPlaying) walk.Play();
            }
            else if (walk.isPlaying)
            {
                walk.Stop();
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
            if (z > 0)
            {
                walk.clip = walks[0];
            }
            else if (z < 0)
            {
                walk.clip = walks[1];
            }
            else
            {
                walk.clip = walks[2];
            }
            float magnitude = Mathf.Sqrt(x * x + z * z);
            if (magnitude != 0)
            {
                x *= Mathf.Abs(x) / magnitude;
                z *= Mathf.Abs(z) / magnitude;
                if (!walk.isPlaying) walk.Play();
            }else if (walk.isPlaying)
			{
                walk.Stop();
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
        }
        else
        {
            walk.Stop();
            isGrounded = false;
        }


        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {

                walk.Stop();
                rigidbody.velocity = new Vector3(0f, Mathf.Sqrt(jumpHeight * -2f * gravity), 0f);

                echo.enabled = false;
                playerAudio.clip = maleJump[Random.Range(0, maleJump.Length)];
                playerAudio.Play();
                StartCoroutine(Jump());
            }
            else if (!jumped)
            {
                rigidbody.velocity = Vector3.down;
            }
        }
        else if (!leftGround)
        {

            walk.Stop();
            rigidbody.velocity += move * speed + (Vector3.up * gravity * Time.deltaTime);
            StartCoroutine(WaitUntilGrounded());
        }
        else
        {

            walk.Stop();
            rigidbody.velocity += Vector3.up * gravity * Time.deltaTime;
            StartCoroutine(JumpUpCheck());
        }
    }

    IEnumerator WaitUntilGrounded()
    {
        leftGround = true;
        yield return new WaitUntil(() => (isGrounded));
        leftGround = false;
    }

    IEnumerator Jump()
    {
        jumped = true;
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
        if (transform.position.y > previousPosition.y && !nearBounce)
        {
            rigidbody.velocity = Vector3.down;
            //Debug.Log("Previous Position: " + previousPosition + "\t\t\tRigidbody Position: " + nextPosition + "\t\t\tCortroller Position: " + transform.position);
        }
        else
        {
            //Debug.Log("Previous Position: " + previousPosition + "\t\t\tRigidbody Position: " + nextPosition + "\t\t\tCortroller Position: " + transform.position);
            transform.position = nextPosition;
        }
    }
}

