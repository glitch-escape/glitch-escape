using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// contains all audio clips in the game for the player
/// <see cref="PlayerAudioController"/>
/// </summary>
[CreateAssetMenu(fileName = "PlayerAudioConfig", menuName = "GameConfigs/PlayerAudioConfig", order = 1)]
public class PlayerAudioConfig : ScriptableObject {
    [Header("Interact")] 
    public AudioClip[] interactSounds;
    
    [Header("Jump")]
    public AudioClip floorJumpSound;
    public AudioClip airJumpSound;
    public AudioClip wallJumpSound;
    public AudioClip landingSound;
    
    [Header("Dash")]
    public AudioClip dashBeginSound;
    public AudioClip dashEndSound;

    [Header("Shoot")] 
    public AudioClip[] shootSounds;
    
    [Header("Walk")]
    public AudioClip[] footstepSounds;

    public float firstFootstepOffset = 0.1f;
    public float footstepsPerSec = 1f;
    public float footstepsPerSecVariation = 0.2f;

    [Header("Interaction")]
    public AudioClip fragmentInteraction;

    public AudioClip mazeSwitch;

    // TODO: add audio clips for other player events
}
