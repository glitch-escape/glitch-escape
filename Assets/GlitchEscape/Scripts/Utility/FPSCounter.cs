using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.Utility;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class FPSCounter : MonoBehaviour {
    [InjectComponent] public TextMeshProUGUI text;
    private FramerateSampler framerateSampler { get; } = new FramerateSampler();
    public float currentFramerate => framerateSampler.framerate;
    void Update() {
        framerateSampler.Update();
        text.text = "FPS: " + Mathf.Round(currentFramerate) + "\n" + (1f / Time.smoothDeltaTime) + "\n" + Time.unscaledDeltaTime;
    }

    void OnGUI() {
        GUILayout.Label("\n\n\n\n\n\n\n" +  Mathf.Round(currentFramerate) + "\n" + (1f / Time.smoothDeltaTime) + "\n" + Time.unscaledDeltaTime);
    }
}
