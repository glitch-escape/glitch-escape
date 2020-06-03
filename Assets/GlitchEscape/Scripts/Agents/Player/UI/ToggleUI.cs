using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class ToggleUI : MonoBehaviour {
    public GameObject[] uiElements;
    public bool toggleWithHKey = true;
    public bool active = true;
    private bool _active = true;
    public void SetActive(bool active) {
        this.active = _active = active;
        foreach (var controller in FindObjectsOfType<FogDetailController>()) {
            controller.showDebugUI = active;
        }
        foreach (var element in uiElements) {
            element.SetActive(active);
        }
    }
    public void ToggleActive() {
        SetActive(!active);
    }
    private void Awake() {
        SetActive(active);
    }
    void Update() {
        #if UNITY_EDITOR
        if (active != _active) SetActive(active);
        #endif
        if (toggleWithHKey && Keyboard.current.hKey.wasPressedThisFrame) ToggleActive();
    }
}
