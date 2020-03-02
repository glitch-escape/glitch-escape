using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControls))]
public class PlayerControlsTest : MonoBehaviour {
    
    private List<String> inputActionEventList = new List<string>();
    
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

    private void LogInputEvent(string message) {
        inputActionEventList.Add(message);
    }

    private void pollButton(PlayerControls.HybridButtonControl button, string name) {
        if (button.wasPressedThisFrame) LogInputEvent(name + " pressed");
        if (button.wasReleasedThisFrame) LogInputEvent(name + " released");
    }
    private void Update() {
        pollButton(PlayerControls.instance.interact, "interact");
        pollButton(PlayerControls.instance.dodge, "dodge");
        pollButton(PlayerControls.instance.dash, "dash");
        pollButton(PlayerControls.instance.manifest, "manifest");
        pollButton(PlayerControls.instance.jump, "jump");
    }

    private Vector2 scrollPos = Vector2.zero;
    private void OnGUI() {
        GUILayout.Label("Active player controls: " + PlayerControls.instance + " on object " 
                        + PlayerControls.instance.gameObject);
        GUILayout.Label("Active control scheme: " + getActiveControlSchemeLabel());
        GUILayout.Label("Look input: "+PlayerControls.instance.lookInput);
        GUILayout.Label("Move input: "+PlayerControls.instance.moveInput);
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for (var i = inputActionEventList.Count; i --> 0; ) {
            GUILayout.Label(inputActionEventList[i]);
            Debug.Log(""+i+" "+inputActionEventList[i]);
        }
        GUILayout.EndScrollView();
    }
}
