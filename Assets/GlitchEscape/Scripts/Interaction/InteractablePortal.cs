﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionTrigger))]
public class InteractablePortal : MonoBehaviour, IPlayerInteractable
{
    // singleton
    private static InteractablePortal _instance = null;
    public static InteractablePortal instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<InteractablePortal>();
                if (_instance == null)
                    Debug.LogError("No InteractablePortal instance in this scene!");
            }
            return _instance;
        }
    }

    public Transform floatTextArea;
    public string interactMessage = "[Go through]";

    private FloatingTextController floatingText;
    private InteractableTank interactableTank;

    void Awake()
    {
        floatingText = FloatingTextController.instance;
    }
    void Start()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

    [Obsolete]
    public void OnInteract(Player player)
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void OnPlayerEnterInteractionRadius(Player player)
    {
        floatingText.EnableText(floatTextArea, interactMessage);
    }

    public void OnPlayerExitInteractionRadius(Player player)
    {
        floatingText.DisableText(floatTextArea);
    }

    public void OpenPortal()
    {
        this.transform.parent.gameObject.SetActive(true);
    }
}
