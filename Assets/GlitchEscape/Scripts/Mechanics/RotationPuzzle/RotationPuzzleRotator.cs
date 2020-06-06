using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName="GameConfigs", menuName = "Rotation Puzzle Config", order = 1)]
public class RotationPuzzleConfig : ScriptableObject {
    public float fullRotationAngle = 45f;
    public AudioClip fullRotationAudioClip;
    public bool useAudioClipDuration = true;
    public float fallbackDuration = 3f;
    public bool stopAudioAtEnd = true;
}


public class RotationPuzzleRotator : MonoBehaviour {
    private RotationPuzzleConfig _config;
    public RotationPuzzleConfig config =>
        _config ?? (_config = GameObject.FindObjectOfType<Player>()?.config.rotationPuzzleConfig);
    
    public bool rotating = false;
    public AudioSource audioSource;
    public float angleRotated = 0f;
    public float rotationSpeed;
    public float fullRotation;

    private void StartRotation() {
        // calculate values from config
        var conf = config;
        var audioClip = conf?.fullRotationAudioClip;
        var rotationAngle = conf?.fullRotationAngle ?? 45f;
        var rotationDuration = conf == null ? 5 :
            conf.useAudioClipDuration && audioClip != null ? audioClip.length :
            conf.fallbackDuration;
        
        // set internal variables
        rotating = true;
        angleRotated = 0f;
        rotationSpeed = rotationAngle / rotationDuration;
        fullRotation = rotationAngle;

        // play audio clip
        if (audioClip != null) {
            if (audioSource == null) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }
    }
    private void StopRotation() {
        rotating = false;
        audioSource?.Stop();
    }
    public void StartStopRotation() {
        if (!rotating) {
            StartRotation();
        } else {
            StopRotation();
        }
    }
    private void Update() {
        if (!rotating) return;
        if (angleRotated < Mathf.Abs(fullRotation)) {
            var angle = rotationSpeed * Time.deltaTime;
            angleRotated += angle;
            gameObject.transform.Rotate(Vector3.up, angle, Space.Self);
        } else {
            rotating = false;
            if (config?.stopAudioAtEnd ?? true) {
                audioSource?.Stop();
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RotationPuzzleRotator))]
[CanEditMultipleObjects]
public class RotationPuzzleRotatorEditor : Editor {
    private float FloatField(SerializedObject obj, string name) {
        return obj.FindProperty(name).floatValue = EditorGUILayout.FloatField(obj.FindProperty(name).floatValue, name);
    }
    private bool ToggleField(SerializedObject obj, string name) {
        return obj.FindProperty(name).boolValue = EditorGUILayout.Toggle(obj.FindProperty(name).boolValue, name);
    }
    private void ObjectField<T>(SerializedObject obj, string name) {
        EditorGUILayout.ObjectField(obj.FindProperty(name), typeof(T));
    }

    public void RenderInspectorGUI(RotationPuzzleRotator target) {
        var conf = target.config;
        if (conf == null) {
            GUILayout.Label("can't get config (missing player in this scene...?)");
            return;
        }
        EditorGUILayout.ObjectField(conf, conf.GetType());
        if (GUILayout.Button(("Start / Stop"))) target.StartStopRotation();
        
        // var obj = new SerializedObject(conf);
        // FloatField(obj, "fullRotationAngle");
        // ObjectField<AudioClip>(obj, "fullRotationAudioClip");
        // if (ToggleField(obj, "useAudioClipDuration")) {
        //     FloatField(obj, "fallbackDuration");
        // }
        // if (GUI.changed) obj.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        RenderInspectorGUI((RotationPuzzleRotator)target);
    }
}
#endif
