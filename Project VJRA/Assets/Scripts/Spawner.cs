using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour

{
    [SerializeField] List<GameObject> KWList;
    [SerializeField] GameObject[] enemies;
    [SerializeField] Transform[] enemySpawnPoints;
    [SerializeField] GameObject enemyObject;
    [SerializeField] float wallSpawnCD;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
