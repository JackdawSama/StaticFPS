using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    float moveX;
    float moveZ;
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float gravity = 9.8f;
    [SerializeField]
    float jumpHeight = 5;
    [SerializeField]
    float dashTime = 0.25f;
    [SerializeField]
    float dashSpeed = 20f;
    Vector3 move;
    Vector3 velocity;

    public float enemyDistance = 5f;
    public LayerMask enemyMask;

    public bool enemyIsNearby;
    public bool isDashing;

    WeaponManager weaponManager;
    // Start is called before the first frame update
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyIsNearby = Physics.CheckSphere(transform.position, enemyDistance, enemyMask);

        HandleMove();
        HandleDash();
        HandleGravity();

        Debug.Log(controller.velocity.magnitude);
    }

    void HandleMove()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    void HandleGravity()
    {
        if(!controller.isGrounded)
        {   
            velocity.y -= gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else if(controller.isGrounded && velocity.y <= 0)
        {
            velocity.y = 0;
        }
    }

    void HandleDash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        //Debug.Log("Is Dashing");
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(move * dashSpeed * Time.deltaTime);

            yield return null;
        }

        isDashing = false;
    }
}
