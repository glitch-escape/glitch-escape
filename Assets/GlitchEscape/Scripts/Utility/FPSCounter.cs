using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour {
    [InjectComponent] public TextMeshProUGUI text;
    private float fps = 60f;
    void Update() {
        var step = 0.1f;
        fps = fps * (1f - step) + step / Time.deltaTime;
        text.text = "FPS: " + Mathf.Round(fps);
    }
}
