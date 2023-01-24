using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{

    //ENEMY VARIABLES
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] float damage;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float engageRadius;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Vector3 interestPoint;
    //END OF ENEMY VARIABLES

    //IDLE VARIABLES
    [SerializeField] bool isIdleMoving = false;
    [SerializeField] bool isIdleLooking = false;
    [SerializeField] float idleDistance;
    //END OF IDLE VARIABLES

    //BLINK VARIABLES
    [SerializeField] float blinkTimer;
    [SerializeField] float blinkDuration;
    [SerializeField] float blinkIntensity;
    //END OF BLINK VARIABLES


    [SerializeField]float roamingRadius;
    public enum State
    {
        idle,
        reposition,
        engage,
        dodge,
        attack,
        dead

    }
    [SerializeField] public State currentState;

    void StateHandler()
    {
        switch (currentState)
        {
            case State.idle:
                //* Make enemy roam map *//
                //* Choose random point within a radius and move enemy to it *//
                //* After point selected make enemy turn towards point and then move *//
                //* When enemy is near the target point have the enemy select another point *//

                if(!isIdleMoving)
                {
                    isIdleLooking = false;
                    interestPoint = SetPointToMove(transform.position, roamingRadius);
                    isIdleMoving = true;
                    idleDistance = CalculateDistance(interestPoint);
                }
                
                if(isIdleMoving)
                {
                    agent.SetDestination(interestPoint);
                    if(idleDistance < 2)
                    {
                        interestPoint = SetPointToMove(transform.position, roamingRadius);
                    }
                }
                break;
            case State.engage:
                //* Enemy should engage with player *//
                //* In this state other enemies shouldn't be attacking player *//
                //* When transitioning to engage all other activity must cease *//
                //* engage must always lead to an attack *//

                break;
            case State.reposition:
                //* In case  enemy is within attacking radius *//
                //* Have enemy choose point within a tighter radius *//
                //* Enemy must always face player *//
                break;
            case State.dodge:
                //* If enemy is being attacked enemy must dodge at times *//
                //* Dodge must be predictable and should also be a little random *//

                break;
            case State.attack:
                //* Enemy should attack player with an attack cue *// 
                
                break;
            case State.dead:
                //* Dead enemy must be set to inactive *//
                //* Enemy variables must be reset *//
                
                break;
        }
    }

    //function to choose point within a radius
    Vector3 SetPointToMove(Vector3 center, float radius)
    {
        //function takes a center, in this case the enemy's transform.position and a radius
        //function takes a radius within which it'll choose a point

        float angle = Random.Range(0, 2f * Mathf.PI);               //chooses a random angle between 0 and 2pi

        Vector3 newPosition = center + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;
        return newPosition;                                         
    }

    //function to blink before attacking
    void Blink()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer/blinkDuration);
        float intensity = lerp * blinkIntensity + 1f;
        meshRenderer.material.color = meshRenderer.material.color * intensity;
    }

    float CalculateDistance(Vector3 point)
    {
        //function to calculate distance between enemy and point/player

        float distance = Vector3.Distance(transform.position, point);

        return distance;
    }
}
