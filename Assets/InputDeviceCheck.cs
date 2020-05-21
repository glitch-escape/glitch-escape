using System;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.DualShock;

public class InputDeviceCheck : MonoBehaviour {
    private TMP_Text text;
    private void Awake() {
        text = text ?? Enforcements.GetComponent<TMP_Text>(this);
    }

    private void Update() {
        if (IsControllerInput()) {
            // DS4 controller text
            if (DualShockGamepad.current != null)
                text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/ControllerButtons");
            // Xbox controller text
            else
                text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/xboxbuttons");
        }
        else if (IsKeyboardInput()) {
            // Mouse and keyboard text
            text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/keyboardbuttons");
        }
        
    }

    private bool IsControllerInput() {
        if (Gamepad.current != null)
            return true;
        return false;
    }
    
    private bool IsKeyboardInput() {
        if (Keyboard.current != null)
            return true;
        return false;
    }
}
