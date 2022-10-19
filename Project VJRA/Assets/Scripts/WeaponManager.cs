using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    float damage = 10f;
    [SerializeField]
    float range = 100f;
    [SerializeField]
    float fireRate = 15f;

    public Camera playerCamera;

    private float nextTimetoFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton ("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f/fireRate;
            HandleFire();
        }
    }

    void HandleFire()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, -playerCamera.transform.forward, out hit, range))
        {

        }
    }
}
