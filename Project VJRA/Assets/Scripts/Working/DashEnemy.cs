using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class DashEnemy : MonoBehaviour
{
    [SerializeField] float enemyHP;
    [SerializeField] float enemyDMG;
    [SerializeField] float enemySpeed;
    [SerializeField] float enemyDashSpeed;
    [SerializeField] public PlayerController player;
    [SerializeField] Vector3 playerLastPos;
    EnemySpawner spawner;

    [SerializeField] float roamRadius;
    [SerializeField] float repositionRadius;
    [SerializeField] Vector3 roamPoint;
    [SerializeField] float roamTimer;
    [SerializeField] float roamCD;

    //POINTS VARIABLES
    [SerializeField] NewWeaponSystem weaponSystem;
    [SerializeField] Points points;
    [SerializeField] float killPoints;
    [SerializeField] float ggBonusPoints;


    public float detectionRadius;
    public MeshRenderer meshRenderer;
    bool isDetected;
    bool isRepositioning = false;

    NavMeshAgent agent;
    NavMeshHit hit;

    public float blinkTimer;
    public float blinkDuration;
    public float blinkIntensity;

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

    int firstno;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemySpeed = agent.speed;
        currentState = EnemyState.idle;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.gray;
        player = FindObjectOfType<PlayerController>();
        weaponSystem = FindObjectOfType<NewWeaponSystem>();
        points = FindObjectOfType<Points>();

        if(player == null)
        {
            Debug.Log("Object not found");
        }

        setPos(transform.position, roamRadius);

        agent.SetDestination(roamPoint);

        isDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            distance = Vector3.Distance(player.transform.position, transform.position);
        }
        StateHandler();

        BlinkOnDamage();
    }

    void StateHandler()
    {
        switch (currentState)
        {
            case EnemyState.idle:

            meshRenderer.material.color = Color.gray;
            roamTimer += Time.deltaTime;

            if(roamTimer > roamCD)
            {
                //Debug.Log("Entered Roam Reset");
                setPos(transform.position, roamRadius);
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

            if(isDetected)
            {
                meshRenderer.material.color = Color.yellow;
                currentState = EnemyState.dash;
                playerLastPos = player.transform.position;
            }

            break;

            case EnemyState.dash:

            meshRenderer.material.color = Color.red;
            agent.speed = enemyDashSpeed;

            if(isDetected && distance <= detectionRadius)
            {
                agent.SetDestination(playerLastPos);

                if(transform.position.x == playerLastPos.x && transform.position.z == playerLastPos.z)
                {
                    isRepositioning = false;
                    currentState = EnemyState.reposition;
                }
            }
            else if(distance > detectionRadius)
            {
                isDetected = false;
                agent.speed = enemySpeed;
                currentState = EnemyState.idle;
            }
            break;

            case EnemyState.reposition:

            meshRenderer.material.color = Color.blue;
            agent.speed = enemySpeed;

            if(!isRepositioning)
            {
                isRepositioning = true;
                setPos(transform.position, repositionRadius);
            }

            if(isDetected && distance <= detectionRadius)
            {
                agent.SetDestination(roamPoint);

                if(transform.position.x == roamPoint.x && transform.position.z == roamPoint.z)
                {
                    playerLastPos = player.transform.position;
                    currentState = EnemyState.dash;
                }
            }
            else if(distance > detectionRadius)
            {
                isDetected = false;
                agent.speed = enemySpeed;
                currentState = EnemyState.idle;
            }
            break;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Enemy Taking DMG");
        enemyHP -= damage;
        
        if(enemyHP <= 0)
        {
            enemyHP = 0;

            Die();
        }

        blinkTimer = blinkDuration;
    }

    public void Die()
    {
        if(weaponSystem.enableGg)
        {
            points.KillTotal(killPoints + ggBonusPoints);
            Destroy(gameObject);
            return;
        }
        if(!weaponSystem.enableGg)
        {
            points.KillTotal(killPoints);
            Destroy(gameObject);
            return;
        }
    }

    void setPos(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);

        Vector3 roamPos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        roamPoint = roamPos;

        //return roamPoint;
    }

    void BlinkOnDamage()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer/blinkDuration);
        float intensity = lerp * blinkIntensity + 1f;
        meshRenderer.material.color = meshRenderer.material.color * intensity;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            if(player != null)
            {
                Debug.Log("Tagged Player");
                player.HandlePlayerDamage(enemyDMG);
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     // Draw a cyan wirecircle at the transform's position
    //     Handles.color = Color.red;
    //     Handles.DrawWireDisc(transform.position, Vector3.up, detectionRadius);
    // }
}
