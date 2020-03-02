using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControls))]
public class PlayerControlsTest : MonoBehaviour {
    
    private string getActiveControlSchemeLabel () {
        switch (PlayerControls.activeControlType) {
            case PlayerControls.InputControlType.None: return "None";
            case PlayerControls.InputControlType.DualshockGamepad: return "DS4 Gamepad";
            case PlayerControls.InputControlType.XboxGamepad: return "Xbox Gamepad";
            case PlayerControls.InputControlType.MouseAndKeyboard: return "Mouse + Keyboard";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void OnGUI() {
        GUILayout.Label("Active player controls: " + PlayerControls.instance + " on object " 
                        + PlayerControls.instance.gameObject);
        GUILayout.Label("Active control scheme: " + getActiveControlSchemeLabel());
        GUILayout.Label("Look input: "+PlayerControls.instance.lookInput);
        GUILayout.Label("Move input: "+PlayerControls.instance.moveInput);
    }
}
