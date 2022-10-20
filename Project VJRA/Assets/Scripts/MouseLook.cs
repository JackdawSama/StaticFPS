using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    float mouseX;
    float mouseY;

    public float mouseSensitivity = 50f;
    public Transform orientation;

    float xRotation = 0f;
    float yRotation = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
