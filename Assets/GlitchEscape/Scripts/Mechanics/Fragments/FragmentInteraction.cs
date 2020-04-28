using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class FragmentInteraction : MonoBehaviour, IActiveInteract
{
    public Transform floatTextArea;
    public string interactMessage = "[Collect Shard]";

    //private FloatingTextController floatingText;
    private bool _pickedUp = false;
    public bool isInteractive => true;
    void Start()
    {
    }

    public void OnInteract(Player player)
    {
        if (!_pickedUp)
        {
            //increase player fragment count?
            this.gameObject.SetActive(false);
            //floatingText.DisableText(floatTextArea);
            _pickedUp = true;
        }
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
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

    // Update is called once per frame
    void Update()
    {
    }
}
