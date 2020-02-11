using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractableTank : MonoBehaviour, IPlayerInteractable
{
    public GameObject assignedKey;
    public GameObject playerInTank;
    public Transform floatTextArea;

    public enum ObjectiveColor { None, Red, Yellow, Green, Blue, Cyan, Purple }
    [Tooltip("corresponding to Polysphere, color is unique in the same object")]
    public ObjectiveColor objective = ObjectiveColor.None;
    public string interactMessage = "[Extract]";

    private FloatingTextController floatingText;
    private InteractablePolysphere interactablePolysphere;
    private Color color = Color.grey;

    void Awake()
    {
        // initialize and error check
        floatingText = FloatingTextController.instance;
        interactablePolysphere = assignedKey.GetComponentInChildren<InteractablePolysphere>();
        if (objective == ObjectiveColor.None)
        {
            Debug.LogError("Missing objective color on: " + this.transform.parent.name);
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
        Renderer rend = this.transform.parent.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", color);
        Debug.Log(this.transform.parent.name + " is " + color);
        rend = assignedKey.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", color);

    }

    public void OnInteract(Player player)
    {
        if (playerInTank.activeInHierarchy && interactablePolysphere.pickedUp && floatingText.isCurrentTarget(floatTextArea))
        {
            playerInTank.SetActive(false);
            floatingText.DisableText(floatTextArea);
        }
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
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
