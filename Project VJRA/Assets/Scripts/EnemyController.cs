using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 50f;
    [SerializeField] float shields = 50f;
    [SerializeField] float detectionRadius = 100f;
    [SerializeField] float fireTimer;
    [SerializeField] float fireCD;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float turnRate = 5f;
    [SerializeField] bool isTurning;

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

        fireTimer += Time.deltaTime;

        if(distance <= detectionRadius)
        {
            agent.SetDestination(player.transform.position);

            if(distance <= agent.stoppingDistance)
            {
                isTurning = true;
                FaceTarget();
                if(!isTurning)
                {
                    Fire();
                }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnRate);
        isTurning = false;
    }

    void Die() 
    {
        Destroy(gameObject);
    }

    void Fire()
    {
        if(fireTimer >= fireCD)
        {
            Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
            fireTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player")
        {
            Die();
        }
    }

}
