using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float dasherTimer;
    [SerializeField] float projectileTimer;
    float globalTimer;
    public int timer;
    [SerializeField] public int timer1;
    [SerializeField] public int timer2;
    [SerializeField] int floasterSpawnCD;
    [SerializeField] int dasherSpawnCD;
    [SerializeField] int projectileSpawnCD;
    //[SerializeField] PlayerController player;

    // variables for timer
    // timer for enemy type 1
    // timer for enemy type 2

    [SerializeField] GameObject floaterType;
    [SerializeField] GameObject dasherType;
    [SerializeField] GameObject projectileType;

    // List for Spawn Points
    [SerializeField] GameObject[] spawnPoint;
    [SerializeField] public PlayerController player;
    int spawingPoint;

    // List for type 1 enemy
    // List for type 2 enemy

    // Start is called before the first frame update
    void Start()
    {
        spawingPoint = 0;
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!player.isAlive)
        {
            Debug.Log("Player is dead");
            return;
        }

        globalTimer += Time.deltaTime;
        dasherTimer += Time.deltaTime;
        projectileTimer += Time.deltaTime;

        timer = Mathf.RoundToInt(globalTimer);
        timer1 = Mathf.RoundToInt(dasherTimer);
        timer2 = Mathf.RoundToInt(projectileTimer);

        if(dasherTimer > dasherSpawnCD)
        {
            Instantiate(dasherType, spawnPoint[spawingPoint].transform.position, transform.rotation);
            spawingPoint++;
            Debug.Log("Spawing Point: " + ".Dasher Spawn");
            if(spawingPoint > spawnPoint.Length)
            {
                spawingPoint = 0;
            }
            dasherTimer = 0;

        }

        else if(projectileTimer > projectileSpawnCD)
        {
            Instantiate(projectileType,  spawnPoint[spawingPoint].transform.position, transform.rotation);
            spawingPoint++;
            Debug.Log("Spawing Point: " + ".Projectile Spawn");
            if(spawingPoint > 0)
            {
                spawingPoint = 0;
            }
            projectileTimer = 0;
        }    
    }
}
