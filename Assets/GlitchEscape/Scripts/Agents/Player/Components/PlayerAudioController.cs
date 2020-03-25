using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Listens to player events (<see cref="PlayerAbilityEvent"/>)
/// and plays sounds when events (eg. dash) happen 
/// </summary>
public class PlayerAudioController : MonoBehaviourWithConfig<PlayerAudioConfig> {
    [InjectComponent] public Player player;
    [InjectComponent] public PlayerMovementController movement;
    [InjectComponent] public AudioSource soundSource;

    void OnEnable() {
        foreach (var ability in GetComponentsInChildren<PlayerAbility>()) {
            ability.OnAbilityEvent += OnPlayerAbilityEvent;
        }
        // TODO: add listeners for other events, eg. player movement?
    }
    void OnDisable() {
        foreach (var ability in GetComponentsInChildren<PlayerAbility>()) {
            ability.OnAbilityEvent -= OnPlayerAbilityEvent;
        }
        // TODO: add listeners for other events, eg. player movement?
    }
    private void OnPlayerAbilityEvent(PlayerAbilityEvent.Type eventType) {
        switch (eventType) {
            // interact
            case PlayerAbilityEvent.Type.Interact: PlaySound(config.interactSounds); break;
            
            // jump
            case PlayerAbilityEvent.Type.FloorJump: PlaySound(config.floorJumpSound); break;
            case PlayerAbilityEvent.Type.AirJump: PlaySound(config.airJumpSound); break;
            case PlayerAbilityEvent.Type.WallJump: PlaySound(config.wallJumpSound); break;
            case PlayerAbilityEvent.Type.EndJump: break;
            
            // dash
            case PlayerAbilityEvent.Type.BeginDash: PlaySound(config.dashBeginSound); break;
            case PlayerAbilityEvent.Type.EndDash: PlaySound(config.dashEndSound); break;
            
            // shoot
            case PlayerAbilityEvent.Type.Shoot: PlaySound(config.shootSounds); break;
            
            // TODO: add audio clips for other player events
            
            default: break;
                // throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
    /// <summary>
    /// Plays an audio clip (if non-null)
    /// </summary>
    /// <param name="clip"></param>
    private void PlaySound(AudioClip clip) {
        if (clip == null) return;
        soundSource.PlayOneShot(clip);
    }
    /// <summary>
    /// Plays a random audio clip (if non-empty)
    /// </summary>
    /// <param name="clips"></param>
    private void PlaySound(AudioClip[] clips) {
        if (clips.Length <= 0) return;
        PlaySound(clips[Random.Range(0, clips.Length - 1)]);
    }
}
