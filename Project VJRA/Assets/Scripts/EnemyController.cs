using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 50f;
    [SerializeField] float shields = 50f;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float closestApproach;
    [SerializeField] float farthestApproach;
    [SerializeField] float lowShieldsclosestApproach;
    [SerializeField] float lowShieldsfarthestApproach;

    [SerializeField] float lowShieldsOffsetClose;
    [SerializeField] float lowShieldsOffsetFar;

    private int randomUnit;
    [SerializeField] float behaviourTimer;

    Transform target;

    enum AIState
    {
        idle,
        move,
        attack
    }
    [SerializeField] AIState state;

    NavMeshAgent agent;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if(distance <= detectionRadius)
        {
            agent.SetDestination(player.transform.position);

            if(distance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0f)
        {
            Die();
        }
    }

    public void burnShields(float damage)
    {
        shields -= damage;
    }

    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
    }

    void Die() 
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player")
        {
            Die();
        }
    }

}
