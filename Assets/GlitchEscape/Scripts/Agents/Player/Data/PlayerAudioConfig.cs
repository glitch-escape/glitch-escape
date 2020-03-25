using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAudioConfig", menuName = "GameConfigs/PlayerAudioConfig", order = 1)]
public class PlayerAudioConfig : ScriptableObject {
    public AudioClip floorJumpSound;
    public AudioClip airJumpSound;
    public AudioClip wallJumpSound;
    public AudioClip dashBeginSound;
    public AudioClip dashEndSound;
    public AudioClip[] footstepSounds;
}
