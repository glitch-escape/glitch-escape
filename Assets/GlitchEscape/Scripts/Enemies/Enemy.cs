﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] 
[RequireComponent(typeof(Animator))] 
public class Enemy : MonoBehaviour, IEnemyControllerComponent {

    [HideInInspector]
    public EnemyController controller;

    #region EnemyProperties
    public NavMeshAgent navMeshAgent {
        get {
            if (m_agent) return m_agent;
            m_agent = GetComponent<NavMeshAgent>();
            if (!m_agent) { Debug.LogError("Player missing Rigidbody!"); }
            return m_agent;
        }
    }
    private NavMeshAgent m_agent = null;

    public Animator animator {
        get {
            if (m_animator) return m_animator;
            m_animator = GetComponent<Animator>();
            if (!m_animator) { Debug.LogError(("Player missing Animator!")); }
            return m_animator;
        }
    }
    private Animator m_animator;
    #endregion

    private enum State { GUARD, PATROL, WANDER, CHASE, RETURN, ATTACK }
    private State curState;

    public Transform[] patrolPoints;
    public float detectRad = 20f, wanderRange = 30f;
    public float ptLeniency = 1.05f;
    
    public Material material;
    public Material[] colors;

     // Sample vars for attack
    public GameObject[] attacks;
    private Attack curAtk;

    private Vector3 origin;
    private int curDest;
    private bool isReturnTrip;
    private Player player;

    public void SetupControllerComponent(EnemyController controller) {
        this.controller = controller;
        player = controller.player;
    }

    #region UnityUpdateAndAwake
    void Awake() {
        // Set properties
        m_agent = navMeshAgent;
        m_animator = animator;

        origin = gameObject.transform.position;
        material = GetComponentInChildren<Renderer>().materials[1];
        if (!material) { Debug.LogError("Enemy.cs: Could not find material reference!"); }

        m_animator.SetFloat("walkSpeed", m_agent.speed);

        // Start in guard state if there's no patrol points
        if(patrolPoints != null) {
            curState = State.PATROL;
            m_agent.SetDestination(patrolPoints[curDest].position);
        }
        else {
            curState = State.GUARD;
            // Set patrol point to origin
            GameObject originObj = new GameObject();
            originObj.transform.position = origin;
            patrolPoints = new Transform[1];
            patrolPoints[0] = originObj.transform;
        }

        // Actually determine attacks later
        if (attacks.Length > 0) {
            curAtk = attacks[0].GetComponent<Attack>();
        }
        else {
            curAtk = null;
        }
    }

    void Update() {
        Behave();
        m_animator.SetBool("isWalking", !m_agent.isStopped);
        Debug.Log(curState);
    }
    #endregion

    // Allows the enemy to act however it wants
    private void Behave() {
        switch (curState) {
            case State.GUARD:   Guard();    break;
            case State.PATROL:  Patrol();   break;
            case State.WANDER:  Wander();   break;
            case State.CHASE:   Chase();    break;
            case State.RETURN:  Return();   break;
            case State.ATTACK:  Attack();   break;
            default:                        break;
        }
    }

    public static Color GUARD_COLOR = Color.cyan;
    public static Color PATROL_COLOR = Color.green;
    public static Color WANDER_COLOR = Color.cyan;
    public static Color CHASE_COLOR = Color.red;
    public static Color RETURN_COLOR = Color.cyan;
    public static Color ATTACK_COLOR = Color.white;

    void SetColor(Color color) {
        material.color = color;
    }

    // Determine if the player is in the detection radius
    private bool DetectPlayer() {
        return detectRad >= Vector3.Distance(player.transform.position, transform.position);
    }

    // Handles enemy actions when in idle state
    private void Guard() {
        SetColor(GUARD_COLOR);
        if(DetectPlayer()) {
            curState = State.CHASE;
        }
    }

    // Handles enemy actions when in patrol state
    private void Patrol() {
        SetColor(PATROL_COLOR);
        // Update destination point if needed
        if(Vector3.Distance(transform.position, m_agent.destination) <= ptLeniency) {
            if(isReturnTrip) {
                curDest -= 1;
                if(curDest < 0) {
                    isReturnTrip = false;
                    curDest += 1;
                }
            }
            else {
                curDest += 1;
                if(curDest >= patrolPoints.Length) {
                    isReturnTrip = true;
                    curDest -= 1;
                }
            }

            m_agent.SetDestination(patrolPoints[curDest].position);
        }

        if(DetectPlayer()) {
            curState = State.CHASE;
        }
    }

    // Handles how the enemy will chase the player
    private void Chase() {
        SetColor(CHASE_COLOR);
        // Update destination to current player position
        m_agent.SetDestination(player.transform.position);

        // Sample conditional to reset enemy state
        if(!DetectPlayer()) {
            curState = State.RETURN;
        }

        float playerDist = Vector3.Distance(player.transform.position, transform.position);
        if(curAtk != null && curAtk.distance >= playerDist) {
            curState = State.ATTACK;
            if (!curAtk.gameObject.activeSelf) {
                curAtk.gameObject.SetActive(true);
            }
            curAtk.StartAttack();
        }
    }

    // Move enemy back to orignal position
    private void Return() {
        SetColor(RETURN_COLOR);
        if(Vector3.Distance(transform.position, m_agent.destination) <= ptLeniency) {
            m_agent.SetDestination(patrolPoints[curDest].position);
        }

        if(Vector3.Distance(transform.position, patrolPoints[curDest].position) <= ptLeniency) {
            if(patrolPoints.Length > 1) {
                curState = State.PATROL;
            }
            else {
                curState = State.GUARD;
            }
            
        }

        if(DetectPlayer()) {
            curState = State.CHASE;
        }
    }

    // Make the enemy move randomly thoughout the map until a player is detected
    private void Wander() {
        SetColor(WANDER_COLOR);
        // Update destination point if needed
        if(m_agent.destination.x == transform.position.x 
            && m_agent.destination.z == transform.position.z) {
            while(true) {
                Vector3 randomDest = transform.position + Random.insideUnitSphere * wanderRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDest, out hit, 2.0f, NavMesh.AllAreas)) {
                    m_agent.SetDestination(hit.position);
                    break;
                }
            }
        }

        if(DetectPlayer()) {
            curState = State.CHASE;
        }
    }

    // Chase player once the attack is over
    private void Attack() {
        SetColor(ATTACK_COLOR);
        if(!curAtk.GetActive()) {
            curState = State.CHASE;
        }
    }

}
