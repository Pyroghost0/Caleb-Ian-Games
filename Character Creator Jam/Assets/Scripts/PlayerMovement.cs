using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public float speed = 12f;
    public Vector3 velocity;
    private float gravity = -9.8f;
    public float gravityMultiplier = 2f;

    public GroundChecker groundChecker;
    public bool isGrounded;
    public float jumpHeight = 3f;

    public float mouseHorizontalSensitivity = 400f;
    public float mouseVirticalSensitivity = 100f;
    public float minAngle = -10f;
    public float maxAngle = 30f;
    private float verticalLookRotation = 10f;
    public GameObject cameraBasisObject;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player = gameObject;
    }

    void Awake()
    {
        gravity = -9.8f * gravityMultiplier;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseHorizontalSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseVirticalSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minAngle, maxAngle);//Cant over rotate
        cameraBasisObject.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);//apply clamp

        isGrounded = groundChecker.inGround;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (velocity.y < 0) {
                velocity.y = -1;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(((move * speed) + velocity) * Time.deltaTime);
    }
}

