using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNearByTrigger : MonoBehaviour
{
    public Dialog dialogManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.changeDialogType(1);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.changeDialogType(0);
        }
    }
}
