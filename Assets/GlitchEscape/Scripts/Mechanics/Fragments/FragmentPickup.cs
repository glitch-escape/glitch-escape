using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class FragmentPickup : MonoBehaviour, IActiveInteract
{
    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";

    //private FloatingTextController floatingText;
    public bool fragmentIsPickedUp = false;
    private bool _pickedUp = false;
    public bool isInteractive => true;

    public GameObject fragmentUI;

    void Start()
    {
    }

    public void Awake()
    {

    }

    public void OnInteract(Player player)
    {
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        if (!_pickedUp)
        {
            //increase player fragment count?
            this.gameObject.SetActive(false);
            //floatingText.DisableText(floatTextArea);
            _pickedUp = true;
            player.GetComponent<FragmentPickupManager>()?.PickUpFragment(this);
            //give some sort of notification that the fragment was picked up

        }
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
    }

    public void OnSelected(Player player)
    {
    }

    public void OnDeselected(Player player)
    {
    }

    public void FragmentPickupCaller(PlayerEvent.Type eventType)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
