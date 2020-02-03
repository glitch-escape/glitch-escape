using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractTrigger : MonoBehaviour
{
    public Dialog dialogManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.changeDialogType(2);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.changeDialogType(1);
        }
    }
}
