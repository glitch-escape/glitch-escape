using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Player))]
public class PlayerVirtueFragments : PlayerComponent {
    private Dictionary<Virtue, uint> fragmentsPickedUp { get; } = new Dictionary<Virtue, uint>();
    private Dictionary<Virtue, uint> fragmentCounts { get; } = new Dictionary<Virtue, uint>();

    /// <summary>
    /// Returns the total number of fragments picked up. NOT limited by fragment totals.
    /// </summary>
    public uint GetFragmentsPickedUp(Virtue virtue) {
        return fragmentsPickedUp.ContainsKey(virtue) ? fragmentsPickedUp[virtue] : 0;
    }
    
    /// <summary>
    /// Gets the total number of fragments that can be picked up. currently hardcoded to 7.
    /// </summary>
    public uint GetFragmentTotal(Virtue virtue) {
        return 7;
    }
    
    /// <summary>
    /// Returns true if all fragments have been completed for the given virtue
    /// </summary>
    public bool IsVirtueCompleted(Virtue virtue) {
        return GetFragmentsPickedUp(virtue) >= GetFragmentTotal(virtue);
    }

    /// <summary>
    /// Gets fragment completion normalized to [0, 1]
    /// </summary>
    public float GetFragmentCompletion(Virtue virtue) {
        var current = (float)GetFragmentsPickedUp(virtue);
        var total = GetFragmentTotal(virtue);
        return total == 0 ? 0f : Mathf.Clamp01(current / (float)total);
    }
    
    public Virtue activeVirtueInThisScene { get; private set; }
    
    public struct FragmentInfo {
        public Virtue virtue;
        public uint fragmentsPickedUp;
        public uint totalFragments;
        public float fragmentCompletion;

        public override String ToString() {
            return virtue.ToString() + " " + (fragmentCompletion * 100f) + "% complete ("
                   + fragmentsPickedUp + " / " + totalFragments + ")";
        }
    }
    public FragmentInfo GetCurrentFragmentInfo(Virtue virtueType) {
        return new FragmentInfo {
            virtue = virtueType,
            fragmentsPickedUp = GetFragmentsPickedUp(virtueType),
            totalFragments = GetFragmentTotal(virtueType),
            fragmentCompletion = GetFragmentCompletion(virtueType)
        };
    }

    /// <summary>
    /// Use this callback event to listen to fragment pick ups
    /// </summary>
    public event Action<FragmentInfo> onFragmentPickedUp;

    /// <summary>
    /// Use this callback event to listen to active virtue changes
    /// (ie. which virtue is present + can be picked up in this level)
    /// </summary>
    public event Action<Virtue> onActiveVirtueChanged;

    /// <summary>
    /// Callback for when all fragments picked up
    /// </summary>
    public event Action<Virtue> onVirtueCompleted;

    public void SetActiveVirtue(Virtue virtue) {
        if (virtue != activeVirtueInThisScene) {
            activeVirtueInThisScene = virtue;
            onActiveVirtueChanged?.Invoke(virtue);
        }
    }
    
    public void PickUpFragment(FragmentPickup fragment) {
        if (fragment.virtueType != Virtue.None) {
            if (!fragmentsPickedUp.ContainsKey(fragment.virtueType))
                fragmentsPickedUp[fragment.virtueType] = 0;
            fragmentsPickedUp[fragment.virtueType] += 1;
            var info = GetCurrentFragmentInfo(fragment.virtueType);
            onFragmentPickedUp?.Invoke(info);
            if (info.fragmentCompletion >= 1f) {
                onVirtueCompleted?.Invoke(fragment.virtueType);
            }
            FireEvent(PlayerEvent.Type.FragmentPickup);
        } else {
            Debug.LogWarning("picked up fragment with no assigned virtue type: "
                + fragment + ", id = '" + fragment.objectPersistencyId + "' in scene " +
                SceneManager.GetActiveScene().name);  
        }
    }
    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelLoaded;
        infoLog?.SetupListeners(this);
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        infoLog?.SetupListeners(null);
    }
    public void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        var fragmentManager = GameObject.FindObjectOfType<SceneFragmentManager>();
        activeVirtueInThisScene = fragmentManager?.virtueType ?? Virtue.None;
        onActiveVirtueChanged?.Invoke(activeVirtueInThisScene);
    }
    public PlayerFragmentInfoLog infoLog { get; } = new PlayerFragmentInfoLog();
    
    /// <summary>
    /// Added for debugging purposes - clears / resets a fragment type
    /// </summary>
    public void ClearFragments(Virtue virtue) {
        fragmentsPickedUp.Remove(virtue);
        fragmentCounts.Remove(virtue);
        
        // fire an on active virtue changed event to trigger UI updates, etc
        onActiveVirtueChanged?.Invoke(activeVirtueInThisScene);
    }
}

public class PlayerFragmentInfoLog {
    private StringBuilder infoLog { get; } = new StringBuilder();
    public PlayerVirtueFragments currentTarget { get; private set; } = null;

    public String GetInfoLog() {
        return infoLog.ToString();
    }
    public void Clear() {
        infoLog.Clear();
    }
    public void SetupListeners(PlayerVirtueFragments target) {
        if (currentTarget != null) {
            currentTarget.onFragmentPickedUp -= OnFragmentPickedUp;
            currentTarget.onVirtueCompleted -= OnVirtueCompleted;
            currentTarget.onActiveVirtueChanged -= OnActiveVirtueChanged;
        }
        if (target != null) {
            target.onFragmentPickedUp += OnFragmentPickedUp;
            target.onVirtueCompleted += OnVirtueCompleted;
            target.onActiveVirtueChanged += OnActiveVirtueChanged;
        }
        currentTarget = target;
        infoLog.AppendLine("Setup listeners on " + target);
        Debug.Log(GetInfoLog());
    }
    private void OnFragmentPickedUp(PlayerVirtueFragments.FragmentInfo info) {
        infoLog.AppendLine("fragment picked up: " + info);
    }
    private void OnVirtueCompleted(Virtue virtue) {
        infoLog.AppendLine("completed virtue: " + virtue);
    }
    private void OnActiveVirtueChanged(Virtue virtue) {
        infoLog.AppendLine("changed active virtue in scene to " + virtue);
    }
}


[CustomEditor(typeof(PlayerVirtueFragments))]
[CanEditMultipleObjects]
class PlayerFragmentEditor : Editor {
    public static void RenderEditorGUI(PlayerVirtueFragments target) {
        var v = target.activeVirtueInThisScene;
        var selected = (Virtue)EditorGUILayout.EnumPopup("active virtue", v);
        if (selected != v) target.SetActiveVirtue(selected);
        GUILayout.Label("fragment info: " + target.GetCurrentFragmentInfo(selected));

        if (GUILayout.Button("Setup listeners")) {
            target.infoLog.SetupListeners(target);
        }
        if (GUILayout.Button("Pick up fragment")) {
            var tempFragment = new GameObject("temp fragment", typeof(BoxCollider),typeof(FragmentPickup));
            var fragment = tempFragment.GetComponent<FragmentPickup>();
            fragment.virtueType = target.activeVirtueInThisScene;
            target.PickUpFragment(fragment);
            DestroyImmediate(tempFragment);
        }
        if (GUILayout.Button("Clear fragment type")) {
            target.ClearFragments(target.activeVirtueInThisScene);
        }
        if (GUILayout.Button("Simulate level load")) {
            target.OnLevelLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        if (GUILayout.Button("Clear info log")) {
            target.infoLog.Clear();
        }
        GUILayout.Label(target.infoLog.GetInfoLog());
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        RenderEditorGUI((PlayerVirtueFragments)target);
    }
}
