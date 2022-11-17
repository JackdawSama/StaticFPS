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

    string[] RefMag;

    // Start is called before the first frame update
    void Start()
    {
        RefMag = new string[Gun.magSize];

        for(int i = 0; i < Gun.magSize; i++)
        {
            MagazineUI_Kinetic[i].SetActive(false);
            MagazineUI_Plasma[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMagUI();
        if(RefMag[0] == "Plasma")
        {
            SetPlasmaAmmo(Gun.Magazine[0]._plasmaAmmoCurrent);
        }
    }

    public void SetPlasmaAmmo(float ammo)
    {
        if(Gun.Magazine[0]._isPlasma)
        {
            plasmaSlider[0].value = ammo;
        }
    }

    public void RefMagUpdate()
    {
        for(int i = 0; i < Gun.magSize; i++)
        {
            if(Gun.Magazine[i] != null)
            {
                RefMag[i] = Gun.Magazine[i].tag;
                // Debug.Log("Updated REF Mag");
            }
            else return;
        }
    }

    public void UpdateMagUI()
    {
        RefMagUpdate();

        for(int i = 0; i < Gun.magSize; i++)
        {
            if(RefMag == null)
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(false);
                return;
            }
            if(RefMag[i] == "Kinetic")
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(true);
            }
            else if(RefMag[i] == "Plasma")
            {
                MagazineUI_Plasma[i].SetActive(true);
                MagazineUI_Kinetic[i].SetActive(false);
            }
        }
    }
}
