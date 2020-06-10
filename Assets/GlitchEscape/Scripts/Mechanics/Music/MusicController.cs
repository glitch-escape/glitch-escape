using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {
    public AudioClip[] audioTracks;
    public int currentTrackIndex = 0;
    public enum PlaybackBehavior {
        LoopTrack,
        PlayAllTracks
    };
    public PlaybackBehavior playbackBehavior = PlaybackBehavior.LoopTrack;
    public bool useDpadToControlTracks = true;
    public bool useKeyboardToControlTracks = true;
    private AudioSource source;
    private bool isPaused = false;
    
    void Start() {
        source = GetComponent<AudioSource>();
        PlayTrack(currentTrackIndex);
    }
    void Update() {
        if (!source.isPlaying) {
            switch (playbackBehavior) {
                case PlaybackBehavior.LoopTrack: ReplayThisTrack(); break;
                case PlaybackBehavior.PlayAllTracks: PlayNextTrack(); break;
            }
        }
        if (useDpadToControlTracks && Gamepad.current != null) {
            var dpad = Gamepad.current.dpad;
            if (dpad.up.wasPressedThisFrame) {
                // if (!isPaused) {
                //     source.Pause();
                //     isPaused = true;
                // } else {
                //     source.UnPause();
                //     isPaused = false;
                // }
            } else if (dpad.left.wasPressedThisFrame) {
                PlayNextTrack();
            } else if (dpad.right.wasPressedThisFrame) {
                PlayPrevTrack();
            }
        }

        if (useKeyboardToControlTracks && Keyboard.current != null) {
            var kb = Keyboard.current;
            if (kb.pKey.wasPressedThisFrame) {
                // if (!isPaused) {
                //     source.Pause();
                //     isPaused = true;
                // } else {
                //     source.UnPause();
                //     isPaused = false;
                // }
            } else if (kb.leftBracketKey.wasPressedThisFrame) {
                PlayNextTrack();
            } else if (kb.rightBracketKey.wasPressedThisFrame) {
                PlayPrevTrack();
            }
        }
    }
    public void PlayTrack(int track) {
        if (track >= 0 && track < audioTracks.Length) {
            isPaused = false;
            if (source.isPlaying) {
                source.Stop();
            }
            source.PlayOneShot(audioTracks[track]);
        }
    }
    public void ReplayThisTrack() {
        PlayTrack(currentTrackIndex);
    }
    public void PlayNextTrack() {
        currentTrackIndex = currentTrackIndex < audioTracks.Length ? currentTrackIndex + 1 : 0;
        PlayTrack(currentTrackIndex);
    }
    public void PlayPrevTrack() {
        currentTrackIndex = currentTrackIndex > 0 ? currentTrackIndex - 1 : audioTracks.Length - 1;
        PlayTrack(currentTrackIndex);
    }
}
