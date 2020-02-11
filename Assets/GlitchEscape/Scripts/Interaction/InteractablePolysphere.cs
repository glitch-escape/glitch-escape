using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractablePolysphere : MonoBehaviour, IPlayerInteractable
{
    public Transform floatTextArea;
    public string interactMessage = "[Pick up]";

    private FloatingTextController floatingText;
    private InteractableTank interactableTank;

    private bool _pickedUp = false;
    public bool pickedUp => _pickedUp;

    void Awake()
    {
        floatingText = FloatingTextController.instance;
    }

    public void OnInteract(Player player)
    {
        this.transform.parent.gameObject.SetActive(false);
        floatingText.DisableText(floatTextArea);
        _pickedUp = true;
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        floatingText.EnableText(floatTextArea, interactMessage);
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }
}
