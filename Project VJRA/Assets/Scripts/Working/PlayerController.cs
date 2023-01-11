using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{


    //SECTION : KEYBINDS
    [Header("Controls")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    //SECTION END

    //SECTION : MOVEMENT VARIABLES
    [Header("Basic Movement")]
    public float moveSpeed;
    float desiredSpeed;
    float lastDesiredSpeed;
    bool desiredSpeedChanged;
    float speedChangeFactor;
    [SerializeField] float walkSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float groundDrag;
    bool keepMomentum;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float airMultiplier;
    [SerializeField] float customGravity;
    [SerializeField] float gravRef;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashSpeedChangeFactor;
    public bool isDashing = false;

    //SECTION END

    //SECTION : GROUND CHECK
    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    bool isJumping = false;
    //SECTION END

    //SECTION : PLAYER VARIABLES
    [SerializeField] Transform orientation;
    private float standHeight;
    [SerializeField] float crouchHeight;

    float playerMaxHP = 100;
    public float playerHP;

    float hInput;
    float vInput;
    Vector3 moveDir;

    Rigidbody rb;

    public enum MovementState
    {
        idle,
        walking,
        dashing,
        crouching,
        air
    }
    [SerializeField] public MovementState state;
    [SerializeField] MovementState lastState;
    //SECTION END

    [Header("Audio")]
    float footStepTimer;
    [SerializeField] float footstepCD = 0.35f;
    AudioSource audioSource;
    [SerializeField] AudioClip[] Footsteps;
    [SerializeField] AudioClip jump;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        moveSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        standHeight = transform.localScale.y;
        crouchHeight = transform.localScale.y/2;

        playerHP = playerMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMoveInputs();
        GroundCheck();
        LimitSpeed();
        StateHandler();

        footStepTimer += Time.deltaTime;

        if(footStepTimer >= footstepCD)
        {
            HandleFootstepsAudio();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    //PLAYER INPUT SECTION
    void HandleMoveInputs()                                                         //Handles Input from player for movement, crouching and jumping
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        JumpInput();
        HandleCrouch();
    }

    void JumpInput()                                                                //Jump Input handle function
    {
        if(Input.GetKeyDown(jumpKey) && !isJumping && isGrounded)
        {
            isJumping = true;

            HandleJump();

            audioSource.PlayOneShot(jump);
        }
    }
    void HandleCrouch()                                                             //Crouch Input handle function
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
    //END OF SECTION

    //PLAYER MOVEMENT SECTION
    void MovePlayer()                                                               //Player movement
    {
        moveDir = orientation.forward * vInput + orientation.right * hInput;

        if(isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            rb.AddForce(new Vector3(0, -1f, 0) * rb.mass * customGravity);
        }
    }

    void HandleJump()                                                               //Player jump
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if(isGrounded)
        {
            isJumping = false;
        }
    }

    //PLAYER MOVEMENT SECTION

    //PLAYER MOVEMENT CHECK SECTION
    void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * 0.5f) + 0.15f, groundMask);

        if(isDashing)return;                                                      //if player is dashing don't apply drag

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
    //END OF SECTION

    //PLAYER HEALTH SECTION
    public void HandlePlayerDamage(float damage)
    {
        playerHP = playerHP - damage;

        if(playerHP <= 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        gameObject.SetActive(false);
    }
    //END OF SECTION

    //AUDIO SECTION
    void HandleFootstepsAudio()
    {
        if(state == MovementState.walking)
        {
            audioSource.PlayOneShot(Footsteps[Random.Range(0,Footsteps.Length - 1)]);
        }
        footStepTimer = 0;
    }
    //END OF SECTION

    //STATE SECTION
    private void StateHandler()
    {
        if(isDashing)
        {
            state = MovementState.dashing;
            desiredSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;

            CacheDesiredData();
            return;
        }
        if(!isGrounded)
        {
            state = MovementState.air;

            if(desiredSpeed < dashSpeed)
            {
                desiredSpeed = walkSpeed;
            }

            CacheDesiredData();
            return;
        }
        if(isGrounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredSpeed = crouchSpeed;

            CacheDesiredData();
            return;
        }
        if(isGrounded && Vector3.Dot(rb.velocity,transform.forward) > 0.1f || Vector3.Dot(rb.velocity,transform.forward) < -0.1f)
        {
            state = MovementState.walking;
            desiredSpeed = walkSpeed;

            CacheDesiredData();
            return;
        }
        state = MovementState.idle;

        CacheDesiredData();
    }
    //END OF SECTION

    void CacheDesiredData()
    {
        desiredSpeedChanged = desiredSpeed != lastDesiredSpeed;
        if(lastState == MovementState.dashing) keepMomentum = true;

        if(desiredSpeedChanged)
        {
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredSpeed;
            }
        }

        lastDesiredSpeed = desiredSpeed;
        lastState = state;    
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredSpeed, time/difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
}
