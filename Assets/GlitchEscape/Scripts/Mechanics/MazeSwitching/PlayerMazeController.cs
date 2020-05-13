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
    public NormalMaze normalMaze;
    public GlitchMaze glitchMaze;

    [NonSerialized]
    private static PlayerMazeController _instance = null;

    public static PlayerMazeController instance =>
        _instance ?? (_instance = Enforcements.GetSingleComponentInScene<PlayerMazeController>());

    
    void Awake() {
        _instance = this;
    }
    
    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    /// <summary>
    /// Call this function to switch mazes (with a cooldown, ie. this call may fail)
    /// </summary>
    /// <returns></returns>
    public bool TriggerMazeSwitch() {
        Debug.Log("attempting to maze switch");
        if (Time.time > lastMazeSwitchTime + mazeSwitchCooldown) {
            Debug.Log("maze switching...");
            lastMazeSwitchTime = Time.time;
            CallMazeEvent();
            SceneMazeController.MazesInScene.SwitchMazes();
            return true;
        }
        return false;
    }

    public void CallMazeEvent()
    {
        if (SceneMazeController.MazesInScene.inNormalMaze)
        {
            FireEvent(PlayerEvent.Type.MazeSwitchToGlitchMaze);
        }
        else
        {
            FireEvent(PlayerEvent.Type.MazeSwitchToNormalMaze);
        }
    }
}
