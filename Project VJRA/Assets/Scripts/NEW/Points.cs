using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    // Start is called before the first frame update

    public float totalPoints;

    public float timerTotal;

    float timerPoints;
    [SerializeField] float timerMultiplier;
    [SerializeField] EnemySpawner spawner;

    float killPoints;
    public float killTotal;
    void Start()
    {
        spawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        TimerTotal();
        PlayerPoints(); 
    }

    void PlayerPoints()
    {
        totalPoints = timerPoints + killPoints;
    }

    public void KillTotal(float points)
    {
        killPoints = points;
        killTotal += killPoints;
    } 

    public void TimerTotal()
    {
        timerPoints =  timerMultiplier * spawner.timer;
        timerTotal += timerPoints;
    }
}
