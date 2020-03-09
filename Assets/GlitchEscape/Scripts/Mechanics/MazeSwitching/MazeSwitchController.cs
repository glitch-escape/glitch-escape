using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// Manages maze switching etc.
// TODO: add postprocessing effects to glitch maze (see inGlitchMaze and glitchPercentRemaining)
public class MazeSwitchController : MonoBehaviour, IPlayerControllerComponent {
    private PlayerController controller;
    private Player player;

    private static MazeSwitchController _instance;
    public static MazeSwitchController instance {
        get {
            if (_instance == null) {
                Debug.LogError("No MazeSwitchController in this level!");
            }
            return _instance;
        }
    }
    
    public void SetupControllerComponent(PlayerController controller) {
        _instance = this;
        this.controller = controller;
        player = controller.player;
        if (!defaultMaze) { Debug.LogWarning("MazeSwitchController: default Maze missing!"); }
        if (!glitchMaze) { Debug.LogWarning("MazeSwitchController: default Maze missing!"); }
        if (!countdownTimerText) { Debug.LogError("MazeSwitchController: countdown timer text missing!"); }
        if (activeMaze == ActiveMaze.None) {
            SetMazeActive(ActiveMaze.Default);
        }
        // TODO: get (or check) reference to postprocessing effects here
    }
    public Maze defaultMaze;
    public Maze glitchMaze;
    public Text countdownTimerText;
    public enum ActiveMaze {
        None,
        Default,
        Glitch
    };
    public ActiveMaze activeMaze;

    private float lastMazeSwitchStartTime = 0f;
    private float timeInThisMaze => Time.time - lastMazeSwitchStartTime;
    [Tooltip("seconds")] 
    public float glitchMazeTimeLimit = 10f;
    
    // remaining time for this glitch maze (assumes that inGlitchMaze is true)
    private float glitchMazeTimeRemaining => glitchMazeTimeLimit - timeInThisMaze;
    
    // percentage (actually [0, 1] normalized value) of the way through the glitch timer.
    // for eg. postprocessing effects, use
    //     0 => no effect
    //     ...
    //     1 => maximum effect
    private float glitchPercentRemaining => glitchMazeTimeRemaining / glitchMazeTimeLimit;
    
    // true iff glitch maze is currently active
    public bool inGlitchMaze => activeMaze == ActiveMaze.Glitch;
    
    private Maze GetMaze(ActiveMaze maze) {
        switch (maze) {
            case ActiveMaze.None: return null;
            case ActiveMaze.Default: return defaultMaze;
            case ActiveMaze.Glitch: return glitchMaze;
        }
        return null;
    }
    public void SetMazeActive(ActiveMaze maze) {
        if (maze != activeMaze) {
            var prevMaze = GetMaze(activeMaze);
            var nextMaze = GetMaze(maze);
            if (prevMaze != null) {
                prevMaze.gameObject.SetActive(false);
            }
            if (nextMaze != null) {
                nextMaze.gameObject.SetActive(true);
            }
            activeMaze = maze;
            bool inGlitchMaze = activeMaze == ActiveMaze.Glitch;
            if (countdownTimerText.gameObject.activeSelf != inGlitchMaze) {
                countdownTimerText.gameObject.SetActive(inGlitchMaze);
            }
            lastMazeSwitchStartTime = Time.time;
        }
    }
    public void SwitchMazes() {
        SetMazeActive(activeMaze == ActiveMaze.Default ? ActiveMaze.Glitch : ActiveMaze.Default);
    }

    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    /// <summary>
    /// Call this function to switch mazes (with a cooldown, ie. this call may fail)
    /// </summary>
    /// <returns></returns>
    public bool TriggerMazeSwitch() {
        if (Time.time > lastMazeSwitchTime + mazeSwitchCooldown) {
            lastMazeSwitchTime = Time.time;
            SwitchMazes();
            return true;
        }
        return false;
    }
    
    void Update() {
        if (inGlitchMaze) {
            // instead of updating maze timer, just apply damage over time
            // 10 damage / sec, default 100 health = 10 seconds, same as we had previously
            player.TakeDamage(10f * Time.deltaTime);
            
            // if (timeInThisMaze >= glitchMazeTimeLimit) {
            //     player.KillPlayer();
            // } else {
            //     var timeRemaining = glitchMazeTimeRemaining + 0.9;
            //     var minutes = (int)(timeRemaining / 60);
            //     var seconds = (int) (timeRemaining % 60);
            //     countdownTimerText.text = "" + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            //
            //     var glitchPercent = glitchPercentRemaining;
            //     // TODO: implement postprocessing effects here
            // }
        }
    }
}
