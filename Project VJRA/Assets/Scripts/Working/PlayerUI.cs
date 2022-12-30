using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        //player = GetComponent<PlayerController>();
        setUI();
    }

    // Update is called once per frame
    void Update()
    {
        setUI();
    }

    void setUI()
    {
        healthText.text = "HP : " + player.playerHP;
    }
}
