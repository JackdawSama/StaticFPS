using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Bullet;

public class NewWeaponSystem : MonoBehaviour
{
    //BULLET Class
    public class Bullet
    {
        public float shieldsDamage;
        public float healthDamage;
        //int _plasmaAmmoMax = 10;
        public float _plasmaAmmoCurrent = 12f;
        public bool _isPlasma;
        public bool _plasmaIsFull = false;
        public bool _plasmaIsEmpty = false;
        public bool canFire = false;
        public string tag;
        public Bullet(string ammo)
        {

            if(ammo == "Plasma")
            {
                //_isPlasma = true;
                shieldsDamage = 8f;
                healthDamage = 5f;
                tag = "Plasma";
            }
            else if(ammo == "Kinetic")
            {
                //_isPlasma = false;
                shieldsDamage = 32f;
                healthDamage = 40f;
                tag = "Kinetic";
            }
            else if(ammo == "GoldenGun")
            {
                tag = "GoldenGun";
            }
        }

        public void EmptyCheck()
        {
            if(_plasmaAmmoCurrent < 1f)
            {
                _plasmaAmmoCurrent = 0;
                Debug.Log("Plasma is EMPTY");
                _plasmaIsEmpty = true;
            }
        }

        public void FireCharge()
        {
            //EmptyCheck();
            if(!_plasmaIsEmpty)
            {
                _plasmaAmmoCurrent--;
            }
        }
    }
    //END of BULLET Class

    //COMMON Weapon Variables
    [SerializeField] float reloadTimer;
    [SerializeField] float reloadCD;
    [SerializeField] bool isTypePlasma;
    [SerializeField] float fireTimer;
    [SerializeField] float fireCD;
    [SerializeField] AudioSource audioSource;

    //MAGAZINE Variables
    [SerializeField] public int magSize = 8;
    [SerializeField] int magCursor;
    [SerializeField] bool magIsFull;
    [SerializeField] public Bullet[] Magazine;

    //PLASMA Weapon Variables
    [SerializeField] public float plasmaCD;
    [SerializeField] public float plasmaRange;
    [SerializeField] TrailRenderer bulletTrail_Plasma;
    [SerializeField] AudioClip[] plasmaSounds;
    
    //KINETIC Weapon variables
    [SerializeField] public float kineticCD;
    [SerializeField] public float kineticRange;
    [SerializeField] bool ammoFull_Kinetic;
    [SerializeField] TrailRenderer bulletTrail_Kinetic;
    [SerializeField] AudioClip kineticSound;
    [SerializeField] ParticleSystem muzzleFlash;

    //GOLDENGUN Variables
    [SerializeField] public bool enableGg = false;
    [SerializeField] TrailRenderer bulletTrail_GG;


    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] PlayerDash dashData;


    // Start is called before the first frame update
    void Start()
    {
        magIsFull = false;
        Magazine = new Bullet[magSize];

        //gets a reference to the PlayerController component
        player = GetComponent<PlayerController>();
        dashData = GetComponent<PlayerDash>();

        audioSource = GetComponent<AudioSource>();

        reloadTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        HandlePlasmaChecks();

        fireTimer += Time.deltaTime;

        if(Magazine[0] != null)
        {
            if(Magazine[0].tag == "Plasma")
            {
                fireCD = plasmaCD;
            }
            else if(Magazine[0].tag == "Kinetic")
            {
                fireCD = kineticCD;
            }
            else if(Magazine[0].tag == "GoldenGun")
            {
                fireCD = kineticCD;
            }
        }

        if(dashData.GGisActive)
        {
            enableGg = true;
            if(enableGg)
            {
                HandleGoldenGunReload();
                dashData.GGisActive = false;
                Debug.Log(dashData.GGisActive);
                dashData.perfectDodge = false;
            }
        }

        if(Magazine[0] == null)
        {
            enableGg = false;
        }

        if(Input.GetMouseButton(0) && fireTimer > fireCD)
        {
            HandleFire();
        }

        SetMagCursor();

        DebugCheck();

        if(!magIsFull && !enableGg)
        {
            HandlePlasmaRelod();
            HandleKineticReload();
        }

    }

    void DebugCheck()
    {
        if(Input.GetMouseButtonDown(2))
        {
            if(Magazine[0] == null)
            {
                Debug.Log("Mag is EMPTY");
                return;
            }

            for(int i =0; i < magSize; i++)
            {
                if(Magazine[i] != null)
                {
                Debug.Log("Bullet No. " + (i+1) + ". " + Magazine[i].tag);
                }
            }
            if(Magazine[0].tag == "Plasma")
            {
                Debug.Log("Plasma Charge at : " + Magazine[0]._plasmaAmmoCurrent);
            }
            Debug.Log("Cursor at : " + magCursor);
        }
    }

    void HandleFire()
    {
        //check for bullet type and fire
        if(Magazine[0] == null)
        {
            Debug.Log("Mag is EMPTY");
            //enableGg = false;
            return;
        }
        
        //CheckBulletType();

        if(Magazine[0].tag == "Plasma")
        {
            Debug.Log("Fired Plasma");
            if(!Magazine[0]._plasmaIsEmpty)
            {
                Magazine[0].FireCharge();
                RaycastHit hit;
                if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, plasmaRange))
                {
                    EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                    EnemyAI critterEnemy = hit.transform.GetComponent<EnemyAI>();
                    DashEnemy dashEnemy = hit.transform.GetComponent<DashEnemy>();


                    if(enemy != null)
                    {
                        enemy.TakeDamage(Magazine[0].healthDamage,Magazine[0].shieldsDamage);
                        Debug.Log("Hit Enemy");
                    }
                    else if(critterEnemy != null)
                    {
                        critterEnemy.TakeDamage(Magazine[0].healthDamage);
                        Debug.Log("Hit Critter");
                    }
                    else if(dashEnemy != null)
                    {
                        dashEnemy.TakeDamage(Magazine[0].healthDamage);
                        Debug.Log("Hit Dasher");
                    }
                }

                audioSource.PlayOneShot(plasmaSounds[Random.Range(0,plasmaSounds.Length - 1)]);

                TrailRenderer trail = Instantiate(bulletTrail_Plasma, projectileSpawn.position, Quaternion.identity);
                StartCoroutine(Projectile(trail,hit));
                fireTimer = 0;
            }

        }
        else if(Magazine[0].tag == "Kinetic")
        {
            Debug.Log("Fired Kinetic");
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, kineticRange))
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                EnemyAI critterEnemy = hit.transform.GetComponent<EnemyAI>();
                DashEnemy dashEnemy = hit.transform.GetComponent<DashEnemy>();


                if(enemy != null)
                {
                    enemy.TakeDamage(Magazine[0].healthDamage,Magazine[0].shieldsDamage);
                    Debug.Log("Hit Enemy");
                }
                else if(critterEnemy != null)
                {
                    critterEnemy.TakeDamage(Magazine[0].healthDamage);
                    Debug.Log("Hit Critter");
                }
                else if(dashEnemy != null)
                {
                    dashEnemy.TakeDamage(Magazine[0].healthDamage);
                    Debug.Log("Hit Dasher");
                }

            }

            audioSource.PlayOneShot(kineticSound);
            muzzleFlash.Play();

            TrailRenderer trail = Instantiate(bulletTrail_Kinetic, projectileSpawn.position, Quaternion.identity);
            StartCoroutine(Projectile(trail,hit));

            RemovefromMag();
            Debug.Log("Removed Kinetic");
            magCursor--;
            fireTimer = 0;
        }

        else if(Magazine[0].tag == "GoldenGun")
        {
            Debug.Log("Fired GoldenGun");
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, kineticRange))
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                EnemyAI critterEnemy = hit.transform.GetComponent<EnemyAI>();
                DashEnemy dashEnemy = hit.transform.GetComponent<DashEnemy>();


                if(enemy != null)
                {
                    enemy.Die();
                    Debug.Log("Enemy KILLED");
                }
                else if(critterEnemy != null)
                {
                    critterEnemy.Die();
                    Debug.Log("Critter KILLED");
                }
                else if(dashEnemy != null)
                {
                    dashEnemy.Die();
                    Debug.Log("Dasher KILLED");
                }

            }

            audioSource.PlayOneShot(kineticSound);
            muzzleFlash.Play();

            TrailRenderer trail = Instantiate(bulletTrail_GG, projectileSpawn.position, Quaternion.identity);
            StartCoroutine(Projectile(trail,hit));

            RemovefromMag();
            Debug.Log("Removed GG");
            magCursor--;
            fireTimer = 0;
        }
    }

    void HandlePlasmaChecks()
    {
        if(Magazine[0] != null && Magazine[0].tag == "Plasma")
        {
            Magazine[0].EmptyCheck();
        }

        if(Magazine[0] != null && Magazine[0].tag == "Plasma" && Magazine[0]._plasmaIsEmpty)
        {
            RemovefromMag();
            Debug.Log("Removed Plasma");
            magCursor--;
        }
    }
    void HandlePlasmaRelod()
    {
        if(player.state == PlayerController.MovementState.walking)
        {
            reloadTimer += Time.deltaTime;

            if(reloadTimer > reloadCD)
            {
                AddtoMag(new Bullet("Plasma"));
                magCursor++;
                Debug.Log("Added Plasma");
                reloadTimer = 0;
            }
        }
    }

    void HandleKineticReload()
    {
        //TODO : LEFT SHIFT IS HARDCODED HERE BUT NOT IN DASHING SCRIPT! REMEMBER TO PROPERLY SET THIS UP IN FUTURE!
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            AddtoMag(new Bullet("Kinetic"));
            magCursor++;
            Debug.Log("Added Kinetic");
        }
    }

    void FlushMag()
    {
        for(int i = 0; i < magSize; i++)
        {
            Magazine[i] = null;
        }
    }

    void HandleGoldenGunReload()
    {
        //TODO Add Golden Gun functionality.
        FlushMag();
        for(int i = 0; i < 4; i++)
        {
            AddtoMag(new Bullet("GoldenGun"));
            Debug.Log("Added GoldenGun");
        }
        //dashData.GGisActive = false;
    }

    void CheckBulletType()
    {
        if(Magazine[0]._isPlasma == true)
        {
            isTypePlasma = true;
        }
        else if(Magazine[0]._isPlasma == false)
        {
            isTypePlasma = false;
        }
    }

    void SetMagCursor()
    {
        magCursor = 0;

        for(int i = 0; i < magSize; i++)
        {
            if (Magazine[i] == null)
            {
                magCursor = i;
                break;
            }
            else if(Magazine[i].tag == "Kinetic" || Magazine[i].tag == "Plasma" || Magazine[i].tag == "GoldenGun")
            {
                magCursor++;
            }

            if(magCursor >= magSize)
            {
                magCursor = magSize - 1;
                magIsFull = true;
                return;
            }
            else if(magCursor < magSize)
            {
                magIsFull = false;
            }
        }
    }
    void AddtoMag(Bullet bulletType)
    {
        SetMagCursor();
        if(!magIsFull)
        {
            Magazine[magCursor] = bulletType;
        }
    }

    void RemovefromMag()
    {
        Magazine[0] = null;

        for(int i = 1; i < magSize; i++)
        {
            Magazine[i-1] = Magazine[i];
        }
        Magazine[magSize - 1] = null;
    }

    private IEnumerator Projectile(TrailRenderer trail, RaycastHit hit)
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