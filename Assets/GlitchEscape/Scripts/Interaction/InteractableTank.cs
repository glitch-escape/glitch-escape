﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractableTank : MonoBehaviour, IPlayerInteractable
{
    public GameObject assignedKey;
    public GameObject playerInTank;
    public Transform floatTextArea;
    public string interactMessage = "[Free hostage]";

    private FloatingTextController floatingText;
    private InteractablePolysphere interactablePolysphere;
    private InteractablePortal interactablePortal;
    private const int MAX_TANK = 6;

    void Awake()
    {
        interactablePortal = InteractablePortal.instance;

        // initialize and error check
        floatingText = FloatingTextController.instance;
        interactablePolysphere = assignedKey.GetComponentInChildren<InteractablePolysphere>();
        InteractableTank[] interactableTanks = FindObjectsOfType(typeof(InteractableTank)) as InteractableTank[];
    }

    public void OnInteract(Player player)
    {
        // only apply interact when not yet picked up and is the last approached object.
        if (playerInTank.activeInHierarchy && interactablePolysphere.pickedUp && floatingText.IsCurrentTarget(floatTextArea))
        {
            if(player.shardcomp.value == player.shardcomp.maximum)
            {
                playerInTank.SetActive(false);
                floatingText.DisableText(floatTextArea);
                //reset to 0 for the next tank?
                interactablePortal.OpenPortal();
            }
        }
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        // only apply enter interact when not yet picked up.
        if (playerInTank.activeInHierarchy && interactablePolysphere.pickedUp)
        {
            floatingText.EnableText(floatTextArea, interactMessage);
        }
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }
}
