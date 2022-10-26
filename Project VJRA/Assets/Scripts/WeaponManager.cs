using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    float fullDamage = 10f;

    [SerializeField]
    float lowDamage = 5f;

    float damage;

    [SerializeField]
    float range = 100f;
    [SerializeField]
    float fireRate = 15f;

    [SerializeField]
    int maxAmmo = 10;
    [SerializeField]
    int currentAmmo;
    bool noAmmo;
    public Text currentAmmoUI;

    [SerializeField]
    private TrailRenderer bulletTrail;

    public Transform bulletSpawnPoint;
    public Camera playerCamera;

    public PlayerMovement player;
    public PlayerController playerRB;
    private Vector3 playerLastPos;

    private float nextTimetoFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
        playerLastPos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //HandleReload();

        if(Input.GetButton ("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f/fireRate;
            HandleFire();
        }
    }

    void HandleFire()
    {

        HandleAmmoDamage();

        if(!noAmmo)
        {
            currentAmmo--;
            currentAmmoUI.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();

        if(currentAmmo <= 0)
            {
                noAmmo = true;
                Debug.Log(noAmmo);
                currentAmmo = 0;
            }

            RaycastHit hit;
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
            {

                EnemyController enemy =  hit.transform.GetComponent<EnemyController>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, hit));
            }
        }
        else
        {
            Debug.Log("MOVE TO REFILL");
        }
    }

    void HandleAmmoDamage()
    {
        if(player.transform.position != playerLastPos && player.enemyIsNearby && player.isDashing)
        {
            damage = lowDamage;
            //Debug.Log("Low DMG" + damage);
            playerLastPos = player.transform.position;
        }
        else if(player.transform.position != playerLastPos || player.isDashing)
        {
            damage = lowDamage;
            //Debug.Log("Low DMG" + damage);
            playerLastPos = player.transform.position;
        }
        else
        {
            damage = fullDamage;
            //Debug.Log("Full DMG" + damage);
            playerLastPos = player.transform.position;
        }
    }

    void HandleReload()
    {
        if(playerRB.moveSpeed > 7)
        {
            Debug.Log("Coroutine started");
            StartCoroutine("loadAmmo");
            
            Debug.Log(currentAmmo);
        }

        if(currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }


        // if(player.transform.position != playerLastPos)
        // {
        //     currentAmmo++;
        // }

        // if(currentAmmo > maxAmmo)
        // {
        //     currentAmmo = maxAmmo;
        // }
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

    private IEnumerator loadAmmo()
    {
        float time = 0f;

        while(time < 0.3)
        {
            currentAmmo = currentAmmo + 1;
            time += Time.deltaTime/5;
        }
        Debug.Log("Coroutine completed");
        yield return null;
    }
}
