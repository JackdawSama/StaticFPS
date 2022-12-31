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
            // Debug.Log("In state Idle");
            if(distance <= detectionRadius)
            {
                currentState = EnemyState.detect;
                isDetected = true;
            }
            break;
            case EnemyState.detect:
            // Debug.Log("In state Detected");
            if(isDetected)
            {
                meshRenderer.material.color = Color.red;
                currentState = EnemyState.attack;

            }
            break;
            case EnemyState.attack:
            // Debug.Log("In state Attack");
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
            default:
            currentState = EnemyState.idle;
            break;
        }
    }

    void MoveEnemytoTarget()
    {
        agent.SetDestination(player.transform.position);
    }
}
