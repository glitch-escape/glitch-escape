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
                }
            }
            
        }
    }

    private bool rotating = false;
    Vector3 rotatable = new Vector3(0f, 1f, 0f);
    int degrees = 0;
    int howMuchToRotate = 2;
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    public void flickIt() { audioSource?.Stop(); rotate = !rotate; degrees = 0; }
    private void Update() {
        if(rotate && degrees != 45) {
            this.gameObject.transform.Rotate(rotatable, howMuchToRotate, Space.Self);
            degrees += howMuchToRotate;
            if(degrees >= 45)
            {
                rotate = false;
            }
        }

        if(!rotate && degrees >= 45)
        {
            degrees = 0;
        }
    }
}
