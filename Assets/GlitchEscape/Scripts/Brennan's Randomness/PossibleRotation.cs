using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleRotation : MonoBehaviour {
    private bool rotate {
        get => rotating;
        set {
            rotating = value;
            if (audioClips.Length > 0) {
                if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
                if (rotating) {
                    var randomClip = audioClips[Mathf.FloorToInt(Random.Range(0f, audioClips.Length - 1e-6f))];
                    audioSource.clip = randomClip;
                    audioSource.Play();
                    audioSource.playOnAwake = false;
                }
            }
            
        }
    }
    private bool rotating = false;
    public float rotationAngle = 45f;   // full angle to rotate
    private float angleRotated = 0f;    // current degrees rotated
    public float fullRotationTime = 6f; // default clip is 6 seconds long
    private float rotationDegreesPerSec => rotationAngle / fullRotationTime;
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    public void StartStopRotation() { audioSource?.Stop(); rotate = !rotate; angleRotated = 0; }
    private void Update() {
        if (!rotating) return;
        if (angleRotated < Mathf.Abs(rotationAngle)) {
            var angle = rotationDegreesPerSec * Time.deltaTime;
            angleRotated += angle;
            this.gameObject.transform.Rotate(Vector3.up, angle, Space.Self);
        } else {
            rotate = false;
        }
    }
}
