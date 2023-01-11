using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashEnemy : MonoBehaviour
{
    [SerializeField] float enemyHP;
    [SerializeField] float enemyDMG;
    [SerializeField] float enemySpeed;
    [SerializeField] float enemyDashSpeed;
    [SerializeField] PlayerController player;
    [SerializeField] Vector3 playerLastPos;

    [SerializeField] float roamRadius;
    [SerializeField] float repositionRadius;
    [SerializeField] Vector3 roamPoint;
    [SerializeField] float roamTimer;
    [SerializeField] float roamCD;
    bool isActive = false;


    public float detectionRadius;
    public MeshRenderer meshRenderer;
    bool isDetected;

    NavMeshAgent agent;
    // [SerializeField] NavMeshSurface surface;
    // NavMeshData data;
    // bool inBounds;

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
        agent = GetComponent<NavMeshAgent>();
        enemySpeed = agent.speed;
        currentState = EnemyState.idle;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.gray;

        //data = surface.navMeshData;
        setPos(roamRadius);

        agent.SetDestination(roamPoint);

        isDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        StateHandler();
    }

    void StateHandler()
    {
        switch (currentState)
        {
            case EnemyState.idle:
            //IDLE STATE. Randomise points within a radius and have enemy roam it.
            
            //Debug.Log("Entered Idle");
            //isActive = false;
            meshRenderer.material.color = Color.gray;
            roamTimer += Time.deltaTime;

            if(roamTimer > roamCD)
            {
                Debug.Log("Entered Roam Reset");
                setPos(roamRadius);
                agent.SetDestination(roamPoint);
                roamTimer = 0;
            }

            if(distance <= detectionRadius)
            {
                currentState = EnemyState.detected;
                isDetected = true;
            }
            break;

            case EnemyState.detected:
            //IF player is wihtin a radius have the enemy detect the player and go into attack/dash mode
            //Future iterations change radius around player to vision cone

            if(isDetected)
            {
                meshRenderer.material.color = Color.red;
                currentState = EnemyState.dash;
                playerLastPos = player.transform.position;
            }

            break;

            case EnemyState.dash:
            agent.speed = enemyDashSpeed;

            if(isDetected && distance <= detectionRadius)
            {
                agent.SetDestination(playerLastPos);

                if(transform.position.x == playerLastPos.x && transform.position.z == playerLastPos.z)
                {
                    playerLastPos = player.transform.position;
                }
            }
            else if(distance > detectionRadius)
            {
                isDetected = false;
                agent.speed = enemySpeed;
                currentState = EnemyState.idle;
            }
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

    Vector3 setPos(float nearRadius)
    {
        float xPos = Random.Range(-nearRadius, nearRadius);
        float zPos = Random.Range(-nearRadius, nearRadius);

        Debug.Log(" X: " + xPos);
        Debug.Log(" Z: " + zPos);

        Vector3 roamPos = new Vector3(xPos, transform.position.y, zPos);
        roamPoint = roamPos;

        Debug.Log(roamPoint);

        return roamPoint;
    }

    // bool checkBounds(Vector3 position)
    // {
    //     if(position.x <= data.sourceBounds.max.x && position.x >= data.sourceBounds.min.x && position.z <= data.sourceBounds.max.z && position.z >= data.sourceBounds.min.z)
    //     {
    //         inBounds = true;
    //     }
    //     else inBounds = false;

    //     return inBounds;
    // }

    private IEnumerator MovetoPos(Vector3 targetPos)
    {
        isActive = true;
        playerLastPos = player.transform.position;

        while(!Physics.CheckSphere(playerLastPos, 0.5f, 3))
        {
            agent.SetDestination(playerLastPos);
        }
        yield return null;

        if(currentState == EnemyState.dash)
        {
            Debug.Log("Exiting Dash");
            currentState = EnemyState.reposition;
        }
        else if(currentState == EnemyState.reposition)
        {
            currentState = EnemyState.dash;
        }
        isActive = false;
    }  

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            if(player != null)
            {
                Debug.Log("Tagged Player");
            }
        }
    }
}
