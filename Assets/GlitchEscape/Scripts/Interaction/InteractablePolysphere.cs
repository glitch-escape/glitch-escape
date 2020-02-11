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
    private ObjectiveController objectiveController;
    private bool _pickedUp = false;
    public bool pickedUp => _pickedUp;

    private Color _color;
    public Color color { get { return _color; } set { _color = value; } }

    void Awake()
    {
        objectiveController = ObjectiveController.instance;
        floatingText = FloatingTextController.instance;
    }

    public void OnInteract(Player player)
    {
        // if ... uhh, it's temporary, ignore it
        if ((objectiveController.multiPickUp || (!objectiveController.multiPickUp && !objectiveController.keyInHand)) && !_pickedUp)
        {
            objectiveController.PickUpKey();
            this.transform.parent.gameObject.SetActive(false);
            floatingText.DisableText(floatTextArea);
            _pickedUp = true;
            Debug.Log(_color);
            objectiveController.ShowKey(_color);
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
}
