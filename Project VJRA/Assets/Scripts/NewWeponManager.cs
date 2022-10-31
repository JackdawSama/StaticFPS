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

        bool isHigh;

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
    Bullet bullets;
    List<Bullet> Magazine1;
    //SECTION END

    //SECTION : WEAPON
    [SerializeField] int currentAmmo;
    bool noAmmo;
    bool magazineFull;
    bool isReloading = false;
    float timeToReload;
    float fireCD = 0f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 2f;
    //SECTION END

    //SECTION : OTHER STUFF
    [SerializeField] Camera playerCamera;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] PlayerController playerRB;





    // Start is called before the first frame update
    void Start()
    {
        Magazine1 = new List<Bullet>();
        for(int i = 0; i <= maxBullets/2; i++)
        {
            Magazine1.Add(new Bullet(true));

            currentAmmo++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleMagazine();
    
        if(Input.GetButton ("Fire1") && Time.time >= fireCD)
        {
            fireCD = Time.time + 1f/fireRate;
            HandleFire();
        }

        if(!magazineFull)
        {
            timeToReload = timeToReload + Time.deltaTime;

            if(timeToReload > 5 && playerRB.state == PlayerController.MovementState.walking || playerRB.state == PlayerController.MovementState.sprinting && !isReloading)
            {
                isReloading = true;
                StartCoroutine(handleReload());
                timeToReload = 0;
            }
        }
    }

    void HandleFire()
    {
        if(noAmmo)
        {
            Debug.Log("No bullets");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(Magazine1[0].damage);
            }

            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail,hit));
        }

        Magazine1.RemoveAt(0);
        //currentAmmo--;
        Debug.Log("Debug from HandleFire : " + Magazine1.Count);
        
        if(Magazine1.Count <= 0)
        {
            noAmmo = true;
        }
    }

    void handleMagazine()
    {
        if(Magazine1.Count >= maxBullets)
        {
            float magDiff = Mathf.Abs(maxBullets - Magazine1.Count);
            for(int i = 0; i < magDiff; i++)
            {
                Magazine1.RemoveAt(0);
            }
            magazineFull = true;
            return;
        }
        if(Magazine1.Count < maxBullets)
        {
            magazineFull = false;
            return;
        }
        Debug.Log("Debug from Handle Mag : " + Magazine1.Count);
    }
    private IEnumerator handleReload()
    {

        if(playerRB.state == PlayerController.MovementState.walking)
        {
            yield return new WaitForSeconds(2);
            Magazine1.Add(new Bullet(true));
            Debug.Log("Added bullet full");
        }
        else if(playerRB.state == PlayerController.MovementState.sprinting && playerRB.enemyIsNearby)
        {
            yield return new WaitForSeconds(2);
            Magazine1.Add(new Bullet(false));
            Debug.Log("Added bullet weak");
        }

        isReloading = false;
        noAmmo = false;
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
