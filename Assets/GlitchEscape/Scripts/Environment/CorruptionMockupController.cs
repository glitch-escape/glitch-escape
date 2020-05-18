using System;
using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;

[ExecuteInEditMode]
public class CorruptionMockupController : MonoBehaviour, IPlayerDebug {
    public FragmentWorldVFX worldParticleSystem;
    public MeshRenderer clouds;
    public Material redCloudMaterial;
    public Material blueCloudMaterial;
    public Transform handsParent;

    public bool redCloudsEnabled = true;
    public bool corruptionHandsEnabled = true;

    public void SetRedCloudsActive(bool active) {
        redCloudsEnabled = active;
        if (active) {
            clouds.material = redCloudMaterial;
        } else {
            clouds.material = blueCloudMaterial;
        }
    }

    public void SetCorruptionHandsEnabled(bool active) {
        corruptionHandsEnabled = active;
        handsParent?.gameObject.SetActive(active);
    }

    private bool wasCloudsEnabled = false;
    private bool wasHandsEnabled = false;

    void Start() {
        SetRedCloudsActive(wasCloudsEnabled = redCloudsEnabled);
        SetCorruptionHandsEnabled(wasHandsEnabled = corruptionHandsEnabled);
    }
    void Update() {
        if (wasCloudsEnabled != redCloudsEnabled) {
            SetRedCloudsActive(wasCloudsEnabled = redCloudsEnabled);
        }
        if (wasHandsEnabled != corruptionHandsEnabled) {
            SetCorruptionHandsEnabled(wasHandsEnabled = corruptionHandsEnabled);
        }
    }
    public void DrawDebugUI() {
        corruptionHandsEnabled = GUILayout.Toggle(corruptionHandsEnabled, "enable corrupted hands");
        redCloudsEnabled = GUILayout.Toggle(redCloudsEnabled, "red clouds");
    }
    public string debugName => GetType().Name;
}
