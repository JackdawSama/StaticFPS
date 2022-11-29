using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //HEALTH & SHIELD VARIABLES
    [SerializeField] public float health = 50f;
    [SerializeField] public float shields = 50f;
    [SerializeField] float maxShields = 100f;
    [SerializeField] float shieldRegenRate = 2f;
    [SerializeField] GameObject Shields;
    [SerializeField] bool shieldsActive = true;
    //SECTION END

    //AWARNESS VARIABLES
    [SerializeField] float detectionRadius = 100f;
    [SerializeField] bool isTurning;
    [SerializeField] bool isTakingFire;
    [SerializeField] float noDamagerTimer;
    [SerializeField] float noDamagerCD;
    //SECTION END

    //ATTACK VARIABLES
    [SerializeField] float fireTimer;
    [SerializeField] float fireCD;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    //SECTION END
    
    //ENEMY VARIABLES
    [SerializeField] float turnRate = 5f;
    NavMeshAgent agent;
    public GameObject player;
    [SerializeField] float safeDistance;
    float distance;

    [SerializeField] MeshRenderer meshRenderer;
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;

    [SerializeField] float moveToTimer;
    [SerializeField] float maxMoveTimer = 1f;
    [SerializeField] float closeRangeCheckDist = 1f;


    //SECTION END

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);

        fireTimer += Time.deltaTime;

        noDamagerTimer += Time.deltaTime;

        moveToTimer += Time.deltaTime;

        if(distance <= detectionRadius && moveToTimer >= maxMoveTimer)
        {
            agent.SetDestination(player.transform.position);

            moveToTimer = 0;

            isTurning = true;
            FaceTarget();
            if(!isTurning)
            {
                Fire();
            }
        }
        
        if(noDamagerTimer > noDamagerCD)
        {
            isTakingFire = false;
            regenShields();
        }

        if(shields <= 0)
        {
            Shields.SetActive(false);
            shieldsActive = false;
        }
        else if(shields > 0)
        {
            Shields.SetActive(true);
            shieldsActive = true;
        }

        BlinkOnDamage();
    }

    public void TakeDamage(float healthDamage, float shieldDamage)
    {
        isTakingFire = true;
        noDamagerTimer = 0;
        
        if(shieldsActive)
        {
            shields -= shieldDamage;
        }
        else if(!shieldsActive)
        {
            shields = 0;

            health -= healthDamage;
        }

        if(health <= 0)
        {
            health = 0;

            Die();
        }

        blinkTimer = blinkDuration;
    }

    public void burnShields(float damage)
    {
        isTakingFire = true;
        noDamagerTimer = 0;
        shields -= damage;
        if(shields < 0)
        {
            shields = 0;
        }
    }

    void regenShields()
    {
        if(shields <= maxShields && !isTakingFire)
        {
            shields += Time.deltaTime * shieldRegenRate;
            
            if(shields > maxShields)
            {
                shields = maxShields;
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnRate);
        isTurning = false;
    }

    void putDistance()
    {
        if(distance <= safeDistance)
        {
            //set enemy to move to a safe distance from the player
        }
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

    void BlinkOnDamage()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer/blinkDuration);
        float intensity = lerp * blinkIntensity + 1f;
        meshRenderer.material.color = Color.red * intensity;
    }

}
