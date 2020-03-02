using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Users;

public class PlayerControls : MonoBehaviour {    
    private static PlayerControls m_instance = null;
    public static PlayerControls instance => m_instance ?? 
                                          (m_instance = Enforcements.GetSingleComponentInScene<PlayerControls>());

    private void Awake() {
        if (instance != this) {
            Debug.LogError("Duplicate PlayerControls in scene! " + instance + ", " + this);
        }
    }

    //
    // Inspector properties
    //
    public AnimationCurve gamepadAnalogInputCurve;
    public float mouseSensitivity = 1f;
    
    static float ApplyInputCurve(float input, AnimationCurve inputCurve) {
        var sign = input >= 0f ? 1f : -1f;
        return inputCurve.Evaluate(Mathf.Abs(input)) * sign;
    }
    private void ApplyGamepadInputCurve(ref Vector2 input) {
        if (gamepadAnalogInputCurve != null) {
            input.x = ApplyInputCurve(input.x, gamepadAnalogInputCurve);
            input.y = ApplyInputCurve(input.y, gamepadAnalogInputCurve);
        }
    }
    private Vector2 GetNormalizedMouseInput() {
        var input = Mouse.current.delta.ReadValue() * mouseSensitivity / Screen.width;
        // input.x *= mouseSensitivity / Screen.width;
        // input.y *= mouseSensitivity / Screen.height;
        return input;
    }
    
    //
    // Lazy button properties (assemble these lazily / on an as-needed basis b/c we *cannot* construct these
    // in this object's constructor, b/c of how monobehaviors + script lifetimes + scriptable object lifetimes work)
    // 
    private HybridButtonControl m_dash = null;
    private HybridButtonControl m_dodge = null;
    private HybridButtonControl m_jump = null;
    private HybridButtonControl m_manifest = null;
    private HybridButtonControl m_interact = null;

    /// <summary>
    /// Provides hardcoded button state + callbacks for the Dash action
    /// </summary>
    public HybridButtonControl dash =>
        m_dash ?? (m_dash = new HybridButtonControl(
            new IndirectButtonControl(Keyboard.current.leftShiftKey),
            new IndirectButtonControl(() => Gamepad.current?.rightShoulder)));
    
    /// <summary>
    /// Provides hardcoded button state + callbacks for the Dodge action
    /// </summary>
    public HybridButtonControl dodge =>
        
        m_dodge ?? (m_dodge = new HybridButtonControl(
            new IndirectButtonControl(Keyboard.current.cKey),
            new IndirectButtonControl(() => Gamepad.current?.buttonEast)));
   
    /// <summary>
    /// Provides hardcoded button state + callbacks for the Jump action
    /// </summary>
    public HybridButtonControl jump =>
        m_jump ?? (m_jump = new HybridButtonControl(
            new IndirectButtonControl(Keyboard.current.spaceKey),
            new IndirectButtonControl(() => Gamepad.current?.buttonSouth)));
    
    /// <summary>
    /// Provides hardcoded button state + callbacks for the Manifest action
    /// </summary>
    public HybridButtonControl manifest =>
        m_manifest ?? (m_manifest = new HybridButtonControl(
            new IndirectButtonControl(Keyboard.current.tKey),
            new IndirectButtonControl(() => Gamepad.current?.leftShoulder)));
    
    /// <summary>
    /// Provides hardcoded button state + callbacks for the Interact action
    /// </summary>
    public HybridButtonControl interact =>
        m_interact ?? (m_interact = new HybridButtonControl(
            new IndirectButtonControl(Keyboard.current.eKey),
            new IndirectButtonControl(() => Gamepad.current?.buttonWest)));
    
    //
    // getters for 2d move + look inputs
    //
    public Vector2 moveInput {
        get {
            var input = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
            if (input.magnitude > 0f) {
                buttonPollInfo.lastGamepadPressTime = Time.time;
                ApplyGamepadInputCurve(ref input);
            }
            var kb = Keyboard.current;
            if (kb.aKey.isPressed) { input.x -= 1f; buttonPollInfo.lastKeyboardPressTime = Time.time; }
            if (kb.dKey.isPressed) { input.x += 1f; buttonPollInfo.lastKeyboardPressTime = Time.time; }
            if (kb.sKey.isPressed) { input.y -= 1f; buttonPollInfo.lastKeyboardPressTime = Time.time; }
            if (kb.wKey.isPressed) { input.y += 1f; buttonPollInfo.lastKeyboardPressTime = Time.time; }
            if (input.magnitude > 1f) input.Normalize();
            return input;
        }
    }
    public Vector2 lookInput {
        get {
            var input = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;
            if (input.magnitude > 0f) {
                buttonPollInfo.lastGamepadPressTime = Time.time;
                ApplyGamepadInputCurve(ref input);
            }
            var mouseInput = GetNormalizedMouseInput();
            if (mouseInput.magnitude > 0f) {
                buttonPollInfo.lastKeyboardPressTime = Time.time;
            }
            input += mouseInput;
            if (input.magnitude > 1f) input.Normalize();
            return input;
        }
    }

    /// <summary>
    /// Describes the currently active input device (Mouse + Keyboard or gamepad, and if so, which type)
    /// </summary>
    public enum InputControlType {
        None,
        MouseAndKeyboard,
        DualshockGamepad,
        XboxGamepad
    };

    /// <summary>
    /// Get the currently active input device, determined using <device>.lastUpdateTime + then checking, if this is
    /// a gamepad, which gamepad device is currently active.
    ///
    /// FIXME: currently somewhat broken for DS4 controllers on macos - controller aggressively updates + thus
    /// reports itself as active. w/ an xbox controller this seems to work fine.
    ///
    /// Potential, albeit labor intensive fix: manually poll all inputs + thus manually check for when a mouse,
    /// keyboard, or gamepad button / axis is pressed or reports any other kind of input 
    /// </summary>
    // public static InputControlType activeControlType =>
    //     m_instance == null ? InputControlType.None :
    //     Gamepad.current != null 
    //         && Gamepad.current.lastUpdateTime >=
    //             Math.Max(Keyboard.current.lastUpdateTime, Mouse.current.lastUpdateTime)
    //         ? (DualShockGamepad.current != null
    //             ? InputControlType.DualshockGamepad
    //             : InputControlType.XboxGamepad)
    //         : InputControlType.MouseAndKeyboard;
    public static InputControlType activeControlType { get; private set; } = InputControlType.None;
    
    public delegate void InputControlTypeChanged (InputControlType newType);
    
    /// <summary>
    /// Callback you can subscribe to that will report when the currently active / most active input device
    /// changes.
    ///
    /// FIXME: currently slightly broken in some cases; see activeControllerType
    /// </summary>
    public event InputControlTypeChanged onInputControlTypeChanged;
    private InputControlType m_lastControlType = InputControlType.None;

    private ButtonPollInfo buttonPollInfo = new ButtonPollInfo();
    
    void Update() {
        // check buttons + send callback events
        m_dash?.PollAndDispatchEvents(ref buttonPollInfo);
        m_dodge?.PollAndDispatchEvents(ref buttonPollInfo);
        m_manifest?.PollAndDispatchEvents(ref buttonPollInfo);
        m_jump?.PollAndDispatchEvents(ref buttonPollInfo);
        m_interact?.PollAndDispatchEvents(ref buttonPollInfo);

        activeControlType = buttonPollInfo.lastGamepadPressTime >= buttonPollInfo.lastKeyboardPressTime
            ? DualShockGamepad.current != null ? InputControlType.DualshockGamepad : InputControlType.XboxGamepad
            : InputControlType.MouseAndKeyboard;
        
        // check activeControlType + dispatch onInputControlTypeChanged event
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
        if (onInputControlTypeChanged != null) {
            onInputControlTypeChanged(activeControlType);
        }
    }
    
    public delegate ButtonControl ButtonControlGetter ();
    /// <summary>
    /// Wraps an InputSystem ButtonControl. Constructor takes either a ButtonControl argument (which it forwards
    /// calls to), or a zero-argument callback that returns a button control.
    ///
    /// The reasoning here is as follows:
    /// - some control types, like Keyboard, have object instances that are presumably somewhat permanent
    /// (ie. Keyboard.current.spaceKey)
    /// - others, like Gamepad (Gamepad.current) do not, but we can wrap this with a small function call that returns
    /// either a ButtonControl or null at runtime. (ie. () => Gamepad.current?.buttonSouth)
    /// </summary>
    public struct IndirectButtonControl {
        private ButtonControl control;
        private ButtonControlGetter controlGetter;
        public IndirectButtonControl(ButtonControlGetter getter) {
            control = null;
            controlGetter = getter;
        }
        public IndirectButtonControl(ButtonControl control) {
            this.control = control;
            controlGetter = null;
        }
        public bool isPressed => 
            control?.isPressed ??
            controlGetter()?.isPressed ?? false;
        public bool wasPressedThisFrame => 
            control?.wasPressedThisFrame ??
            controlGetter()?.wasPressedThisFrame ?? false;
        public bool wasReleasedThisFrame => 
            control?.wasReleasedThisFrame ??
            controlGetter()?.wasReleasedThisFrame ?? false;
    }

    public delegate void ButtonPressCallback(bool pressed, HybridButtonControl button);

    public struct ButtonPollInfo {
        public float lastGamepadPressTime;
        public float lastKeyboardPressTime;
    }
    
    /// <summary>
    /// Combines two ıuttonControls, or callbacks that may return a ButtonControl, via IndirectButtonControl.
    /// Used to implement hybrid mouse / keyboard and gamepad button inputs.
    /// Used b/c InputAction is a POS and it is currently impossible to tell when a button has been pressed or released
    /// using that approach
    /// </summary>
    public class HybridButtonControl {
        private IndirectButtonControl keyboardButton;
        private IndirectButtonControl gamepadButton;

        public HybridButtonControl(IndirectButtonControl keyboardButton, IndirectButtonControl gamepadButton) {
            this.keyboardButton = keyboardButton;
            this.gamepadButton = gamepadButton;
        }
        public bool isPressed => keyboardButton.isPressed || gamepadButton.isPressed;
        public bool wasPressedThisFrame => keyboardButton.wasPressedThisFrame || gamepadButton.wasPressedThisFrame;
        public bool wasReleasedThisFrame => keyboardButton.wasReleasedThisFrame || gamepadButton.wasReleasedThisFrame;

        public ButtonPressCallback onPressed;
        public ButtonPressCallback onReleased;
        public ButtonPressCallback onChanged;

        private float m_startPressTime = 0f;
        private float m_endPressTime = 0f;
        private bool m_isPressed = false;
        public float pressTime => (m_isPressed ? Time.time : m_endPressTime) - m_startPressTime;
        
        public void PollAndDispatchEvents(ref ButtonPollInfo pollInfo) {
            bool keyPressed = false;
            bool keyReleased = false;
            
            // check individual press states to update key press + track device press states
            if (keyboardButton.wasPressedThisFrame) {
                pollInfo.lastKeyboardPressTime = Time.time;
                keyPressed = true;
            } else if (gamepadButton.wasPressedThisFrame) {
                pollInfo.lastGamepadPressTime = Time.time;
                keyPressed = true;
            }
            if (keyboardButton.wasReleasedThisFrame) {
                pollInfo.lastKeyboardPressTime = Time.time;
                keyReleased = true;
            } else if (gamepadButton.wasReleasedThisFrame) {
                pollInfo.lastGamepadPressTime = Time.time;
                keyReleased = true;
            }
            // fire release + press events
            // important: if concurrent, fire key release first
            if (keyReleased) {
                m_endPressTime = Time.time;
                m_isPressed = false;
                onReleased?.Invoke(false, this);
                onChanged?.Invoke(false, this);
            }
            if (keyPressed) {
                m_startPressTime = Time.time;
                m_isPressed = true;
                onPressed?.Invoke(true, this);
                onChanged?.Invoke(true, this);
            }
        }
    }
}
