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
        float _plasmaAmmoMax;
        float _plasmaAmmoCurrent;
        float _plasmaRechargeRate;
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
    [SerializeField] bool magIsFull;
    Bullet[] Magazine;

    //PLASMA Weapon Variables
    [SerializeField] public float currentAmmo_Plasma;
    [SerializeField] TrailRenderer bulletTrail_Plasma;
    
    //KINETIC Weapon variables
    [SerializeField] public int currentAmmo_Kinetic;
    [SerializeField] bool ammoFull_Kinetic;
    [SerializeField] TrailRenderer bulletTrail_Kinetic;

    //PLAYER variables
    [SerializeField] PlayerController player;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform projectileSpawn;

    [SerializeField] int Kinetics;
    [SerializeField] int Plasmas;
    [SerializeField] int ammoInMag;


    // Start is called before the first frame update
    void Start()
    {
        Magazine = new Bullet[magSize];
        Magazine[0] = new Bullet(true);
        Magazine[1] = new Bullet(false);

        handleAmmoStatus();

        //gets a reference to the PlayerController component
        player = GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();

        reloadTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //handleAmmoStatus();

        if(Input.GetMouseButton(0) && fireTimer > fireCD)
        {
            HandleFire();
        }

        // if(Input.GetMouseButton(1)/*&& fireTimer > fireCD*/)
        // {
        //     HandleFire();
        // }

        if(!magIsFull && reloadTimer > reloadCD)
        {
            HandleRelod();
        }
    }

    /*void HandleFirePlasma()
    {
        // plasmaisFiring = true;
        if(currentAmmo_Plasma <= 0)
        {
            currentAmmo_Plasma = 0;
            Debug.Log("Plasma Rifle Empty");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, plasmaRange))
        {
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();


            if(enemy != null)
            {
                //enemy.burnShields(plasmaDamage);
            }
            currentAmmo_Plasma--;

            TrailRenderer trail = Instantiate(bulletTrail_Kinetic, projectileSpawn.position, Quaternion.identity);

            StartCoroutine(KineticProjectile(trail,hit));
            Debug.Log("Plasma Fire. Ammo Left : " +  currentAmmo_Kinetic);
        }
    }*/

    void handleAmmoStatus()
    {

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
            fireTimer = 0;
        }
        else if(!isTypePlasma)
        {
            Debug.Log("Fired Kinetic");
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
                    Debug.Log("Charging Plasma");
                    Magazine[0].chargeBullet();
                    return;
                }
            }
            AddtoMag(new Bullet(true));
            Debug.Log("Added Plasma");
        }
        else if(player.state == PlayerController.MovementState.dashing)
        {
            AddtoMag(new Bullet(false));
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
