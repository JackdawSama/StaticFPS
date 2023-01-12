using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //Build Enemy Critter using this. 
    //Detect Player
    //Charge at Player
    //Deal damage with contact
    //Die on contact

    //Create Spawner for this

    public PlayerController player;
    public float detectionRadius;
    public MeshRenderer meshRenderer;
    bool isDetected;

    [SerializeField] float critterHP = 2f;
    [SerializeField] float critterDamage = 5f;

    NavMeshAgent agent;

    float distance;

    public enum EnemyState
    {
        idle,
        detect,
        attack,
        dead
    }

    [SerializeField] public EnemyState currentState;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.idle;
        meshRenderer.material.color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        StateHandler();
    }

    private void StateHandler()
    {
        switch(currentState)
        {
            case EnemyState.idle:
            meshRenderer.material.color = Color.gray;
            isDetected = false;
            if(distance <= detectionRadius)
            {
                currentState = EnemyState.detect;
                isDetected = true;
            }
            break;
            case EnemyState.detect:
            if(isDetected)
            {
                meshRenderer.material.color = Color.red;
                currentState = EnemyState.attack;

            }
            break;
            case EnemyState.attack:
            if(distance <= detectionRadius && isDetected == true)
            {
                MoveEnemytoTarget();
            }
            else if(distance >= detectionRadius)
            {
                isDetected = false;
                currentState = EnemyState.idle;
            }
            break;
        }
    }

    void MoveEnemytoTarget()
    {
        agent.SetDestination(player.transform.position);
    }

    public void TakeDamage(float damage)
    {
        critterHP -= damage;
        
        if(critterHP <= 0)
        {
            critterHP = 0;

            Die();
        }
    }

    public void Die()
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
                player.HandlePlayerDamage(critterDamage);
                Die();
                return;
            }
        }
    }
}
