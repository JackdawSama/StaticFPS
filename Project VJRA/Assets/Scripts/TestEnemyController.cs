using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] float enemySpeed = 5f;
    //[SerializeField] float enemyRadius = 1.5f;
    [SerializeField] LayerMask wallMask;
    [SerializeField] bool hitWall;

    [SerializeField] GameObject bulletObject;
    [SerializeField] float fireCD;

    [SerializeField]GameObject targetPlayer;
    float fireTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        moveEnemy();
        Shoot();
    }

    private void Shoot()
    {
        if(fireTimer >= fireCD)
        {
            Instantiate(bulletObject, transform.localPosition, transform.localRotation);
            fireTimer = 0;
        }
    }

    private void moveEnemy()
    {
        transform.localPosition = transform.localPosition + new Vector3(enemySpeed, 0, 0) * Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0f)
        {
            Die();
        }
    }

    void Die() 
    {
        //update player score +100
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Wall")
        {
            enemySpeed = - enemySpeed;
        }
    }
}
