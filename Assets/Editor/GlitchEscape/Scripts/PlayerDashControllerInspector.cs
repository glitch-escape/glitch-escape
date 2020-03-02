using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDashController))]
public class PlayerDashControllerInspector : Editor {
    private PlayerDashController m_target;
    void OnEnable() {  }

    private void DrawToggleableMinMaxSlider(string toggleLabel, ref bool toggle, string sliderLabel, ref float min,
        ref float max, float minValue, float maxValue) 
    {
        // toggle = EditorGUILayout.Toggle(toggleLabel, toggle);
        if (toggle) {
            EditorGUILayout.MinMaxSlider(sliderLabel, ref min, ref max, minValue, maxValue);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            min = EditorGUILayout.FloatField("min", min);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            max = EditorGUILayout.FloatField("max", max);
            EditorGUILayout.EndHorizontal();
        } else {
            max = EditorGUILayout.Slider(sliderLabel, max, minValue, maxValue);
        }
    }

    private bool useDuration = false;
    private bool showPressInfo = true;
    private bool showStamina = true;
    private bool showDuration = true;
    private bool showDistance = true;
    private bool showEtc = true;
    
    public override void OnInspectorGUI() {
        var t = (PlayerDashController)target;
        showPressInfo = EditorGUILayout.Foldout(showPressInfo, "press info");
        if (showPressInfo) {
            t.maxPressTime = EditorGUILayout.Slider(
                             "max press duration", 
                             t.maxPressTime, 
                             1e-2f, 
                             1.5f);
            t.pressTimeFunction = EditorGUILayout.CurveField("press time function", t.pressTimeFunction);
            EditorGUILayout.BeginHorizontal();
            t.varyStaminaCostDependingOnPressTime = EditorGUILayout.Toggle("affect stamina", t.varyStaminaCostDependingOnPressTime);
            t.varyDurationDependingOnPressTime =
                EditorGUILayout.Toggle("affect duration / dist", t.varyDurationDependingOnPressTime);
            EditorGUILayout.EndHorizontal();
        }
        
        showStamina = EditorGUILayout.Foldout(showStamina, "stamina cost");
        if (showStamina) {
            DrawToggleableMinMaxSlider("vary using press time", ref t.varyStaminaCostDependingOnPressTime,
                "stamina cost", ref t.minStaminaCost, ref t.maxStaminaCost, 0f, 100f);
        }

        showDuration = EditorGUILayout.Foldout(showDuration, "dash duration (seconds)");
        if (showDuration) {
            DrawToggleableMinMaxSlider("vary using press time", ref t.varyDurationDependingOnPressTime,
                "dash duration (seconds)", ref t.minDuration, ref t.maxDuration, 0f, 5f);
        }
        
        showDistance = EditorGUILayout.Foldout(showDistance, "dash distance (meters)");
        if (showDistance) {
            float minDist = t.dashSpeed * t.minDuration;
            float maxDist = t.dashSpeed * t.maxDuration;
            DrawToggleableMinMaxSlider("vary using press time", ref t.varyDurationDependingOnPressTime,
                "dash distance (meters)", ref minDist, ref maxDist, 0f, 20f);
            t.minDuration = minDist / t.dashSpeed;
            t.maxDuration = maxDist / t.dashSpeed;
        }

        showEtc = EditorGUILayout.Foldout(showEtc, "dash params");
        if (showEtc) {
            var speed = EditorGUILayout.Slider("dash speed (m/s)", t.dashSpeed, 1e-3f, 20f);
            if (useDuration) {
                t.dashSpeed = speed;
            } else if (speed != t.dashSpeed) {
                t.minDuration *= t.dashSpeed / speed;
                t.maxDuration *= t.dashSpeed / speed;
                t.dashSpeed = speed;
            }
            t.dashVfxDuration = EditorGUILayout.Slider("extra vfx duration", t.dashVfxDuration, 0f, 5f);
            t.abilityCooldown = EditorGUILayout.Slider("cooldown", t.abilityCooldown, 0f, 5f);
            t.drawPlayerAbilityDebugGUI = EditorGUILayout.Toggle("draw debug GUI", t.drawPlayerAbilityDebugGUI);
        }
        t.varyStrengthDependingOnPressTime = false;
        t.maxEffectStrength = 1f;
    }
}
