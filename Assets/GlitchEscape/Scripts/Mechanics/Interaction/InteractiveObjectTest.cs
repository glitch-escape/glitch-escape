using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="InteractiveObject"/> test script, used in InteractionTest.unity.
/// Can also be used to debug scripts inheiriting from <see cref="AInteractiveObject"/>:
/// - add this script and set its interaction settings to match the object being tested
/// - can debug interaction + focus via material color changes by setting <see cref="InteractiveObjectTest.materialColorName"/>
/// to the name of a color field of a material on this object (note that "_BaseColor" is the name for builtin URP mats)
/// - can turn on basic debug logging by setting <see cref="InteractiveObjectTest.enableDebugLogging"/>
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Autowire))]
public class InteractiveObjectTest : AInteractiveObject {
    [InjectComponent] public new MeshRenderer renderer;
    public string materialColorName = "_BaseColor";
    public float interactFlashDuration = 0.1f;
    public bool enableDebugLogging = false;
    private bool focused = false;
    
    private void SetColor(Color color) {
        renderer.material.SetColor(materialColorName, color);
        renderer.material.color = color;
    }
    private void Awake() {
        SetColor(Color.gray);
    }
    public override void OnInteract(Player player) {
        if (enableDebugLogging) Debug.Log("interact pressed!");
        StartCoroutine(FlashInteract());
    }
    private IEnumerator FlashInteract() {
        SetColor(Color.cyan);
        yield return new WaitForSeconds(interactFlashDuration);
        SetColor(focused ? Color.green : Color.gray);
    }
    public override void OnFocusChanged(bool focused) {
        if (enableDebugLogging) Debug.Log("Set focused "+focused);
        this.focused = focused;
        SetColor(focused ? Color.green : Color.grey);
    }
}
