﻿using System;
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
    /// Call this function to switch mazes (with a cooldown, ie. this call may fail)
    /// </summary>
    /// <returns></returns>
    public bool TriggerMazeSwitch() {
        // Debug.Log("attempting to maze switch");

        var mazeController = SceneMazeController.instance;
        if (mazeController != null && Time.time > lastMazeSwitchTime + mazeSwitchCooldown) {
            // Debug.Log("maze switching...");
            lastMazeSwitchTime = Time.time;
            if (mazeController.inNormalMaze) {
                FireEvent(PlayerEvent.Type.MazeSwitchToGlitchMaze);
            } else {
                FireEvent(PlayerEvent.Type.MazeSwitchToNormalMaze);
            }
            mazeController.SwitchMazes();
            return true;
        }
        return false;
    }
}
