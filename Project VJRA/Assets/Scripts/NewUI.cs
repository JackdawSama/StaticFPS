using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUI : MonoBehaviour
{
    [SerializeField] GameObject[] MagazineUI;
    [SerializeField] GameObject plasmaBullet;
    [SerializeField] NewWeaponSystem Gun;
    Slider plasmaSlider;
    [SerializeField] GameObject kineticBullet;
    // Start is called before the first frame update
    void Start()
    {
        Gun = GetComponent<NewWeaponSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlasmaAmmo(float ammo)
    {

    }
}
