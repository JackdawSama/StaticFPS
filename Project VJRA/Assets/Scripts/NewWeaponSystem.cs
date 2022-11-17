using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Bullet;

public class NewWeaponSystem : MonoBehaviour
{
    //BULLET Class
    public class Bullet
    {
        float shieldsDamage;
        float healthDamage;
        float _plasmaAmmoMax = 50f;
        public float _plasmaAmmoCurrent;
        float _plasmaRechargeRate = 5f;
        public bool _isPlasma;
        public bool _plasmaIsFull = false;
        public bool _plasmaIsEmpty = false;
        public bool canFire = false;
        public Bullet(bool isTypePlasma)
        {

            if(isTypePlasma)
            {
                _isPlasma = true;
                shieldsDamage = 12f;
                healthDamage = 5f;
            }
            else if(!isTypePlasma)
            {
                _isPlasma = false;
                shieldsDamage = 5f;
                healthDamage = 15f;
            }
        }
        public void chargeBullet()
        {
            if(_plasmaAmmoCurrent < _plasmaAmmoMax)
            {
                _plasmaAmmoCurrent += Time.deltaTime * _plasmaRechargeRate;
            }

            LimitCharge();
        }

        public void LimitCharge()
        {
            if(_plasmaAmmoCurrent >= _plasmaAmmoMax)
            {
                _plasmaAmmoCurrent = _plasmaAmmoMax;
                _plasmaIsFull = true;
                canFire = true;
            }
        }

        public void EmptyCheck()
        {
            if(_plasmaAmmoCurrent <= 0.5f)
            {
                _plasmaAmmoCurrent = 0;
                _plasmaIsEmpty = true;
            }
        }

        public void FireCharge()
        {
            EmptyCheck();
            _plasmaAmmoCurrent--;
        }
    }
    
    //COMMON Weapon Variables
    [SerializeField] float reloadTimer;
    [SerializeField] float reloadCD;
    [SerializeField] bool isTypePlasma;
    [SerializeField] float fireTimer;
    [SerializeField] float fireCD;
    [SerializeField] AudioSource audioSource;

    //MAGAZINE Variables
    [SerializeField] int magSize = 8;
    [SerializeField] int magCursor;
    [SerializeField] int magCounter;
    [SerializeField] bool magIsFull;
    [SerializeField] public Bullet[] Magazine;

    //PLASMA Weapon Variables
    [SerializeField] public float plasmaRange;
    [SerializeField] TrailRenderer bulletTrail_Plasma;
    
    //KINETIC Weapon variables
    [SerializeField] public float kineticRange;
    [SerializeField] bool ammoFull_Kinetic;
    [SerializeField] TrailRenderer bulletTrail_Kinetic;

    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;

    bool check;


    // Start is called before the first frame update
    void Start()
    {
        magIsFull = false;
        Magazine = new Bullet[magSize];
        Magazine[0] = new Bullet(false);
        Magazine[1] = new Bullet(true);
        for(int i = 2; i < magSize; i++)
        {
            Magazine[i] = null;
        }

        check = true;

        for(int i =0; i < magSize; i++)
        if(Magazine[i] != null)
        {
            Debug.Log(Magazine[i]);
        }

        //handleAmmoStatus();

        //gets a reference to the PlayerController component
        player = GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();

        reloadTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //handleAmmoStatus();

        fireTimer += Time.deltaTime;
        reloadTimer += Time.deltaTime;



        if(Input.GetMouseButton(0) && fireTimer > fireCD)
        {
            HandleFire();

            check = true;
            if(check)
            {
                Debug.Log(magCounter);
                for(int i =0; i < magSize; i++)
                {
                    if(Magazine[i] != null)
                    {
                        Debug.Log(Magazine[i]);
                    }
                }
                check = false;
            }

            Debug.Log(magCounter);
        }

        CheckMagSize();
        if(check)
        {
            //Debug.Log(magCounter);
            for(int i =0; i < magSize; i++)
            {
                if(Magazine[i] != null)
                {
                    Debug.Log(Magazine[i]);
                }
            }
            check = false;
        }

        if(reloadTimer > reloadCD)
        {
            HandleRelod();
        }

    }

    void HandleFire()
    {
        //check for bullet type and fire
        CheckBulletType();
        if(isTypePlasma && Magazine[0].canFire)
        {
            if(Magazine[0]._plasmaIsEmpty)
            {
                RemovefromMag();
                return;
            }
            Debug.Log("Fired Plasma");
            Magazine[0].FireCharge();
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, plasmaRange))
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();


                if(enemy != null)
                {
                    //enemy.burnShields(plasmaDamage);
                    Debug.Log("Hit Enemy");
                }

                TrailRenderer trail = Instantiate(bulletTrail_Plasma, projectileSpawn.position, Quaternion.identity);

                StartCoroutine(Projectile(trail,hit));
            }
            fireTimer = 0;

        }
        else if(!isTypePlasma)
        {
            Debug.Log("Fired Kinetic");
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, kineticRange))
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();


                if(enemy != null)
                {
                    //enemy.burnShields(plasmaDamage);
                    Debug.Log("Hit Enemy");
                }

                TrailRenderer trail = Instantiate(bulletTrail_Kinetic, projectileSpawn.position, Quaternion.identity);

                StartCoroutine(Projectile(trail,hit));
            }
            RemovefromMag();
            fireTimer = 0;
        }
    }

    void HandleRelod()
    {
        CheckBulletType();
        if(player.state == PlayerController.MovementState.walking)
        {
            if(isTypePlasma)
            {
                if(!Magazine[0]._plasmaIsFull)
                {
                    //Debug.Log("Charging Plasma");
                    Magazine[0].chargeBullet();
                    return;
                }
            }
            AddtoMag(new Bullet(true));
            magCounter++;
            Debug.Log("Added Plasma");
        }
        else if(player.state == PlayerController.MovementState.dashing)
        {
            AddtoMag(new Bullet(false));
            magCounter++;
            Debug.Log("Added Kinetic");
        }
        reloadTimer = 0;
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

    void CheckMagSize()
    {
        if(Magazine == null)
        {
            magIsFull = false;
            magCounter = 0;
            return;
        }

        if(!magIsFull)
        { 
            for(int i = 0; i < magSize; i++)
            {
                magCounter =0;
                
                if(Magazine[i] != null)
                {
                    magCounter++;
                }
                else if(Magazine[i] == null)
                {
                    break;
                }
            }

            if(magCounter < magSize)
            {
                magIsFull = false;
            }
            else if(magCounter >= magSize)
            {
                magCounter = magSize;
                magIsFull = true;
            }
        }
        
    }

    void SetMagCursor()
    {
        for(int i = 0; i < magSize; i++)
        {
            if(Magazine[i] == null)
            {
                magCursor = i;
            }
        }
    }
    void AddtoMag(Bullet bulletType)
    {
        SetMagCursor();

        Magazine[magCursor] = bulletType;
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
