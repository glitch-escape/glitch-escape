using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FogMenuSelector : MonoBehaviour {
    [InjectComponent] public Slider slider;
    private FogDetailController fogController;

    private void OnEnable() {
        fogController = FindObjectOfType<FogDetailController>();
        slider.value = fogController?.detailLevel ?? 0f;
        slider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnDisable() {
        slider.onValueChanged.RemoveListener(OnValueChanged);
        fogController = null;
    }
    void OnValueChanged(float value) {
        if (fogController != null) fogController.detailLevel = Mathf.FloorToInt(value);
    }
}
