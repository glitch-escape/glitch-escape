using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractableTank : MonoBehaviour, IPlayerInteractable
{
    public GameObject playerInTank;
    public Transform floatTextArea;
    public FloatingText floatingText;
    public string interactMessage = "[Extract]";

    public void OnInteract(Player player)
    {
        playerInTank.SetActive(false);
        floatingText.DisableText(floatTextArea);
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        if (playerInTank.activeInHierarchy)
        {
            floatingText.EnableText(floatTextArea, interactMessage);
        }
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }
}
