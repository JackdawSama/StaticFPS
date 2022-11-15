using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    class PlasmaBullet
    {
        float shieldsDamage;
        float healthDamage;
        float currentAmmo = 12f;
        float maxAmmo = 50f;
        float rechargeRate = 10f;

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
            }

            if(currentAmmo <= 0.5f)
            {
                currentAmmo = 0;
            }
        }

    }

    class KineticBullet
    {
        float shieldsDamage;
        float healthDamage;
    }
}
