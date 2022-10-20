using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    float damage = 10f;
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

    public Camera playerCamera;

    private float nextTimetoFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
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
        if(!noAmmo)
        {
            currentAmmo --;
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
                EnemyController enemy =  hit.transform.GetComponent<EnemyController>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
        else
        {
            Debug.Log("MOVE TO REFILL");
        }
    }
}
