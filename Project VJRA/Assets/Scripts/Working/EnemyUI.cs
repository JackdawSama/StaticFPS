using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI shieldsText;

    [SerializeField] EnemyController enemy;
    // Start is called before the first frame update
    void Start()
    {
        setEnemyUI();
    }

    // Update is called once per frame
    void Update()
    {
        setEnemyUI();
    }

    void setEnemyUI()
    {
        healthText.text = "HEALTH : " + enemy.health;
        shieldsText.text = "SHIELD : " + enemy.shields;
    }
}
