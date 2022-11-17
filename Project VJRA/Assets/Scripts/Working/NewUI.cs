using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUI : MonoBehaviour
{
    [SerializeField] GameObject[] MagazineUI_Plasma;
    [SerializeField] GameObject[] MagazineUI_Kinetic;
    [SerializeField] NewWeaponSystem Gun;
    [SerializeField] Slider[] plasmaSlider;
    // Start is called before the first frame update
    void Start()
    {
        //Gun = GetComponent<NewWeaponSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMagUI();
        SetPlasmaAmmo(Gun.Magazine[0]._plasmaAmmoCurrent);
    }

    public void SetPlasmaAmmo(float ammo)
    {
        if(Gun.Magazine[0]._isPlasma)
        {
            plasmaSlider[0].value = ammo;
        }
    }

    public void UpdateMagUI()
    {
        for(int i = 0; i < Gun.magSize; i++)
        {
            if(Gun.Magazine[i]._isPlasma)
            {
                MagazineUI_Plasma[i].SetActive(true);
                MagazineUI_Kinetic[i].SetActive(false);
            }
            else if(!Gun.Magazine[i]._isPlasma)
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(true);
            }
            else if(Gun.Magazine[i] == null)
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(false);
            }
        }
    }
}
