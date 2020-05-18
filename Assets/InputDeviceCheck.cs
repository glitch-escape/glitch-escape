using System;
using UnityEngine;
using TMPro;

public class InputDeviceCheck : MonoBehaviour
{
    private enum InputDevice
    {
        MouseKeyboard,
        Gamepad
    };

    private InputDevice _device = InputDevice.MouseKeyboard;
    private TMP_Text _message;
    private void Awake() {
        _message = _message ?? Enforcements.GetComponent<TMP_Text>(this);
        _message.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/ds4_icons");
    }

    private void Update() {
        switch(_device)
        {
            case InputDevice.MouseKeyboard:
                if (IsControllerInput()) {
                    _device = InputDevice.Gamepad;
                    _message.spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/ds4_icons");
                }
                break;
            
            case InputDevice.Gamepad:
                if (IsKeyboardInput()) {
                    _device = InputDevice.MouseKeyboard;
                    // Still need Keyboard sprites
                    _message.spriteAsset = null;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static bool IsControllerInput() {
        return false;
    }
    
    private static bool IsKeyboardInput() {
        return false;
    }
}
