using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneMazeController : MonoBehaviour
{

    [InjectComponent] public NormalMaze normalMaze;
    [InjectComponent] public GlitchMaze glitchMaze;
    
    public static SceneMazeController instance { get; private set; } = null;

    [SerializeField]
    private Maze _currentMaze = null;

    
    [SerializeField]
    public Maze currentMaze
    {
        get => _currentMaze;
        set
        {
            var prevMaze = currentMaze;
            _currentMaze = value;
            prevMaze?.gameObject.SetActive(false);
            _currentMaze?.gameObject.SetActive(true);
        }
    }
    
    public enum ActiveMaze
    {
        Normal,
        Glitch
    };
    public ActiveMaze activeMaze => inNormalMaze ? ActiveMaze.Normal : ActiveMaze.Glitch;
    public bool inNormalMaze
    {
        get => normalMaze?.gameObject.activeInHierarchy ?? false;
        set => currentMaze = value ? (Maze)normalMaze : glitchMaze;
    }
    public bool inGlitchMaze
    {
        get => glitchMaze?.gameObject.activeInHierarchy ?? false;
        set => currentMaze = value ? (Maze)glitchMaze : normalMaze;
    }
    
    void Awake()
    {
        instance = this;

        // get maze references
        normalMaze?.gameObject.SetActive(true);
        glitchMaze?.gameObject.SetActive(false);

        // set the default maze to active (and deactivate the glitch maze)
        inNormalMaze = true;
    }

    void OnEnable()
    {
        instance = this;

        // trigger maze updates, keeping the current maze active
        var maze = currentMaze;
        currentMaze = maze;
    }

    
    void OnDisable()
    {
        instance = null;
        // clear maze references, in case these objects get destroyed
        normalMaze = null;
        glitchMaze = null;
    }
    public void ResetMaze() {
        inNormalMaze = true;
    }
    public void SwitchMazes()
    {
        currentMaze = inGlitchMaze ? (Maze)normalMaze : (Maze)glitchMaze;
    }

    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    
}
