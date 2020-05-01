using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [RequireComponent(typeof(OldEnemyController))]
[RequireComponent(typeof(NavMeshAgent))] 
[RequireComponent(typeof(Animator))] 
public class OldEnemy : MonoBehaviour {

    [HideInInspector]
    public OldEnemyController controller;
    private Player player;

    #region EnemyProperties
    public NavMeshAgent navMeshAgent {
        get {
            if (m_agent) return m_agent;
            m_agent = GetComponent<NavMeshAgent>();
            if (!m_agent) { Debug.LogError("Enemy missing Rigidbody!"); }
            return m_agent;
        }
    }
    private NavMeshAgent m_agent = null;

    public Animator animator {
        get {
            if (m_animator) return m_animator;
            m_animator = GetComponent<Animator>();
            if (!m_animator) { Debug.LogError(("Enemy missing Animator!")); }
            return m_animator;
        }
    }
    private Animator m_animator;
    #endregion

    /*
    private enum State { GUARD, PATROL, WANDER, CHASE, RETURN,
                         STRIKE, CHARGE, RETREAT, SHOOT }
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
    */

    // REMOVE IF PATROL IS UNWANTED
    #region Old Code to Let Enemy Prefab Move Again
    public Transform[] patrolPoints;
    public float ptLeniency = 1.05f;
    private Vector3 origin;
    private int curDest;
    private bool isReturnTrip;

    private void Start() {
        if (patrolPoints.Length > 0) {
            m_agent.SetDestination(patrolPoints[curDest].position);
        }
    }

    private void Update() {
        // Update destination point if needed
        if (patrolPoints.Length > 0 && Vector3.Distance(transform.position, m_agent.destination) <= ptLeniency) {
            if (isReturnTrip) {
                curDest -= 1;
                if (curDest < 0) {
                    isReturnTrip = false;
                    curDest += 1;
                }
            }
            else {
                curDest += 1;
                if (curDest >= patrolPoints.Length) {
                    isReturnTrip = true;
                    curDest -= 1;
                }
            }

            m_agent.SetDestination(patrolPoints[curDest].position);
        }
    }
    #endregion


    void OnEnable() {
        player = Enforcements.GetSingleComponentInScene<Player>(this);
    }
    // public void SetupControllerComponent(OldEnemyController controller) {
    //     this.controller = controller;
    //     player = controller.player;
    // }

    #region UnityUpdateAndAwake
    void Awake() {
        // Set properties
        m_agent = navMeshAgent;
        m_animator = animator;
        // m_animator.SetFloat("walkSpeed", m_agent.speed);
    }
    #endregion

    public void SetNavDest(Vector3 position) {
        m_agent.SetDestination(position);
    }

    // REMOVE LATER THIS IS TO AVOID ERROR MSG
    public void TakeDamage(float temp) { }

}
