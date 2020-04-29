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

    [SerializeField]
    private Maze _currentMaze = null;
    
    [SerializeField]
    public Maze currentMaze {
        get => _currentMaze;
        set {
            var prevMaze = currentMaze;
            _currentMaze = value;
            prevMaze?.gameObject.SetActive(false);
            _currentMaze?.gameObject.SetActive(true);
            // Debug.Log("switching mazes");
            if (prevMaze != null) {
                lastMazeSwitchTime = Time.time;
                if (value == normalMaze) {
                    FireEvent(PlayerEvent.Type.MazeSwitchToNormalMaze);
                } else if (value == glitchMaze) {
                    FireEvent(PlayerEvent.Type.MazeSwitchToGlitchMaze);
                }
            }
        }
    }
    public enum ActiveMaze {
        Normal,
        Glitch
    };
    public ActiveMaze activeMaze => inNormalMaze ? ActiveMaze.Normal : ActiveMaze.Glitch;
    public bool inNormalMaze {
        get => normalMaze?.gameObject.activeInHierarchy ?? false;
        set => currentMaze = value ? (Maze)normalMaze : glitchMaze;
    }
    public bool inGlitchMaze {
        get => glitchMaze?.gameObject.activeInHierarchy ?? false;
        set => currentMaze = value ? (Maze)glitchMaze : normalMaze;
    }
    private TMaze TryGetMaze<TMaze>(string fallbackName) where TMaze : Maze {
        // use Resources.FindObjectsOfTypeAll<T>() to find inactive game objects (this breaks w/ GameObjects.Find...)
        // (https://answers.unity.com/questions/890636/find-an-inactive-game-object.html)
        var mazes = Resources.FindObjectsOfTypeAll<TMaze>();
        if (mazes.Length > 2) Debug.LogError("Found multiple mazes of type "+typeof(TMaze).Name+"! "+mazes);
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
        // inGlitchMaze = !inGlitchMaze;
        currentMaze = inGlitchMaze ? (Maze)normalMaze : (Maze)glitchMaze;
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
