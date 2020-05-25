using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractTrigger : MonoBehaviour
{
    
    public PlayerDialogController dialogManager;
    public string speakerName;

    //public Dialog dialogManager;
    public Transform floatTextArea;
   // public int eventNumber = 0;

    private bool eventTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
           print("they ehere");
           dialogManager.SetSpeaker(speakerName);

            // NOTES:
            // diable movement
            // make x to interact appear on screen

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
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.SetSpeaker(null);
            //dialogManager.CheckPlayerNearBy(false, eventNumber);
        }
    }
}
