using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionMenuSelector : MonoBehaviour {
    [InjectComponent] public TMP_Dropdown resolutionDropdown;
    [InjectComponent] public Toggle fullscreenToggle;
    private Resolution[] resolutions;

    private void OnEnable() {
        resolutions = Screen.resolutions;
        resolutionDropdown.options.Clear();
        foreach (var resolution in resolutions) {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        }
        resolutionDropdown.onValueChanged.AddListener(OnValueChanged);
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        fullscreenToggle.onValueChanged.AddListener(OnToggledFullscreen);
    }
    private void OnDisable() {
        resolutionDropdown.onValueChanged.RemoveListener(OnValueChanged);   
        fullscreenToggle.onValueChanged.RemoveListener(OnToggledFullscreen);   
    }
    private void OnToggledFullscreen(bool fullscreen) {
        Screen.SetResolution(Screen.width, Screen.height, fullscreen);
    }
    private void OnValueChanged(int i ) {
        if (i >= 0 && i < resolutions.Length) {
            Screen.SetResolution(resolutions[i].width, resolutions[i].height, Screen.fullScreen);
        }
    }
}
