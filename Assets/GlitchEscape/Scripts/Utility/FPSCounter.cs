using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Scripts.Utility;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour {
    [InjectComponent] public TextMeshProUGUI text;
    private FramerateSampler framerateSampler { get; } = new FramerateSampler();
    public float currentFramerate => framerateSampler.framerate;
    void Update() {
        framerateSampler.Update();
        if (framerateSampler.hasSamples) {
            text.text = "FPS: " + Mathf.Round(currentFramerate);
                        // + "\n" + (1f / Time.smoothDeltaTime) + "\n" + Time.unscaledDeltaTime;
        } else {
            text.text = "";
        }
    }
    // void OnGUI() {
    //     GUILayout.Label("\n\n\n\n\n\n\n" +  Mathf.Round(currentFramerate) + "\n" + (1f / Time.smoothDeltaTime) + "\n" + Time.unscaledDeltaTime);
    // }
}
