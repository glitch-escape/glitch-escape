using System;
using System.Collections;
using System.Collections.Generic;
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
            var info = new FragmentInfo {
                virtue = fragment.virtueType,
                fragmentsPickedUp = GetFragmentsPickedUp(fragment.virtueType),
                totalFragments = GetFragmentTotal(fragment.virtueType),
                fragmentCompletion = GetFragmentCompletion(fragment.virtueType)
            };
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
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }
    void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        var fragmentManager = GameObject.FindObjectOfType<SceneFragmentManager>();
        activeVirtueInThisScene = fragmentManager?.virtueType ?? Virtue.None;
        onActiveVirtueChanged?.Invoke(activeVirtueInThisScene);
    }
}