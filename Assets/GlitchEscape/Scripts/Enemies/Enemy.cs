using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum State { GUARD, PATROL, WANDER, CHASE, RETURN }
    private State curState;

    public Transform player;
    public Transform[] patrolPoints;
    public float detectRad, wanderRange;
    NavMeshAgent m_Agent;

    private Vector3 origin;
    private int curDest;
    private bool isReturnTrip;

    void Start()
    {
        curState = State.GUARD;
        m_Agent = GetComponent<NavMeshAgent>();
        origin = gameObject.transform.position;

        if(patrolPoints.Length > 0) 
        {
            curState = State.PATROL;
            m_Agent.SetDestination(patrolPoints[curDest].position);
        }
        else
        {
            curState = State.GUARD;
            // Set patrol point to origin
            GameObject originObj = new GameObject();
            originObj.transform.position = origin;
            patrolPoints = new Transform[1];
            patrolPoints[0] = originObj.transform;
        }
    }

    void Update()
    {
        Behave();
        Debug.Log(curState);
    }

    // Allows the enemy to act however it wants
    private void Behave() 
    {
        switch (curState)
        {
            case State.GUARD:   Guard();    break;
            case State.PATROL:  Patrol();   break;
            case State.WANDER:  Wander();   break;
            case State.CHASE:   Chase();    break;
            case State.RETURN:  Return();   break;
            default:                        break;
        }
    }

    // Determine if the player is in the detection radius
    private bool DetectPlayer() 
    {
        return detectRad >= Vector3.Distance(player.position, transform.position);
    }

    // Handles enemy actions when in idle state
    private void Guard() 
    {
        if(DetectPlayer())
        {
            curState = State.CHASE;
        }
    }

    // Handles enemy actions when in patrol state
    private void Patrol() 
    {
        // Update destination point if needed
        if(m_Agent.destination.x == transform.position.x 
            && m_Agent.destination.z == transform.position.z)
        {
            if(isReturnTrip)
            {
                curDest -= 1;
                if(curDest < 0)
                {
                    isReturnTrip = false;
                    curDest += 1;
                }
            }
            else
            {
                curDest += 1;
                if(curDest >= patrolPoints.Length)
                {
                    isReturnTrip = true;
                    curDest -= 1;
                }
            }

            m_Agent.SetDestination(patrolPoints[curDest].position);
        }

        if(DetectPlayer())
        {
            curState = State.CHASE;
        }
    }

    // Handles how the enemy will chase the player
    private void Chase()
    {
        // Update destination to current player position
        m_Agent.SetDestination(player.position);

        // Sample conditional to reset enemy state
        if(!DetectPlayer())
        {
            curState = State.RETURN;
        }
    }

    // Move enemy back to orignal position
    private void Return()
    {
        if(m_Agent.destination.x == transform.position.x 
            && m_Agent.destination.z == transform.position.z)
        {
            m_Agent.SetDestination(patrolPoints[curDest].position);
        }

        if(transform.position.x == patrolPoints[curDest].position.x
            && transform.position.z == patrolPoints[curDest].position.z)
        {
            if(patrolPoints.Length > 1)
            {
                curState = State.PATROL;
            }
            else
            {
                curState = State.GUARD;
            }
            
        }
    }

    // Make the enemy move randomly thoughout the map until a player is detected
    private void Wander()
    {
        // Update destination point if needed
        if(m_Agent.destination.x == transform.position.x 
            && m_Agent.destination.z == transform.position.z)
        {
            bool gotPoint = false;
            while(true)
            {
                Vector3 randomDest = transform.position + Random.insideUnitSphere * wanderRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDest, out hit, 2.0f, NavMesh.AllAreas)) {
                    m_Agent.SetDestination(hit.position);
                    break;
                }
            }
        }

        if(DetectPlayer())
        {
            curState = State.CHASE;
        }
    }

}
