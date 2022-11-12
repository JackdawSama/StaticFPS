using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    //PLASMA Weapon Variables
    [SerializeField] float maxAmmo_Plasma;
    [SerializeField] float currentAmmo_Plasma;
    [SerializeField] float plasmaDamage;
    [SerializeField] float plasmaCD;
    [SerializeField] float plasmaFireTimer;
    
    //KINETIC Weapon variables
    [SerializeField] float maxAmmo_Kinetic;
    [SerializeField] float currentAmmo_Kinetic;
    [SerializeField] float kineticDamage;
    [SerializeField] float kineticCD;
    [SerializeField] float kineticFireTimer;
    [SerializeField] float kineticRange;

    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleFirePlasma()
    {
        //handles the plasma fire of the weapon
        //set to primary aka left click
        //projectile
        //high rate of fire low damage per charge
        //large magazine
    }

    void HandleFireKinetic()
    {
        //handles the kinetic fire of the weapon
        //set to secondary aka right click
        //hitscan
        //low rate of fire high damage per shot
        //small magaine : 2
    }

    void HandleRelod()
    {
        //handles reload for both plasma and kinetic
        //reloads plasma when moving
        //reloads kinetic when dashing

        if(player.state == PlayerController.MovementState.walking)
        {
            //add plasma ammo
        }
        else if(player.state == PlayerController.MovementState.dashing)
        {
            //add kinetic ammo
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0f;
        Vector3 startPos = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }
}
