using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentPickup : PersistentInteractiveObject<FragmentPickup.State> {
    public struct State {
        public bool collected; // = false
    }
    protected override void OnStateRestored() {
        if (state.collected) {
            gameObject.SetActive(false);
        }
    }

    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";

    //private FloatingTextController floatingText;
    public bool fragmentIsPickedUp = false;
    public GameObject fragmentUI;

    public override void OnFocusChanged(bool focused) {}

    public override void OnInteract(Player player) {
        state.collected = true;
        gameObject.SetActive(false);
        player.GetComponent<FragmentPickupManager>()?.PickUpFragment(this);
    }
}
