using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// Manages maze switching etc.
// TODO: add postprocessing effects to glitch maze (see inGlitchMaze and glitchPercentRemaining)
// TODO: move actual maze switch component of this to a MazeController script w/ [InjectComponent] child refs
// and rename this (which should have a much smaller impl) to PlayerMazeSwitchAbility, or something
// (or just roll into player interaction ability...)
public class PlayerMazeController : PlayerComponent {
    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    /// <summary>
    /// Tells us whether we're currently on a maze switch or not
    /// Set by SetMazeSwitch() and ClearMazeSwitch(); also cleared in OnDisable() and OnLevelTransition()
    /// </summary>
    private MazeSwitch activeMazeSwitch = null;
    private bool onMazeSwitch = false;

    /// <summary>
    /// Called by MazeSwitch.OnFocusChanged(true)
    /// </summary>
    public void SetMazeSwitch(MazeSwitch mazeSwitch) {
        activeMazeSwitch = mazeSwitch;
        onMazeSwitch = mazeSwitch == null;
    }
    
    /// <summary>
    /// Called by MazeSwitch.OnFocusChanged(false)
    /// </summary>
    public void ClearMazeSwitch(MazeSwitch mazeSwitch = null) {
        if (mazeSwitch == null || activeMazeSwitch == mazeSwitch) {
            activeMazeSwitch = null;
            onMazeSwitch = false;
        }
    }
    
    /// <summary>
    /// Call this function to switch mazes (with a cooldown, ie. this call may fail)
    /// Called by MazeSwitch.OnInteract()
    /// </summary>
    /// <returns></returns>
    public bool TriggerMazeSwitch() {
        // Debug.Log("attempting to maze switch");

        var mazeController = SceneMazeController.instance;
        if (mazeController != null && Time.time > lastMazeSwitchTime + mazeSwitchCooldown) {
            // Debug.Log("maze switching...");
            lastMazeSwitchTime = Time.time;
            var activeMaze = mazeController.SwitchMazes();
            switch (activeMaze) {
                case SceneMazeController.ActiveMaze.Glitch: FireEvent(PlayerEvent.Type.MazeSwitchToGlitchMaze);
                    break;
                case SceneMazeController.ActiveMaze.Normal: FireEvent(PlayerEvent.Type.MazeSwitchToNormalMaze);
                    break;
            }
            return true;
        }
        return false;
    }

    private void OnEnable() {
        player.OnKilled += OnPlayerRespawn;
    }
    private void OnDisable() {
        player.OnKilled -= OnPlayerRespawn;
        ClearMazeSwitch();
    }
    private void OnPlayerRespawn() {
        SceneMazeController.instance?.ResetMaze();
    }
    void Update() {
        if (!onMazeSwitch && (SceneMazeController.instance?.inGlitchMaze ?? false)) {
            // instead of updating maze timer, just apply damage over time
            // 10 damage / sec, default 100 health = 10 seconds, same as we had previously
            player.TakeDamage(10f * Time.deltaTime);
        }
    }
}
