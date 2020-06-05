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

    void SetActive(bool active, PlayerControls.InputControlType inputType) {
        dualshockButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.DualshockGamepad);
        xboxButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.XboxGamepad);
        keyboardButtonPrompt?.gameObject.SetActive(active && inputType == PlayerControls.InputControlType.MouseAndKeyboard);
    }
    public override void OnInteract(Player player) {}
    public override void OnFocusChanged(bool focused) {
        if (focused != hasFocus) {
            hasFocus = focused;
            var player = GameObject.FindObjectOfType<Player>();
            if (focused && player != null) {
                player.input.onInputControlTypeChanged += OnActiveInputTypeChanged;
            }
            else if (player != null) {
                player.input.onInputControlTypeChanged -= OnActiveInputTypeChanged;
            }
        }
    }
    private void OnActiveInputTypeChanged(PlayerControls.InputControlType type) {
        SetActive(hasFocus, type);
    }
    private void OnEnable() {
        SetActive(hasFocus, PlayerControls.activeControlType);
    }
    private void OnDisable() {
        if (hasFocus) {
            var player = GameObject.FindObjectOfType<Player>();
            if (player != null) {
                player.input.onInputControlTypeChanged -= OnActiveInputTypeChanged;
            }

            hasFocus = false;
        }
    }
}
