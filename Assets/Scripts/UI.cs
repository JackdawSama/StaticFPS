using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text healthText = default;
    [SerializeField] private Text chargeText = default;

    private void OnEnable()
    {
        FPSController.OnDamage += UpdateHealth;
        FPSController.OnHeal += UpdateHealth;

        FPSController.OnStaminaChange += UpdateCharge;
    }

    private void OnDisable()
    {
        FPSController.OnDamage -= UpdateHealth;
        FPSController.OnHeal -= UpdateHealth;

        FPSController.OnStaminaChange -= UpdateCharge;
    }

    private void Start()
    {
        UpdateHealth(100);
        UpdateCharge(100);
    }
    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString("00");
    }
    private void UpdateCharge(float currentCharge)
    {
        chargeText.text = currentCharge.ToString("00");
    }
}
