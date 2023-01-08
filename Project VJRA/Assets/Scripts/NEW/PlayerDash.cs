using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] KeyCode dashKey = KeyCode.LeftShift;
    Rigidbody rb;
    PlayerController player;
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerCam;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashUpForce;
    [SerializeField] float dashDuration;

    [SerializeField] float dashCD;
    [SerializeField] float dashTimer = 0;

    Vector3 delayedForce;
    [SerializeField] AudioClip dash;
    AudioSource audioSource;

    [Header("Golden Gun")]
    bool GGisActive;
    bool perfectDodge;
    [SerializeField] float perfectDodgeRadius;
    [SerializeField] LayerMask dodgeLayer;



    [Header("CameraEffects")]
    [SerializeField] PlayerCamera cam;
    [SerializeField] float dashFOV;


    [Header("Settings")]
    [SerializeField] bool useCameraForward = false;
    [SerializeField] bool allowAllDirections = true;
    //[SerializeField] bool disableGravity = false;
    [SerializeField] bool resetVel = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        dashTimer += Time.deltaTime;

        DashInput();
    }

    void DashInput()
    {

        if(Input.GetKeyDown(dashKey) && !player.isDashing)
        {
            player.isDashing = true;
            
            HandleDash();
        }
    }
    void HandleDash()
    {
        cam.DoFOV(dashFOV);

        Transform forwardT;
        if(useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);
        
        Vector3 forceToApply = direction * dashForce * 10f + orientation.up * dashUpForce;

        delayedForce = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        audioSource.PlayOneShot(dash);

        Invoke(nameof(DashReset), dashDuration);
    }

    void DashReset()
    {
        player.isDashing = false;

        cam.DoFOV(cam.iCamFOV);

        Debug.Log(player.isDashing);
    }

    void DelayedDashForce()
    {
        if(resetVel)
        {
            rb.velocity = Vector3.zero;
        }

        rb.AddForce(delayedForce, ForceMode.Impulse);
    }

    Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if(allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }
        
        if(verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }

    void PerfectDodgeCheck()
    {
        perfectDodge = Physics.CheckSphere(transform.position, perfectDodgeRadius, dodgeLayer);

        if(perfectDodge && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Perfect Dodge");
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, perfectDodgeRadius);
    }
}

