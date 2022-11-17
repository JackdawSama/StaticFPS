using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   
public class Bullets : MonoBehaviour
{
        
        float shieldsDamage;
        float healthDamage;
        float _plasmaAmmoMax;
        float _plasmaAmmoCurrent;
        float _plasmaRechargeRate;
        float _plasmaIsFull;
        float fireCD;
        public bool plasma;
        AudioClip fireSound;
    public Bullets(bool isTypePlasma)
    {

        if(isTypePlasma)
        {
            plasma = true;
        }
        else if(!isTypePlasma)
        {
            plasma = false;
        }
    }

    public class Base
    {
        float shieldsDamage;
        float healthDamage;
    }
    public class PlasmaBullet : Base
    {
        public float currentAmmo = 12f;
        public float maxAmmo = 50f;
        public float rechargeRate = 10f;

        public bool isFull = false;

        public void chargeBullet()
        {
            if(currentAmmo < maxAmmo)
            {
                currentAmmo += Time.deltaTime * rechargeRate;
            }

            handleMagLimit();
        }

        public void handleMagLimit()
        {
            if(currentAmmo >= maxAmmo)
            {
                currentAmmo = maxAmmo;
                isFull = true;
            }

            if(currentAmmo <= 0.5f)
            {
                currentAmmo = 0;
                isFull = false;
            }
        }

    }
}
