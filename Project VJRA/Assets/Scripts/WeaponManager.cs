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
        if(Input.GetButton ("Fire1") && Time.time >= nextTimetoFire)
        {
            nextTimetoFire = Time.time + 1f/fireRate;
            HandleFire();
        }
    }

    void HandleFire()
    {

        HandleAmmo();

        if(!noAmmo)
        {
            //currentAmmo --;
            currentAmmoUI.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();

        if(currentAmmo <= 0)
            {
                noAmmo = true;
                Debug.Log(noAmmo);
                currentAmmo = 0;
            }

            RaycastHit hit;
            if(Physics.Raycast(playerCamera.transform.position, -playerCamera.transform.forward, out hit, range))
            {
                //Debug.Log(hit.transform.name);

                EnemyController enemy =  hit.transform.GetComponent<EnemyController>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("enemy took damage");
                }

                //TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                //StartCoroutine(SpawnTrail(trail, hit));
            }
        }
        else
        {
            Debug.Log("MOVE TO REFILL");
        }
    }

    void HandleAmmo()
    {
        if(player.transform.position != playerLastPos && player.enemyIsNearby)
        {
            damage = lowDamage;
            Debug.Log("Low DMG" + damage);
            playerLastPos = player.transform.position;
        }
        else
        {
            damage = fullDamage;
            Debug.Log("Full DMG" + damage);
            playerLastPos = player.transform.position;
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
