using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewWeponManager : MonoBehaviour
{
    class Bullet
    {
        public float damage;
        Color bulletUIColour;

        [SerializeField] float lowDamage = 12f;
        [SerializeField] float fullDamage = 25f;

        [SerializeField] Color lowDamageUI;
        [SerializeField] Color fullDamageUI;

        bool isHigh = true;

        public Bullet(bool isHigh)
        {
            if(isHigh)
            {
                damage = fullDamage;
                bulletUIColour = fullDamageUI;
            }
            else if(!isHigh)
            {
                damage = lowDamage;
                bulletUIColour = lowDamageUI;
            }
            
        }

    }

    //SECTION : BULLETS
    [SerializeField] int maxBullets = 8;

    //Bullet fullBullets;
    //Bullet lowBullets;
    Bullet bullets;
    //Bullet currentBullet;
    List<Bullet> Magazine1;
    //SECTION END

    //SECTION : WEAPON
    int currentAmmo;
    bool noAmmo;
    bool isReloading;
    float fireCD = 0f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 5f;
    //SECTION END

    //SECTION : OTHER STUFF
    [SerializeField] Camera playerCamera;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] PlayerController playerRB;





    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i <= maxBullets/2; i++)
        {
            Magazine1.Add(bullets = new Bullet(true));

            currentAmmo++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleFire()
    {
        if(!noAmmo)
        {
            Magazine1.Remove(bullets);
            currentAmmo--;
        }

        if(currentAmmo <= 0)
        {
            noAmmo = true;
            currentAmmo = 0;
        }

        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(bullets.damage);
            }

            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail,hit));
        }
    }
    void handleReload()
    {
        if(playerRB.state == PlayerController.MovementState.walking || playerRB.state == PlayerController.MovementState.sprinting)
        {
            Magazine1.Add(bullets = new Bullet(true));
        }
        else if(playerRB.state == PlayerController.MovementState.sprinting && playerRB.enemyIsNearby)
        {
            Magazine1.Add(bullets = new Bullet(false));
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
