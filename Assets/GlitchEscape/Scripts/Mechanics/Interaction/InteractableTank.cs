using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractableTank : MonoBehaviour, IActiveInteract
{
    public GameObject assignedKey;
    public GameObject playerInTank;
    public Transform floatTextArea;

    public enum ObjectiveColor { None, Red, Yellow, Green, Blue, Cyan, Purple }
    [Tooltip("corresponding to Polysphere, color is unique in the same object")]
    public ObjectiveColor objective = ObjectiveColor.None;
    public string interactMessage = "[Free hostage]";

    private FloatingTextController floatingText;
    private InteractablePolysphere interactablePolysphere;
    private ObjectiveController objectiveController;
    private InteractablePortal interactablePortal;
    private Color color = Color.grey;
    private const int MAX_TANK = 6;

    void Awake()
    {
        objectiveController = ObjectiveController.instance;
        interactablePortal = InteractablePortal.instance;

        // initialize and error check
        floatingText = FloatingTextController.instance;
        interactablePolysphere = assignedKey.GetComponentInChildren<InteractablePolysphere>();
        if (objective == ObjectiveColor.None)
        {
            Debug.LogError("Missing objective color on: " + this.transform.parent.name);
        }
        InteractableTank[] interactableTanks = FindObjectsOfType(typeof(InteractableTank)) as InteractableTank[];
        // Debug.Log("Found " + interactableTanks.Length + " instances with this script attached");
        int counter = 0;
        foreach (InteractableTank interactableTank in interactableTanks)
        {
            if (interactableTank.objective == this.objective)
            {
                counter++;
            }
            if (counter > 1)
            {
                Debug.LogError("Duplicated color on: " + interactableTank.transform.parent.name);
            }
        }
        // set color corresponding to polysphere
        switch (objective)
        {
            case ObjectiveColor.None:
                color = Color.grey;
                break;
            case ObjectiveColor.Red:
                color = Color.red;
                break;
            case ObjectiveColor.Yellow:
                color = Color.yellow;
                break;
            case ObjectiveColor.Green:
                color = Color.green;
                break;
            case ObjectiveColor.Blue:
                color = Color.blue;
                break;
            case ObjectiveColor.Cyan:
                color = Color.cyan;
                break;
            case ObjectiveColor.Purple:
                color = Color.magenta;
                break;
            default:
                break;
        }
        Renderer renderer = this.transform.parent.GetComponent<Renderer>();
        renderer.materials[0].SetColor("_EmissionColor", color);
        renderer = assignedKey.GetComponent<Renderer>();
        renderer.material.SetColor("_BaseColor", color);
        interactablePolysphere.color = this.color;
    }

    public void OnInteract(Player player)
    {
        // only apply interact when not yet picked up and is the last approached object.
        if (playerInTank.activeInHierarchy && interactablePolysphere.pickedUp && floatingText.IsCurrentTarget(floatTextArea))
        {
            playerInTank.SetActive(false);
            floatingText.DisableText(floatTextArea);
            objectiveController.CountUp();
            objectiveController.UpdateKey(this.color, false);
            if (objectiveController.objectiveCounter >= MAX_TANK)
            {
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

    public bool isInteractive => true;
    public void OnSelected(Player player) {
    }

    public void OnDeselected(Player player) {
    }
}
