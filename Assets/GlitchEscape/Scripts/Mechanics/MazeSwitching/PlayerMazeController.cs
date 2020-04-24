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
    private NormalMaze normalMaze;
    private GlitchMaze glitchMaze;

    [NonSerialized]
    private static PlayerMazeController _instance = null;

    public static PlayerMazeController instance =>
        _instance ?? (_instance = Enforcements.GetSingleComponentInScene<PlayerMazeController>());

    [NonSerialized]
    private Maze _currentMaze = null;
    
    private Maze currentMaze {
        get => _currentMaze;
        set {
            if (_currentMaze != value) {
                if (_currentMaze != null) {
                    lastMazeSwitchTime = Time.time;
                    if (value == normalMaze) {
                        FireEvent(PlayerEvent.Type.MazeSwitchToNormalMaze);
                    } else if (value == glitchMaze) {
                        FireEvent(PlayerEvent.Type.MazeSwitchToGlitchMaze);
                    }
                }
                _currentMaze?.gameObject.SetActive(false);
                value?.gameObject.SetActive(true);
                _currentMaze = value;
                
            }
        }
    }
    public enum ActiveMaze {
        Normal,
        Glitch
    };
    
    [SerializeField]
    private ActiveMaze _activeMaze = ActiveMaze.Normal;
    
    public ActiveMaze activeMaze {
        get => _activeMaze;
        set {
            if (normalMaze == null || glitchMaze == null) {
                Debug.Log("cannot maze switch: missing normal and/or glitch maze!");
                return;
            }
            switch (_activeMaze) {
                case ActiveMaze.Normal: currentMaze = (Maze) normalMaze;
                    break;
                case ActiveMaze.Glitch: currentMaze = (Maze) glitchMaze;
                    break;
            }
        }
    }
    public bool inNormalMaze {
        get => activeMaze == ActiveMaze.Normal;
        set => activeMaze = ActiveMaze.Normal;
    }
    public bool inGlitchMaze {
        get => activeMaze == ActiveMaze.Glitch;
        set => activeMaze = ActiveMaze.Glitch;
    }
    private TMaze TryGetMaze<TMaze>(string fallbackName) where TMaze : Maze {
        // use Resources.FindObjectsOfTypeAll<T>() to find inactive game objects (this breaks w/ GameObjects.Find...)
        // (https://answers.unity.com/questions/890636/find-an-inactive-game-object.html)
        var mazes = Resources.FindObjectsOfTypeAll<TMaze>();
        if (mazes.Length > 1) Debug.LogError("Found multiple mazes of type "+typeof(TMaze).Name+"!");
        if (mazes.Length > 0) return mazes[0];
        var baseMazes = Resources.FindObjectsOfTypeAll<Maze>();
        foreach (var maze in baseMazes) {
            if (maze.gameObject.name == fallbackName) {
                Debug.Log("Found "+fallbackName+" maze with legacy maze type; replacing with "+
                          typeof(TMaze).Name);
                var m = maze.gameObject;
                Destroy(maze); // remove regular maze component
                return m.AddComponent<TMaze>();
            }
        }
        Debug.LogWarning("No object with "+typeof(TMaze).Name+" component or with name '"+fallbackName+
                         "' with Maze component found in this scene (note: this will effectively disable maze switching)");
        return null;
    }

    private void TryGetMazeRefs() {
        normalMaze = TryGetMaze<NormalMaze>("BazeMaze");
        glitchMaze = TryGetMaze<GlitchMaze>("GlitchMaze");
    }
    void Awake() {
        _instance = this;
        
        // get maze references
        TryGetMazeRefs();
        normalMaze?.gameObject.SetActive(true);
        glitchMaze?.gameObject.SetActive(false);
        
        // set the default maze to active (and deactivate the glitch maze)
        inNormalMaze = true;
    }

    void OnEnable() {
        _instance = this;
        
        // re-get maze references
        TryGetMazeRefs();
        
        // trigger maze updates, keeping the current maze active
        var maze = currentMaze;
        currentMaze = maze;
        player.OnKilled += OnPlayerRespawn;
    }

    void OnDisable() {
        _instance = null;
        
        // clear maze references, in case these objects get destroyed
        normalMaze = null;
        glitchMaze = null;
        player.OnKilled -= OnPlayerRespawn;
    }
    void OnPlayerRespawn() {
        inNormalMaze = true;
    }
    public void SwitchMazes() {
        inGlitchMaze = !inGlitchMaze;
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
        }
    }
}
