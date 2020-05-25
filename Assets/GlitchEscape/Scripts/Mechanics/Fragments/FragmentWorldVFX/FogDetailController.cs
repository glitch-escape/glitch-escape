using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class FogDetailController : MonoBehaviour {
    public FragmentWorldVFX[] presets;
    public int activePreset = 0;

    void Start() {
        if (activePreset >= presets.Length) activePreset = 0;
        if (activePreset < 0) activePreset = presets.Length - 1;
        for (int i = 0; i < presets.Length; ++i) {
            presets[i].gameObject.SetActive(i == activePreset);
        }
    }

    // Update is called once per frame
    void Update() {
        var prevPreset = activePreset;
        if (Gamepad.current?.rightTrigger.wasPressedThisFrame ?? false) {
            ++activePreset;
        }
        if (Gamepad.current?.leftTrigger.wasPressedThisFrame ?? false) {
            --activePreset;
        }
        if (activePreset >= presets.Length) activePreset = 0;
        if (activePreset < 0) activePreset = presets.Length - 1;
        if (activePreset != prevPreset) {
            for (int i = 0; i < presets.Length; ++i) {
                presets[i].gameObject.SetActive(i == activePreset);
            }
        }
    }
    private void OnGUI() {
        GUILayout.Label("active preset: " + presets[activePreset].name);
    }
}
