using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 50f;

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
        agent.SetDestination(player.transform.position);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        //Debug.Log("-10f");
        if(health <= 0f)
        {
            Die();
        }
    }

    void Die() 
    {
        Destroy(gameObject);
    }
}