using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(AreaTextTrigger))]
public class NPCInteractTrigger : MonoBehaviour
{
    public Player player;
    public PlayerDialogController dialogManager;
    public AreaTextTrigger inputText;
    public string speakerName;
    public string postOrbSpeaker; // keep empty if it's the same
    public bool isTank;
    public float turnSpeed, wanderRange;
    public float minIdle, maxIdle; // range for npc standing still
    public Virtue virtueType;

    private bool hasPostOrbSpeaker => postOrbSpeaker != "";
    private Transform theNPC => transform.parent;
    private Animator _anim;
    private NavMeshAgent m_Agent;
    private bool playerDetected;
    private float idleTime;
    private float nextWaitTime;
    //sound source for completion sound
    [InjectComponent] public AudioSource soundSource;
    public AudioClip taskComplete;

    private bool eventTriggered = false;
    private bool hasCollectedOrb
        => player.fragments.IsVirtueCompleted(Virtue.Courage) 
            || player.fragments.IsVirtueCompleted(Virtue.Humanity)
            || player.fragments.IsVirtueCompleted(Virtue.Transcendence);

    void Start() {
        if(!dialogManager) dialogManager = FindObjectOfType<PlayerDialogController>();
        if(!inputText) inputText = GetComponent<AreaTextTrigger>();
        player = FindObjectOfType<Player>();
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
        if(hasPostOrbSpeaker && hasCollectedOrb) {
            //sound clip
            if (taskComplete != null)
            {
                soundSource.PlayOneShot(taskComplete);
            }
            dialogManager.SetSpeaker(postOrbSpeaker);
        }
        else { dialogManager.SetSpeaker(speakerName); }

        inputText.OnPlayerEnterInteractionRadius(null);
        if(!isTank) { Halt(); }
        playerDetected = true;
    }

    void OnTriggerStay(Collider other) {
        if(dialogManager.PreventMovement()) {
            inputText.OnPlayerExitInteractionRadius(null);
        }
        else {
           inputText.OnPlayerEnterInteractionRadius(null);
        }
        // Look at player
        if(!isTank) {
            Vector3 targetDirection = other.transform.position - theNPC.position;
            float singleStep = turnSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(theNPC.forward, targetDirection, singleStep, 0.0f);
            theNPC.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void OnTriggerExit(Collider other) {
        dialogManager.SetSpeaker(null);
        inputText.OnPlayerExitInteractionRadius(null);
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
