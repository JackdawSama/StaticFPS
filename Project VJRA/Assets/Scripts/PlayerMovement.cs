using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    float moveX;
    float moveZ;
    float speed = 5f;
    Vector3 move;
    // Start is called before the first frame update
    void Start()
    {
        //controller = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);
    }
}
