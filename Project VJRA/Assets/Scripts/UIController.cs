using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text kineticAmmo;
    [SerializeField] Text plasmaAmmo;

    Text enemyHealth;
    Text enemyShields;

    WeaponSystem weaponRef;
    EnemyController enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        kineticAmmo.text =  weaponRef.currentAmmo_Kinetic.ToString();
        plasmaAmmo.text =  weaponRef.currentAmmo_Plasma.ToString();

        enemyHealth.text = enemy.health.ToString();
        enemyShields.text = enemy.shields.ToString();
    }
}
