using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Listens to player events (<see cref="PlayerEvent"/>)
/// and plays sounds when events (eg. dash) happen 
/// </summary>
public class PlayerAudioController : MonoBehaviourWithConfig<PlayerAudioConfig> {
    [InjectComponent] public Player player;
    [InjectComponent] public PlayerMovement movement;
    [InjectComponent] public AudioSource soundSource;

    void OnEnable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent += OnPlayerEvent;
        }
    }
    void OnDisable() {
        foreach (var component in GetComponentsInChildren<IPlayerEventSource>()) {
            component.OnEvent -= OnPlayerEvent;
        }
    }
    private void OnPlayerEvent(PlayerEvent.Type eventType) {
        switch (eventType) {
            // interact
            case PlayerEvent.Type.Interact: PlaySound(config.interactSounds); break;
            
            // jump
            case PlayerEvent.Type.FloorJump: PlaySound(config.floorJumpSound); break;
            case PlayerEvent.Type.AirJump: PlaySound(config.airJumpSound); break;
            case PlayerEvent.Type.WallJump: PlaySound(config.wallJumpSound); break;
            case PlayerEvent.Type.EndJump: break;
            
            // dash
            case PlayerEvent.Type.BeginDash: PlaySound(config.dashBeginSound); break;
            case PlayerEvent.Type.EndDash: PlaySound(config.dashEndSound); break;
            
            // shoot
            case PlayerEvent.Type.Shoot: PlaySound(config.shootSounds); break;

            case PlayerEvent.Type.FragmentPickup: PlaySound(config.fragmentInteraction); break;
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
