using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    //COMMON Weapon Variables
    [SerializeField] float reloadTimer;
    [SerializeField] float reloadCD;

    //PLASMA Weapon Variables
    [SerializeField] float maxAmmo_Plasma;
    [SerializeField] float currentAmmo_Plasma;
    [SerializeField] GameObject plasmaProjectile;
    [SerializeField] bool ammoFull_Plasma;
    [SerializeField] float plasmaDamage;
    [SerializeField] float plasmaRecharge = 1f;
    [SerializeField] float plasmaCD;
    [SerializeField] float plasmaFireTimer;
    
    //KINETIC Weapon variables
    [SerializeField] int maxAmmo_Kinetic;
    [SerializeField] int currentAmmo_Kinetic;
    [SerializeField] bool ammoFull_Kinetic;
    [SerializeField] float kineticDamage;
    [SerializeField] float kineticCD;
    [SerializeField] float kineticFireTimer;
    [SerializeField] float kineticRange;
    [SerializeField] TrailRenderer bulletTrail;

    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;


    // Start is called before the first frame update
    void Start()
    {
        //gets a reference to the PlayerController component
        player = GetComponent<PlayerController>();

        //sets timers to 0
        plasmaFireTimer = 0;
        kineticFireTimer = 0;
        reloadTimer = 0;

        //set ammo
        currentAmmo_Plasma = maxAmmo_Plasma/2;
        currentAmmo_Kinetic = 0;

    }

    // Update is called once per frame
    void Update()
    {
        handleAmmoStatus();

        plasmaFireTimer += Time.deltaTime;
        kineticFireTimer += Time.deltaTime;

        if(Input.GetMouseButton(0) && plasmaFireTimer > plasmaCD)
        {
            HandleFirePlasma();
            plasmaFireTimer = 0;
        }

        if(Input.GetMouseButton(1) && kineticFireTimer > kineticCD)
        {
            HandleFireKinetic();
            kineticFireTimer = 0;
        }

        if(!ammoFull_Plasma || !ammoFull_Kinetic)
        {
            reloadTimer += Time.deltaTime;

            if(reloadTimer > reloadCD && player.state == PlayerController.MovementState.walking || player.state == PlayerController.MovementState.dashing)
            {
                HandleRelod();
            }
        }
    }

    void HandleFirePlasma()
    {
        // plasmaisFiring = true;
        if(currentAmmo_Plasma <= 0)
        {
            currentAmmo_Plasma = 0;
            Debug.Log("Plasma Rifle Empty");
            return;
        }

        Instantiate(plasmaProjectile, projectileSpawn.position, projectileSpawn.rotation);
        currentAmmo_Plasma--;
        Debug.Log("Plasma Fire. Ammo Left : " + currentAmmo_Plasma);
    }

    void HandleFireKinetic()
    {
        if(currentAmmo_Kinetic <= 0)
        {
            currentAmmo_Kinetic = 0;
            Debug.Log("Kinetic Rifle Empty");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, kineticRange))
        {
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();

            if(enemy != null)
            {
                enemy.TakeDamage(kineticDamage);
            }
            currentAmmo_Kinetic--;

            TrailRenderer trail = Instantiate(bulletTrail, projectileSpawn.position, Quaternion.identity);

            StartCoroutine(KineticProjectile(trail,hit));
            Debug.Log("Plasma Fire. Ammo Left : " +  currentAmmo_Kinetic);
        }
    }

    void handleAmmoStatus()
    {
        if(currentAmmo_Plasma >= maxAmmo_Plasma)
        {
            currentAmmo_Plasma = maxAmmo_Plasma;
            ammoFull_Plasma = true;
        }
        else ammoFull_Plasma = false;

        if(currentAmmo_Kinetic >= maxAmmo_Kinetic)
        {
            currentAmmo_Kinetic = maxAmmo_Kinetic;
            ammoFull_Kinetic = true;
        }
        else ammoFull_Kinetic = false;
    }

    void HandleRelod()
    {
        if(player.state == PlayerController.MovementState.walking)
        {
            currentAmmo_Plasma += plasmaRecharge;
        }
        else if(player.state == PlayerController.MovementState.dashing)
        {
            currentAmmo_Kinetic++;
        }
        reloadTimer = 0;
    }

    private IEnumerator KineticProjectile(TrailRenderer trail, RaycastHit hit)
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
