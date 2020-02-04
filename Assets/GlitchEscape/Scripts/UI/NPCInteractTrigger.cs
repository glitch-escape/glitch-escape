using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractTrigger : MonoBehaviour
{
    public Dialog dialogManager;
    public Transform floatTextArea;
    public int eventNumber = 0;

    private bool eventTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.CheckPlayerNearBy(true, eventNumber);
            if (!eventTriggered)
            {
                dialogManager.SwitchDialogEvent(eventNumber, floatTextArea);
                eventTriggered = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.CheckPlayerNearBy(false, eventNumber);
        }
    }
}
