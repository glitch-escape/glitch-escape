using System;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.DualShock;

public class InputDeviceCheck : MonoBehaviour {

    // Optional inspector variables
    public bool changeText;
    public string keyboard, controller, xbox;

    private TMP_Text text;
    private void Awake() {
        text = text ?? Enforcements.GetComponentInChildren<TMP_Text>(this);
    }

    private void Update() {
        if (IsControllerInput()) {
            // DS4 controller text
            if (DualShockGamepad.current != null)
                text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/ControllerButtons");
            // Xbox controller text
            else
                text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/xboxbuttons");

            if(changeText && DualShockGamepad.current != null) text.text = controller;
            else if(changeText) text.text = xbox;
        }
        else if (IsKeyboardInput()) {
            // Mouse and keyboard text
            text.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/keyboardbuttons");

            if(changeText) text.text = keyboard;
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
