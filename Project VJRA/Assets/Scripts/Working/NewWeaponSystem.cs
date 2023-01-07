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
        public Bullet(bool isTypePlasma)
        {

            if(isTypePlasma)
            {
                _isPlasma = true;
                shieldsDamage = 8f;
                healthDamage = 3f;
                tag = "Plasma";
            }
            else if(!isTypePlasma)
            {
                _isPlasma = false;
                shieldsDamage = 20f;
                healthDamage = 40f;
                tag = "Kinetic";
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

    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;


    // Start is called before the first frame update
    void Start()
    {
        magIsFull = false;
        Magazine = new Bullet[magSize];

        //gets a reference to the PlayerController component
        player = GetComponent<PlayerController>();

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
            if(Magazine[0]._isPlasma)
            {
                fireCD = plasmaCD;
            }
            else if(!Magazine[0]._isPlasma)
            {
                fireCD = kineticCD;
            }
        }

        if(Input.GetMouseButton(0) && fireTimer > fireCD)
        {
            HandleFire();
        }

        SetMagCursor();

        DebugCheck();

        if(!magIsFull)
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
                Debug.Log("Bullet No. " + (i+1) + ". " + Magazine[i]._isPlasma);
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
            return;
        }
        
        CheckBulletType();

        if(isTypePlasma)
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
                }

                audioSource.PlayOneShot(plasmaSounds[Random.Range(0,plasmaSounds.Length - 1)]);

                TrailRenderer trail = Instantiate(bulletTrail_Plasma, projectileSpawn.position, Quaternion.identity);
                StartCoroutine(Projectile(trail,hit));
                fireTimer = 0;
            }

        }
        else if(!isTypePlasma)
        {
            Debug.Log("Fired Kinetic");
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, kineticRange))
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                EnemyAI critterEnemy = hit.transform.GetComponent<EnemyAI>();


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
    }

    void HandlePlasmaChecks()
    {
        if(Magazine[0] != null && Magazine[0]._isPlasma)
        {
            Magazine[0].EmptyCheck();
        }

        if(Magazine[0] != null && Magazine[0]._isPlasma && Magazine[0]._plasmaIsEmpty)
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
                AddtoMag(new Bullet(true));
                magCursor++;
                Debug.Log("Added Plasma");
                reloadTimer = 0;
            }
        }
    }

    void HandleKineticReload()
    {
        if(player.state == PlayerController.MovementState.dashing)
        {
            AddtoMag(new Bullet(false));
            magCursor++;
            Debug.Log("Added Kinetic");
        }
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
            else if(Magazine[i].tag == "Kinetic" || Magazine[i].tag == "Plasma")
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
