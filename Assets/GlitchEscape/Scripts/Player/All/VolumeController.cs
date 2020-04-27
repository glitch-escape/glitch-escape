using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    private AudioSource src;

    private float musicVol = 0.5f;

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    void Update()
    {
        src.volume = musicVol;
    }

    public void SetVolume(float vol)
    {
        musicVol = vol;
    }
}
