using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 12.5f;
    [SerializeField] float damage = 10f;
    [SerializeField] float lifeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;

        lifeTime = Time.deltaTime;
        
        if(lifeTime > 2f)
        {
            deleteBullet();
        }
    }

    void deleteBullet()
    {
        GameObject.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            //minus score from player total
            deleteBullet();
            return;
        }
        if(collider.gameObject.tag == "Wall")
        {
            deleteBullet();
        }
        if(collider.gameObject.tag == "Enemy")
        {
            collider.gameObject.GetComponent<EnemyController>().burnShields(damage);
            Debug.Log("Shields damaged");
            deleteBullet();
        }

    }
}
