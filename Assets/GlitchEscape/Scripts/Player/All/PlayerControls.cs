using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Users;

public class PlayerControls : MonoBehaviour {
    private Input m_input = null;
    public Input input => m_input ?? (m_input = new Input());
    
    private static PlayerControls m_instance = null;
    public static PlayerControls instance => m_instance ?? 
                                          (m_instance = Enforcements.GetSingleComponentInScene<PlayerControls>());
    
    public Vector2 moveInput => instance.input.Controls.Move.ReadValue<Vector2>();
    public Vector2 lookInput => instance.input.Controls.Look.ReadValue<Vector2>();
    
    public InputAction dash => instance.input.Controls.Dash;
    public InputAction dodge => instance.input.Controls.Dodge;
    public InputAction jump => instance.input.Controls.Jump;
    public InputAction manifest => instance.input.Controls.Manifest;

    public enum InputControlType {
        None,
        MouseAndKeyboard,
        DualshockGamepad,
        XboxGamepad
    };
    public static InputControlType activeControlType =>
        m_instance == null ? InputControlType.None :
        Gamepad.current != null 
            && Gamepad.current.lastUpdateTime >=
                Math.Max(Keyboard.current.lastUpdateTime, Mouse.current.lastUpdateTime)
            ? (DualShockGamepad.current != null
                ? InputControlType.DualshockGamepad
                : InputControlType.XboxGamepad)
            : InputControlType.MouseAndKeyboard;
    
    public delegate void InputControlTypeChanged (InputControlType newType);
    public event InputControlTypeChanged onInputControlTypeChanged;
    private InputControlType m_lastControlType = InputControlType.None;

    void Update() {
        var controlType = activeControlType;
        if (controlType != m_lastControlType && onInputControlTypeChanged != null) {
            m_lastControlType = controlType;
            onInputControlTypeChanged(controlType);
        }
    }
    void OnDisable() {
        m_lastControlType = InputControlType.None;
    }
    private void OnEnable() {
        input.Enable();
        if (onInputControlTypeChanged != null) {
            onInputControlTypeChanged(activeControlType);
        }
    }
}
