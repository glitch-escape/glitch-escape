using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudioController : MonoBehaviourWithConfig<PlayerAudioConfig> {
    [InjectComponent] public Player player;
    [InjectComponent] public PlayerDashController dash;
    [InjectComponent] public PlayerJumpController jump;
    [InjectComponent] public AudioSource soundSource;

    private void PlaySound(AudioClip clip) {
        if (clip == null) return;
        soundSource.PlayOneShot(clip);
    }
    private void PlaySound(AudioClip[] clips) {
        if (clips.Length <= 0) return;
        PlaySound(clips[Random.Range(0, clips.Length - 1)]);
    }
    void OnEnable() {
        dash.OnDashBegin += OnDashBegin;
        dash.OnDashEnd += OnDashEnd;
        jump.OnFloorJump += OnFloorJump;
        jump.OnAirJump += OnAirJump;
        jump.OnWallJump += OnWallJump;
    }
    void OnDisable() {
        
    }
    private void OnFloorJump () { PlaySound(config.floorJumpSound); }
    private void OnAirJump () { PlaySound(config.airJumpSound); }
    private void OnWallJump () { PlaySound(config.wallJumpSound); }
    private void OnDashBegin () { PlaySound(config.dashBeginSound); }
    private void OnDashEnd () { PlaySound(config.dashEndSound); }
    private void OnFootStep () { PlaySound(config.footstepSounds); }
}
