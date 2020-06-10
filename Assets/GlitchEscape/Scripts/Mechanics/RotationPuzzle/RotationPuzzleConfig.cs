using UnityEngine;

[CreateAssetMenu(fileName="GameConfigs", menuName = "Rotation Puzzle Config", order = 1)]
public class RotationPuzzleConfig : ScriptableObject {
    public float fullRotationAngle = 45f;
    public AudioClip fullRotationAudioClip;
    public bool useAudioClipDuration = true;
    public float fallbackDuration = 3f;
    public bool stopAudioAtEnd = true;
}
