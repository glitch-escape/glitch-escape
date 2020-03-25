using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a maze switch that the player can use to switch between mazes.
/// See: Player.cs, PlayerMazeController.cs
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(InteractionTrigger))]
public class MazeSwitch : MonoBehaviour, IPlayerInteractable {
    private Material material;
    private float speedWhenActive;
    private Color colorWhenActive;
    private const float speedWhenDisabled = 0f;
    private static Color colorWhenDisabled = Color.grey;
    
    private const string SPEED_PARAM = "Speed_EA381B29";
    private const string COLOR_PARAM = "Color_C8A5C6B";
    
    
    void Awake() {
        material = GetComponent<Renderer>().material;
        speedWhenActive = material.GetFloat(SPEED_PARAM);
        colorWhenActive = material.GetColor(COLOR_PARAM);
        SetMazeSwitchActive(false);
    }
    /// <summary>
    /// Called by Player's maze switch impl as a response to SetActiveMazeSwitch() + ClearActiveMazeSwitch()
    /// Used to set material to show when the maze switch is active or not
    /// </summary>
    /// <param name="enabled"></param>
    public void SetMazeSwitchActive(bool enabled) {
        if (enabled) {
            material.SetFloat(SPEED_PARAM, speedWhenActive);
            material.SetColor(COLOR_PARAM, colorWhenActive);
        } else {
            material.SetFloat(SPEED_PARAM, speedWhenDisabled);
            material.SetColor(COLOR_PARAM, colorWhenDisabled);
        }
    }

    public void OnInteract(Player player) {
        PlayerMazeController.instance.TriggerMazeSwitch();
    }

    public void OnPlayerEnterInteractionRadius(Player player) {
        player.controller.SetSavePoint(transform);
        player.SetActiveMazeSwitch(this);
    }

    public void OnPlayerExitInteractionRadius(Player player) {
        player.ClearActiveMazeSwitch(this);
    }
}
