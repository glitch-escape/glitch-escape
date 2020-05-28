using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentPickup : AInteractiveObject
{
    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";

    //private FloatingTextController floatingText;
    public bool fragmentIsPickedUp = false;
    private bool _pickedUp = false;
    public GameObject fragmentUI;

    public override void OnFocusChanged(bool focused) {}

    public override void OnInteract(Player player)
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

    // Update is called once per frame
    void Update()
    {
    }
}
