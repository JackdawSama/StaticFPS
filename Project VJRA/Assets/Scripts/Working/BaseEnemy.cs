using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public enum State
    {
        idle,
        reposition,
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
}
