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
    Vector3 move;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       HandleMove();
       HandleGravity();
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
        else if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
    }
}
