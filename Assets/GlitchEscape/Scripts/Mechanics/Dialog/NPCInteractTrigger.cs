using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaTextTrigger))]
public class NPCInteractTrigger : MonoBehaviour
{
    
    public PlayerDialogController dialogManager;
    public AreaTextTrigger inputText;
    public string speakerName;

    //public Dialog dialogManager;
   // public Transform floatTextArea;
   // public int eventNumber = 0;

    private bool eventTriggered = false;

    void Start() {
        if(!dialogManager) dialogManager = FindObjectOfType<PlayerDialogController>();
        if(!inputText) inputText = GetComponent<AreaTextTrigger>();
    }

    void OnTriggerEnter(Collider other)
    {
      //  print(gameObject.transform.parent.gameObject.name);
      //  print(other.transform.parent.gameObject.transform.parent.gameObject.name);

        if (other.tag == "Player")
        {
           dialogManager.SetSpeaker(speakerName);
           inputText.OnPlayerEnterInteractionRadius(null);
           /*
            dialogManager.CheckPlayerNearBy(true, eventNumber);
            if (!eventTriggered)
            {
                dialogManager.SwitchDialogEvent(eventNumber, floatTextArea);
                eventTriggered = true;
            }
            */
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Player")
        {
           if(dialogManager.PreventMovement()) {
               inputText.OnPlayerExitInteractionRadius(null);
           }
           else {
                inputText.OnPlayerEnterInteractionRadius(null);
           }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.SetSpeaker(null);
            inputText.OnPlayerExitInteractionRadius(null);
            //dialogManager.CheckPlayerNearBy(false, eventNumber);
        }
    }
}
