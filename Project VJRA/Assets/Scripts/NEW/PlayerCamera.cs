using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    //MOUSE LOOK VARIABLES

    public float sensX;
    public float sensY;

    public Transform orientation;
    float xRotation;
    float yRotation;

    public float iCamFOV;

    //END OF VARIABLES

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        iCamFOV = GetComponent<Camera>().fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -=mouseY;
        xRotation = Mathf.Clamp(xRotation,-90f, 90f);

        //rotate camera and orientation;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFOV(float value)
    {
        GetComponent<Camera>().DOFieldOfView(value, 0.25f);
    }
}
