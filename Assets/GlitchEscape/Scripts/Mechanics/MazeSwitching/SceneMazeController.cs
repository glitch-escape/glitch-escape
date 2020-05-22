using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    private void OnEnable() { instance = this; }
    private void OnDisable() {
        if (instance == this) {
            instance = null;
        }
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
}
