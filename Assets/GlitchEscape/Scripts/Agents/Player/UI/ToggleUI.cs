using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class ToggleUI : MonoBehaviour {
    public GameObject[] uiElements;
    public bool toggleWithHKey = true;
    public bool active = true;
    private bool _active = true;
    public void SetActive(bool active) {
        this.active = _active = active;
        PlayerPrefs.SetInt("hideHUD", active ? 1 : 0);
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
        active = PlayerPrefs.GetInt("hideHUD", active ? 1 : 0) != 0;
        SetActive(active);
    }
    void Update() {
        #if UNITY_EDITOR
        if (active != _active) SetActive(active);
        #endif
        if (toggleWithHKey && Keyboard.current.hKey.wasPressedThisFrame) ToggleActive();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }
    void OnLevelLoaded(Scene scene, LoadSceneMode mode) {
        SetActive(active);
    }
}
