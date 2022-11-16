using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    public class PlasmaBullet
    {
        float shieldsDamage;
        float healthDamage;
        float currentAmmo = 12f;
        float maxAmmo = 50f;
        float rechargeRate = 10f;

        bool isFull = false;

        void chargeBullet()
        {
            if(currentAmmo < maxAmmo)
            {
                currentAmmo += Time.deltaTime * rechargeRate;
            }

            handleMagLimit();
        }

        void handleMagLimit()
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

    class KineticBullet
    {
        float shieldsDamage;
        float healthDamage;
    }
}
