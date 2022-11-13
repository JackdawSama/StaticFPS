using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 50f;
    [SerializeField] float shields = 50f;
    [SerializeField] float maxShields = 100f;
    [SerializeField] float shieldRegenRate = 2f;
    [SerializeField] float detectionRadius = 100f;
    [SerializeField] float fireTimer;
    [SerializeField] float fireCD;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float turnRate = 5f;
    [SerializeField] bool isTurning;
    [SerializeField] bool isTakingFire = false;
    [SerializeField] float noDamagerTimer;
    [SerializeField] float noDamagerCD;

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

        noDamagerTimer = Time.deltaTime;

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

        if(noDamagerTimer > noDamagerCD && shields <= maxShields)
        {
            isTakingFire = false;
            StartCoroutine(regenShields());
        }
    }

    public void TakeDamage(float damage)
    {
        isTakingFire = true;
        if(shields <= 0)
        {
            health -= damage;
            if(health <= 0f)
            {
                Die();
            }
        }
    }

    public void burnShields(float damage)
    {
        isTakingFire = true;
        shields -= damage;
        if(shields < 0)
        {
            shields = 0;
        }
    }

    IEnumerator regenShields()
    {
        if(shields <= 25)
        {
            while(shields <= maxShields)
            {
                if(!isTakingFire)
                {
                    shields += shieldRegenRate;
                }
                else break;
            }
            if(shields > maxShields)
            {
                shields = maxShields;
            }
        }
        yield return null;
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

    // void OnTriggerEnter(Collider collisionInfo)
    // {
    //     if(collisionInfo.gameObject.tag == "Player")
    //     {
    //         Die();
    //     }
    // }

}
