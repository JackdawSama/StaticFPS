using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    // [SerializeField] float maxSpeed;

    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;

    bool isJumping = false;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;


    public Transform orientation;

    float hInput;
    float vInput;

    Vector3 moveDir;

    Rigidbody rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMoveInputs();
        GrouundCheck();
        LimitSpeed();
        JumpInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMoveInputs()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        moveDir = transform.forward * vInput + transform.right * hInput;

        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    void GrouundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * 0.5f) + 0.3f, groundMask);

        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    void LimitSpeed()
    {
        Vector3 trueSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(trueSpeed.magnitude > moveSpeed)
        {
            Vector3 limitSpeed = trueSpeed.normalized * moveSpeed;
            rb.velocity = new Vector3(limitSpeed.x, rb.velocity.y, limitSpeed.z);
        } 
    }

    void JumpInput()
    {
        if(Input.GetButtonDown("Jump") && !isJumping && isGrounded)
        {
            isJumping = true;

            HandleJump();
        }
    }

    void HandleJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if(isGrounded)
        {
            isJumping = false;
        }
    }
}
