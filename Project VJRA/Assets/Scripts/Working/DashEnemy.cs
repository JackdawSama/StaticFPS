using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashEnemy : MonoBehaviour
{
    [SerializeField] float enemyHP;
    [SerializeField] float enemyDMG;
    [SerializeField] PlayerController player;

    public float detectionRadius;
    public MeshRenderer meshRenderer;
    bool isDetected;

    NavMeshAgent agent;

    float distance;

    public enum EnemyState
    {
        idle,
        roam,
        detected,
        reposition,
        dash,
        dead
    }

    [SerializeField] public EnemyState currentState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StateHandler()
    {
        switch (currentState)
        {
            case EnemyState.idle:
            //IDLE STATE. Randomise points within a radius and have enemy roam it.
            break;

            case EnemyState.detected:
            //IF player is wihtin a radius have the enemy detect the player and go into attack/dash mode
            //Future iterations change radius around player to vision cone
            break;

            case EnemyState.dash:
            //Take the player's lasy known position at the time of state change and dash to that point at high speeds.
            break;

            case EnemyState.reposition:
            //post dash have the enmy choose arandom point in a vicinity roam to it and do a dash again.
            //Do this till enemy dead or till player is out of attack radius
            break;

            case EnemyState.dead:
            //a death state for the enemy.
            break;
            
            default:
            break;
        }
    }

    public void TakeDamage(float damage)
    {
        enemyHP -= damage;
        
        if(enemyHP <= 0)
        {
            enemyHP = 0;

            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            if(player != null)
            {
                Debug.Log("Took DMG");
                player.HandlePlayerDamage(enemyDMG);
                Die();
                return;
            }
        }
    }
}
