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
    
    [Header("Dash")]
    public AudioClip dashBeginSound;
    public AudioClip dashEndSound;

    [Header("Shoot")] 
    public AudioClip[] shootSounds;
    
    [Header("Walk")]
    public AudioClip[] footstepSounds;

    [Header("Interaction")]
    public AudioClip fragmentInteraction;
    
    // TODO: add audio clips for other player events
}
