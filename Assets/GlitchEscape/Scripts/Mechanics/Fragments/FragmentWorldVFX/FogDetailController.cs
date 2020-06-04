using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class FogDetailController : MonoBehaviour {
    public FragmentWorldVFX[] presets;
    public int activePreset = 0;
    public bool showDebugUI = true;
    public bool enableFogLODControlsWithF7AndF8Keys = true;
    public bool enableFogLODControlsWithGamepadTriggers = true;

    void Start() {
        activePreset = PlayerPrefs.GetInt("fogLOD", activePreset);
        if (activePreset >= presets.Length) activePreset = 0;
        if (activePreset < 0) activePreset = presets.Length - 1;
        for (int i = 0; i < presets.Length; ++i) {
            presets[i].gameObject.SetActive(i == activePreset);
        }
    }

    // Update is called once per frame
    void Update() {
        var prevPreset = activePreset;
        if (enableFogLODControlsWithGamepadTriggers) {
            if (Gamepad.current?.rightTrigger.wasPressedThisFrame ?? false) {
                ++activePreset;
            }
            if (Gamepad.current?.leftTrigger.wasPressedThisFrame ?? false) {
                --activePreset;
            }
        }
        if (enableFogLODControlsWithF7AndF8Keys) {
            if (Keyboard.current.f8Key.wasPressedThisFrame) {
                ++activePreset;
            }
            if (Keyboard.current.f7Key.wasPressedThisFrame) {
                --activePreset;
            }
        }
        if (activePreset >= presets.Length) activePreset = 0;
        if (activePreset < 0) activePreset = presets.Length - 1;
        if (activePreset != prevPreset) {
            PlayerPrefs.SetFloat("fogLOD", activePreset);
            PlayerPrefs.Save();
            for (int i = 0; i < presets.Length; ++i) {
                presets[i].gameObject.SetActive(i == activePreset);
            }
        }
    } 
    private void OnGUI() {
        if (!showDebugUI) return;
        GUILayout.Label("active preset: " + presets[activePreset].name);
        bool changedPreset = false;
        for (int i = 0; i < presets.Length; ++i) {
            if (GUILayout.Button(presets[i].gameObject.name)) {
                activePreset = i;
                changedPreset = true;
            }
        }
        if (changedPreset) {
            for (int i = 0; i < presets.Length; ++i) {
                presets[i].gameObject.SetActive(i == activePreset);
            }
        }
    }
}
