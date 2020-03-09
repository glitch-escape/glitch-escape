using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlitchShaderTestController : MonoBehaviour {
    private Material material;
    public float durationPeriod = 1f;
    public float minDurationPeriod = 1e-2f;
    public float maxDurationPeriod = 10f;
    
    private void OnEnable() {
        material = GetComponent<Renderer>().material;
    }
    void OnGUI() {
        if (material == null) {
            GUILayout.Label("no attached material!");
            return;
        }
        if (GUILayout.Button("Restart shader effect")) {
            material.SetFloat(
                PlayerDashController.GLITCH_MATERIAL_START_TIME,
                Time.time);
        }
        GUILayout.Label("animation duration: " +
                (durationPeriod >= 1f ? "" + durationPeriod + " second(s)" : "" + (durationPeriod * 1e3) + " ms"));
        var period = GUILayout.HorizontalSlider(
            durationPeriod, 
            minDurationPeriod, 
            maxDurationPeriod);

        if (period != durationPeriod) {
            material.SetFloat(
            PlayerDashController.GLITCH_MATERIAL_DURATION,
                durationPeriod = period);
        }
    }
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
