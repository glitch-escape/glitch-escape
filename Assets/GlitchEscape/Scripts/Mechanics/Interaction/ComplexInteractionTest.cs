using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Example script to show how to implement player-interactive objects
/// </summary>
[RequireComponent(typeof(Collider))]
public class ComplexInteractionTest : AInteractiveObject {
    public TMP_Text displayText;
    public string interactMessage = "Press <interact> to interact";
    public string[] messages;
    public float messageDisplayTime = 2f;
    public bool pressInteractSkipsToNextMessage = true;

    public enum ShowMessageState {
        None,
        ShowTooltip,
        ShowMessage
    };
    public ShowMessageState messageState = ShowMessageState.None;
    private bool showMessages => messageState == ShowMessageState.ShowMessage;

    void Start() {
        if (displayText == null) {
            displayText = GetComponentInChildren<TMP_Text>(); 
        }
        if (displayText == null) {
            Debug.LogError("TestInteractable: missing display text!");
        }
        displayText.gameObject.SetActive(false);
    }
    private void OnEnable() {
        messageState = ShowMessageState.None;
        playerInInteractionRadius = false;
    }

    void ShowMessage(string message) {
        // show / hide message if message is null / non-null
        if ((message != null) != displayText.gameObject.activeSelf) {
            displayText.gameObject.SetActive(message != null);
        }
        if (message != null) {
            displayText.text = message;
        }
    }
    private int currentMessage = 0;
    private float messageDisplayStartTime = 0f;
    private bool playerInInteractionRadius = false;
    private float elapsedTime => Time.time - messageDisplayStartTime;
    
    void ShowNextMessage() {
        if (currentMessage + 1 >= messages.Length) {
            EndMessages();
        } else {                      
            ++currentMessage;
            messageDisplayStartTime = Time.time;
            ShowMessage(messages[currentMessage]);
        }
    }

    void EndMessages() {
        messageState = ShowMessageState.None;                        
        messageDisplayStartTime = Time.time;
        ShowMessage(null);
    }
    void ShowTooltip() {
        messageState = ShowMessageState.ShowTooltip;
        ShowMessage(interactMessage);
    }
    void StartShowingMessages() {
        messageState = ShowMessageState.ShowMessage;
        currentMessage = -1;
        ShowNextMessage();
    }
    void Update() {
        base.Update();
        if (showMessages && elapsedTime > messageDisplayTime) {
            ShowNextMessage();
        } else if (playerInInteractionRadius && elapsedTime > messageDisplayTime) {
            // re-display tooltip after a bit if player is still in interaction radius
            ShowTooltip();
        }
    }
    public override void OnInteract(Player player) {
        switch (messageState) {
            case ShowMessageState.ShowTooltip: StartShowingMessages(); break;
            case ShowMessageState.ShowMessage: ShowNextMessage(); break;
            case ShowMessageState.None: break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void OnFocusChanged(bool focused) {
        if (!showMessages) { // not currently displaying a series of messages, ie. can begin interaction
            // show / hide interaction text
            if (focused) ShowTooltip();
            else ShowMessage(null);
        }
        playerInInteractionRadius = focused;
    }
}
