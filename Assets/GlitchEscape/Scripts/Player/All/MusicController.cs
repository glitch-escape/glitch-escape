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
                if (source.isPlaying) {
                    source.Stop();
                } else {
                    source.Play();
                }
            } else if (dpad.left.wasPressedThisFrame) {
                PlayNextTrack();
            } else if (dpad.right.wasPressedThisFrame) {
                PlayPrevTrack();
            }
        }

        if (useKeyboardToControlTracks && Keyboard.current != null) {
            var kb = Keyboard.current;
            if (kb.pKey.wasPressedThisFrame) {
                if (source.isPlaying) {
                    source.Stop();
                } else {
                    source.Play();
                }
            } else if (kb.leftBracketKey.wasPressedThisFrame) {
                PlayNextTrack();
            } else if (kb.rightBracketKey.wasPressedThisFrame) {
                PlayPrevTrack();
            }
        }
    }
    public void PlayTrack(int track) {
        if (track >= 0 && track < audioTracks.Length) {
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
        currentTrackIndex = (currentTrackIndex + 1) % audioTracks.Length;
        PlayTrack(currentTrackIndex);
    }
    public void PlayPrevTrack() {
        currentTrackIndex = (currentTrackIndex - 1) % audioTracks.Length;
        PlayTrack(currentTrackIndex);
    }
}
