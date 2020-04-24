using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractablePolysphere : MonoBehaviour, IActiveInteract
{
    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";

    private FloatingTextController floatingText;
    private bool _pickedUp = false;
    public bool pickedUp => _pickedUp;

    void Awake()
    {
        floatingText = FloatingTextController.instance;
    }

    public void OnInteract(Player player)
    {
        // if ... uhh, it's temporary, ignore it
        if (!_pickedUp)
        {
            player.shardcomp.value += 1;
            this.transform.parent.gameObject.SetActive(false);
            floatingText.DisableText(floatTextArea);
            _pickedUp = true;
        }

    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        floatingText.EnableText(floatTextArea, interactMessage);
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }

    public bool isInteractive => true;
    public void OnSelected(Player player) {
    }

    public void OnDeselected(Player player) {
    }
}
