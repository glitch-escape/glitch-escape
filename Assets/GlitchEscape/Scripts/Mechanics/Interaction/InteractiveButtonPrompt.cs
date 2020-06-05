using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveButtonPrompt : AInteractiveObject {
    public TMP_Text dualshockButtonPrompt;
    public TMP_Text xboxButtonPrompt;
    public TMP_Text keyboardButtonPrompt;
    private bool hasFocus = false;
    private PlayerControls.InputControlType lastControlType;

    void SetActive(bool active, PlayerControls.InputControlType inputType) {
        if (inputType != PlayerControls.InputControlType.None) {
            lastControlType = inputType;
        } else {
            inputType = lastControlType;
        }
        dualshockButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.DualshockGamepad);
        xboxButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.XboxGamepad);
        keyboardButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.MouseAndKeyboard);
    }
    public override void OnInteract(Player player) {}
    public override void OnFocusChanged(bool focused) {
        SetActive(hasFocus = focused, PlayerControls.activeControlType);
    }
    private void OnActiveInputTypeChanged(PlayerControls.InputControlType type) {
        SetActive(hasFocus, type);
    }
    private void OnEnable() {
        SetActive(hasFocus, PlayerControls.activeControlType);
        var player = GameObject.FindObjectOfType<Player>();
        if (player) player.input.onInputControlTypeChanged += OnActiveInputTypeChanged;
    }
    private void OnDisable() {
        var player = GameObject.FindObjectOfType<Player>();
        if (player) player.input.onInputControlTypeChanged -= OnActiveInputTypeChanged;
        SetActive(hasFocus = false, PlayerControls.activeControlType);
    }
}
