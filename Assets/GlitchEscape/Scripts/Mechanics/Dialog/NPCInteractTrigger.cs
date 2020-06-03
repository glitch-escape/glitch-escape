using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(AreaTextTrigger))]
public class NPCInteractTrigger : MonoBehaviour
{
    
    public PlayerDialogController dialogManager;
    public AreaTextTrigger inputText;
    public string speakerName;
    public bool isTank;
    public float turnSpeed, wanderRange;
    public float minIdle, maxIdle; // range for npc standing still
    public Virtue virtueType;

    private Transform theNPC => transform.parent;
    private Animator _anim;
    private NavMeshAgent m_Agent;
    private bool playerDetected;
    private float idleTime;
    private float nextWaitTime;

    private bool eventTriggered = false;

    void Start() {
        if(!dialogManager) dialogManager = FindObjectOfType<PlayerDialogController>();
        if(!inputText) inputText = GetComponent<AreaTextTrigger>();
        Player player = FindObjectOfType<Player>();
        _anim = theNPC.GetComponent<Animator>();
        if(!isTank) {
            m_Agent = theNPC.GetComponent<NavMeshAgent>();
            m_Agent.SetDestination(theNPC.position);
            nextWaitTime = Random.Range(minIdle, maxIdle);
        }

        if(isTank && player.fragments.IsVirtueCompleted(virtueType))    Destroy(theNPC.gameObject);
        if(!isTank && !player.fragments.IsVirtueCompleted(virtueType))  Destroy(theNPC.gameObject);
        if(!isTank && _anim) { _anim.SetFloat("runSpeed", 0); }
    }

    void Update() {
        if(!isTank) {
            if(!playerDetected) Wander();
            _anim.SetFloat("runSpeed", m_Agent.velocity.magnitude);
        }
    }

    void OnTriggerEnter(Collider other) {
        if(!isTank) {
            dialogManager.SetSpeaker(speakerName);
            inputText.OnPlayerEnterInteractionRadius(null);
            Halt();
        }
        playerDetected = true;
    }

    void OnTriggerStay(Collider other) {
        // Look at player
        if(!isTank) {
            if(dialogManager.PreventMovement()) {
                inputText.OnPlayerExitInteractionRadius(null);
            }
            else {
                inputText.OnPlayerEnterInteractionRadius(null);
            }

            Vector3 targetDirection = other.transform.position - theNPC.position;
            float singleStep = turnSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(theNPC.forward, targetDirection, singleStep, 0.0f);
            theNPC.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void OnTriggerExit(Collider other) {
        if(!isTank) {
            dialogManager.SetSpeaker(null);
            inputText.OnPlayerExitInteractionRadius(null);
        }
        playerDetected = false;
    }

    // Super sloppy npc wander functionality (but at least we have it)
    private void Wander() {
        // Update destination point if needed
        if(Time.time - idleTime > nextWaitTime && m_Agent.destination.x == theNPC.position.x 
            && m_Agent.destination.z == theNPC.position.z) {

            while(true) {
                Vector3 randomDest = theNPC.position + Random.insideUnitSphere * wanderRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDest, out hit, 2.0f, NavMesh.AllAreas)) {
                    m_Agent.SetDestination(hit.position);
                    idleTime = Time.time;
                    nextWaitTime = Random.Range(minIdle, maxIdle);
                    break;
                }
            }
        }
    }

    private void Halt() {
        m_Agent.SetDestination(theNPC.position);
        if(_anim) { _anim.SetFloat("runSpeed", 0); }
    }

}
