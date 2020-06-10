using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// Manages maze switching etc.
// TODO: add postprocessing effects to glitch maze (see inGlitchMaze and glitchPercentRemaining)
// TODO: move actual maze switch component of this to a MazeController script w/ [InjectComponent] child refs
// and rename this (which should have a much smaller impl) to PlayerMazeSwitchAbility, or something
// (or just roll into player interaction ability...)
public class PlayerMazeController : PlayerComponent, IPlayerDebug {
    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    /// <summary>
    /// Tells us whether we're currently on a maze switch or not
    /// Set by SetMazeSwitch() and ClearMazeSwitch(); also cleared in OnDisable() and OnLevelTransition()
    /// </summary>
    private MazeSwitch activeMazeSwitch = null;
    private bool onMazeSwitch => activeMazeSwitch != null;

    /// <summary>
    /// Called by MazeSwitch.OnFocusChanged(true)
    /// </summary>
    public void SetMazeSwitch(MazeSwitch mazeSwitch) {
        activeMazeSwitch = mazeSwitch;
    }
    
    /// <summary>
    /// Called by MazeSwitch.OnFocusChanged(false)
    /// </summary>
    public void ClearMazeSwitch(MazeSwitch mazeSwitch = null) {
        if (mazeSwitch == null || activeMazeSwitch == mazeSwitch) {
            activeMazeSwitch = null;
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
        SceneManager.sceneLoaded += OnSceneLoad;
        mazeOpacityController.ResetAndSetupMaterials();
    }
    private void OnDisable() {
        player.OnKilled -= OnPlayerRespawn;
        SceneManager.sceneLoaded -= OnSceneLoad;
        ClearMazeSwitch();
        mazeOpacityController.ClearMaterials();
    }
    private void OnPlayerRespawn() {
        SceneMazeController.instance?.ResetMaze();
    }
    void Update() {
        bool inGlitchMaze = SceneMazeController.instance?.inGlitchMaze ?? false;
        if (!onMazeSwitch && inGlitchMaze) {
            // instead of updating maze timer, just apply damage over time
            // 10 damage / sec, default 100 health = 10 seconds, same as we had previously
            player.TakeDamage(10f * Time.deltaTime);
        }

        // apply maze fade out
        if (inGlitchMaze) {
            var playerHealth = player.health.value / player.health.maximum;
            mazeOpacityController.SetOpacity(playerHealth);
        }
    }
    public class MazeOpacityController {
        private List<Material> activeGlitchMazeMaterials { get; } = new List<Material>();
        public float opacity { get; private set; } = 1f;
        private const string MAZE_OPACITY = "Vector1_62D5110A";
        public void ResetAndSetupMaterials() {
            activeGlitchMazeMaterials.Clear();
            foreach (var platform in GameObject.FindGameObjectsWithTag("GlitchMazePlatform")) {
                var renderer = platform.GetComponent<Renderer>();
                if (renderer != null) { 
                    activeGlitchMazeMaterials.Add(renderer.material);
                }
            }
            opacity = 0f;
            SetOpacity(1f);
        }
        public void ClearMaterials() {
            activeGlitchMazeMaterials.Clear();
        }
        public void SetOpacity(float opacity) {
            if (opacity == this.opacity) return;
            this.opacity = opacity;
            foreach (var material in activeGlitchMazeMaterials) {
                material.SetFloat(MAZE_OPACITY, opacity);
            }
        }
    }
    public MazeOpacityController mazeOpacityController { get; } = new MazeOpacityController();
    
    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode) {
        mazeOpacityController.ResetAndSetupMaterials();
    }
    
    public void DrawDebugUI() {
        GUILayout.Label("active maze switch: "+activeMazeSwitch);
        GUILayout.Label("on maze switch? "+onMazeSwitch);
        GUILayout.Label("mazes present? "+(SceneMazeController.instance != null));
        GUILayout.Label("in glitch maze? "+(SceneMazeController.instance?.inGlitchMaze ?? false));
    }
    public string debugName => this.GetType().Name;
}
