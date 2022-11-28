using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    //SECTION : KEYBINDS
    [Header("Controls")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    //SECTION END

    //SECTION : MOVEMENT VARIABLES
    [Header("Movement")]
    public float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float airMultiplier;
    //SECTION END

    //SECTION : GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    bool isJumping = false;
    //SECTION END

    //SECTION : PLAYER VARIABLES
    private float standHeight;
    private float crouchHeight;

    float hInput;
    float vInput;
    Vector3 moveDir;

    Rigidbody rb;

    //public float enemyDistance = 5f;
    //public LayerMask enemyMask;
    //public bool enemyIsNearby;

    public enum MovementState
    {
        idle,
        walking,
        dashing,
        crouching,
        air
    }
    [SerializeField] public MovementState state;
    //SECTION END

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        standHeight = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMoveInputs();
        GroundCheck();
        LimitSpeed();
        StateHandler();
        //HandleEnemyCheck();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMoveInputs()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        JumpInput();
        HandleCrouch();


    }


    //handles the states of the player
    private void StateHandler()
    {
        if(isGrounded && Input.GetKeyDown(sprintKey) && rb.velocity.magnitude > 0)
        {
            state = MovementState.dashing;
            return;
        }
        if(isGrounded && Vector3.Dot(rb.velocity,transform.forward) > 0.1f || Vector3.Dot(rb.velocity,transform.forward) < -0.1f)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            return;
        }
        if(isGrounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
            return;
        }
        if(!isGrounded)
        {
            state = MovementState.air;
            return;
        }
        state = MovementState.idle;
    }


    //moves the player and switches their states absed on their speed
    void MovePlayer()
    {
        moveDir = transform.forward * vInput + transform.right * hInput;

        if(isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);

            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                rb.AddForce(moveDir.normalized * dashSpeed * 10f, ForceMode.Impulse);
            }
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void GroundCheck()
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
        if(Input.GetKeyDown(jumpKey) && !isJumping && isGrounded)
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

    void HandleCrouch()
    {
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,crouchHeight,transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,standHeight,transform.localScale.z);
        }
    }

    // void HandleEnemyCheck()
    // {
    //     enemyIsNearby = Physics.CheckSphere(transform.position, enemyDistance, enemyMask);
    // }
}
