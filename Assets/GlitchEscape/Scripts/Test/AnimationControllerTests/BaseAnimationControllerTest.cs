using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class BaseAnimationControllerTest : MonoBehaviour {
    public Animator animatedModel;
    private GameObject animatedModelInstance;
    protected Animator animator { get; private set; }
    
    // GUI functions
    protected void ShowAnimatorBoolean(string property, ref bool value) {
        var result = GUILayout.Toggle(value, property);
        if (result != value) {
            animator.SetBool(property, value = result);
        }
    }
    protected void ShowAnimatorFloat(string property, ref float value, float defaultValue, float minValue, float maxValue) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(property + ": " + value);
        var result = GUILayout.HorizontalSlider(value, minValue, maxValue);
        if (GUILayout.Button("reset")) result = defaultValue;
        GUILayout.EndHorizontal();
        if (result != value) {
            animator.SetFloat(property, value = result);
        }
    }
    protected void ShowAnimatorTrigger(string property) {
        if (GUILayout.Button(property)) {
            animator.SetTrigger(property);
        }
    }
    
    void Start() {
        if (animatedModel == null) {
            Debug.LogError("No assigned player model!");
            return;
        }
        // check if instantiated
        var animators = GameObject.FindObjectsOfType<Animator>();
        foreach (var animator in animators) {
            if (animatedModel == animator) {
                animatedModelInstance = animatedModel.gameObject;
                this.animator = animatedModel;
                return;
            }
        }
        // does not exist in scene, so instantiate it
        animatedModelInstance = GameObject.Instantiate(animatedModel.gameObject, transform);
        animator = animatedModelInstance.GetComponent<Animator>();
    }
}
