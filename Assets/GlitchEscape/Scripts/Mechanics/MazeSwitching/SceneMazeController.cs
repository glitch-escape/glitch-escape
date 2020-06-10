using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneMazeController : MonoBehaviour {
    public static SceneMazeController instance { get; private set; } = null;
    public enum ActiveMaze { Normal, Glitch };
    
    [InjectComponent] public NormalMaze normalMaze;
    [InjectComponent] public GlitchMaze glitchMaze;

    public ActiveMaze activeMaze = ActiveMaze.Normal;
    public event System.Action<ActiveMaze> OnMazeSwitch;

    public bool inGlitchMaze => activeMaze == ActiveMaze.Glitch;
    public bool inNormalMaze => activeMaze == ActiveMaze.Normal;

    void Awake() {
        instance = this;
        SetActiveMaze(activeMaze);
    }

    private void OnEnable() {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoad;
        mazeOpacityController.ResetAndSetupMaterials();
    }
    private void OnDisable() {
        if (instance == this) {
            instance = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoad;
        mazeOpacityController.ClearMaterials();
    }
    
    public void SetActiveMaze(ActiveMaze maze) {
        var mazeChanged = maze != activeMaze;
        switch (activeMaze = maze) {
            case ActiveMaze.Normal:
                normalMaze?.gameObject.SetActive(true);
                glitchMaze?.gameObject.SetActive(false);
                break;
            case ActiveMaze.Glitch:
                normalMaze?.gameObject.SetActive(false);
                glitchMaze?.gameObject.SetActive(true);
                break;
        }
        if (mazeChanged) {
            OnMazeSwitch?.Invoke(activeMaze);
        }
    }
    public ActiveMaze SwitchMazes() {
        switch (activeMaze) {
            case ActiveMaze.Glitch: SetActiveMaze(ActiveMaze.Normal); return ActiveMaze.Normal;
            case ActiveMaze.Normal: SetActiveMaze(ActiveMaze.Glitch); return ActiveMaze.Glitch;
        }
        return ActiveMaze.Normal;
    }
    public void ResetMaze() {
        SetActiveMaze(ActiveMaze.Normal);
    }

    private const string MAZE_OPACITY = "Vector1_62D5110A";
    public float glitchMazeOpacity => Player.instance?.maze.glitchMazeOpacity ?? 1f;
    public void SetMazeGlitchMazeOpacity(float opacity) {
        Player.instance?.maze.SetMazeGlitchMazeOpacity(opacity);
    }
    private class MazeOpacityController {
        private HashSet<Material> activeGlitchMazeMaterials { get; } = new HashSet<Material>();
        public float opacity { get; private set; } = 1f;
        
        public void ResetAndSetupMaterials() {
            activeGlitchMazeMaterials.Clear();
            foreach (var platform in GameObject.FindGameObjectsWithTag("GlitchMazePlatform")) {
                var renderer = platform.GetComponent<Renderer>();
                if (renderer != null) { 
                    activeGlitchMazeMaterials.Add(renderer.sharedMaterial);
                }
            }
            opacity = 0f;
            SetOpacity(1f);
        }
        public void ClearMaterials() {
            activeGlitchMazeMaterials.Clear();
        }
        public void SetOpacity(float opacity) {
            // if (opacity == this.opacity) return;
            this.opacity = opacity;
            foreach (var material in activeGlitchMazeMaterials) {
                material.SetFloat(MAZE_OPACITY, opacity);
            }
        }
    }
    private MazeOpacityController mazeOpacityController { get; } = new MazeOpacityController();
    
    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode) {
        mazeOpacityController.ResetAndSetupMaterials();
    }
}
