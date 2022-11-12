using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 12.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += gameObject.transform.forward * projectileSpeed * Time.deltaTime;
        
    }

    void deleteBullet()
    {
        GameObject.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player")
        {
            //minus score from player total
            deleteBullet();
            return;
        }
        if(collisionInfo.gameObject.tag == "Wall")
        {
            deleteBullet();
        }

    }
}
