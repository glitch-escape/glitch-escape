using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a maze switch that the player can use to switch between mazes.
/// See: Player.cs, PlayerMazeController.cs
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(InteractionTrigger))]
public class MazeSwitch : MonoBehaviour, IActiveInteract
{
    private Material material;
    private float speedWhenActive;
    private Color colorWhenActive;
    private const float speedWhenDisabled = 0f;
    private static Color colorWhenDisabled = Color.grey;

    //[InjectComponent] public Player player;
    
    private const string SPEED_PARAM = "Speed_EA381B29";
    private const string COLOR_PARAM = "Color_C8A5C6B";

    public bool isInteractive => true;

    void Awake() {
        material = GetComponent<Renderer>().material;
        speedWhenActive = material.GetFloat(SPEED_PARAM);
        colorWhenActive = material.GetColor(COLOR_PARAM);
        SetMazeSwitchActive(false);
    }
    private void SetMazeSwitchActive(bool enabled) {
        if (enabled) {
            material.SetFloat(SPEED_PARAM, speedWhenActive);
            material.SetColor(COLOR_PARAM, colorWhenActive);
        } else {
            material.SetFloat(SPEED_PARAM, speedWhenDisabled);
            material.SetColor(COLOR_PARAM, colorWhenDisabled);
        }
    }
    public void OnInteract(Player player) {
        //SceneMazeController.instance.TriggerMazeSwitch();
        PlayerMazeController.instance.TriggerMazeSwitch();
    }
    public void OnSelected(Player player) {
        player.spawn.SetSpawnPosition(this);
        SetMazeSwitchActive(true);
    }
    public void OnDeselected(Player player) {
        SetMazeSwitchActive(false);
    }
    public void OnPlayerEnterInteractionRadius(Player player) { SetMazeSwitchActive(true); player.spawn.SetSpawnPosition(this); player.config.isOnMazeTrigger = true; }
    public void OnPlayerExitInteractionRadius(Player player) { SetMazeSwitchActive(false); player.config.isOnMazeTrigger = false; }
}
