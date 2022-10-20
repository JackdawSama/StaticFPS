using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followPlayerPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = followPlayerPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followPlayerPos.position;
    }
}
