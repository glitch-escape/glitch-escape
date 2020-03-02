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

    public bool press;
    private void Update() {
        if (Mouse.current.leftButton.isPressed) {
            inputActionEventList.Add("Hello, world!");
        }
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

    void AddCallbacks(InputAction action, string actionName) {
        action.performed += context => inputActionEventList.Add("Performed " + actionName + "? type=" +
                                                                context.valueType
                                                                + " performed=" + context.performed + " cancelled=" +
                                                                context.canceled);
        
        action.canceled += context => inputActionEventList.Add("Canceled " + actionName + "? type=" +
                                                                context.valueType
                                                                + " performed=" + context.performed + " cancelled=" +
                                                                context.canceled);
        action.started += context => inputActionEventList.Add("Started " + actionName + "? type=" +
                                                                context.valueType
                                                                + " performed=" + context.performed + " cancelled=" +
                                                                context.canceled);
    }
    void OnEnable() {
        PlayerControls.instance.onInputControlTypeChanged += type
            => inputActionEventList.Add("switched active controls to " + type);
        
        AddCallbacks(PlayerControls.instance.dash, "dash");
        AddCallbacks(PlayerControls.instance.dodge, "dodge");
        AddCallbacks(PlayerControls.instance.jump, "jump");
        AddCallbacks(PlayerControls.instance.manifest, "manifest");
    }
}
