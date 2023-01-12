using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUI : MonoBehaviour
{
    [SerializeField] GameObject[] MagazineUI_Plasma;
    [SerializeField] GameObject[] MagazineUI_Kinetic;
    [SerializeField] GameObject[] MagazineUI_GG;
    [SerializeField] NewWeaponSystem Gun;
    [SerializeField] Slider plasmaSlider;

    string[] RefMag;

    // Start is called before the first frame update
    void Start()
    {
        RefMag = new string[Gun.magSize];

        for(int i = 0; i < Gun.magSize; i++)
        {
            MagazineUI_Kinetic[i].SetActive(false);
            MagazineUI_Plasma[i].SetActive(false);
            MagazineUI_GG[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMagUI();
            
        if(RefMag[0] != "Null")
        {    
           SetPlasmaAmmo(Gun.Magazine[0]._plasmaAmmoCurrent);
        }
    }

    public void SetPlasmaAmmo(float ammo)
    {
        RefMagUpdate();

        if(RefMag[0] == "Plasma")
        {
            plasmaSlider.value = ammo;
        }
    }

    public void RefMagUpdate()
    {
        for(int i = 0; i < Gun.magSize; i++)
        {
            if(Gun.Magazine[i] != null)
            {
                RefMag[i] = Gun.Magazine[i].tag;
            }
            else if(Gun.Magazine[i] == null)
            {
                RefMag[i] = "Null";
            }
        }
    }

    public void UpdateMagUI()
    {
        RefMagUpdate();

        for(int i = 0; i < Gun.magSize; i++)
        {
            if(RefMag[i] == "Kinetic")
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(true);
                MagazineUI_GG[i].SetActive(false);
            }
            else if(RefMag[i] == "Plasma")
            {
                MagazineUI_Plasma[i].SetActive(true);
                MagazineUI_Kinetic[i].SetActive(false);
                MagazineUI_GG[i].SetActive(false);
            }
            else if(RefMag[i] == "GoldenGun")
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(false);
                MagazineUI_GG[i].SetActive(true);

            }
            else if(RefMag[i] == "Null")
            {
                MagazineUI_Plasma[i].SetActive(false);
                MagazineUI_Kinetic[i].SetActive(false);
                MagazineUI_GG[i].SetActive(false);
            }
        }
    }
}
