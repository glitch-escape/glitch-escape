using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Autowire))]
//[InjectComponent] Maze allMazes;
public class SceneMazeController : MonoBehaviour
{

    [InjectComponent] public NormalMaze normalMaze;
    [InjectComponent] public GlitchMaze glitchMaze;


    private Player player;

    private static SceneMazeController mazesInScene = null;

    public static SceneMazeController MazesInScene
    {
        get {return mazesInScene; }
    }

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
        mazesInScene = this;

        //get player in the scene, inject component doesnt seem to work?
        if (player == null)
        {
            player = (Player)FindObjectOfType(typeof(Player));
        }

        // get maze references
        normalMaze?.gameObject.SetActive(true);
        glitchMaze?.gameObject.SetActive(false);

        // set the default maze to active (and deactivate the glitch maze)
        inNormalMaze = true;
    }

    void OnEnable()
    {
        mazesInScene = this;

        // trigger maze updates, keeping the current maze active
        var maze = currentMaze;
        currentMaze = maze;
        player.OnKilled += OnPlayerRespawn;
    }

    
    void OnDisable()
    {
        mazesInScene = null;

        // clear maze references, in case these objects get destroyed
        normalMaze = null;
        glitchMaze = null;
        player.OnKilled -= OnPlayerRespawn;
    }
   
    void OnPlayerRespawn()
    {
        inNormalMaze = true;
    }
    public void SwitchMazes()
    {
        currentMaze = inGlitchMaze ? (Maze)normalMaze : (Maze)glitchMaze;
    }

    private float lastMazeSwitchTime = -10f;
    [Range(0f, 1f)] public float mazeSwitchCooldown = 0.2f;

    void Update()
    {
        
        if (inGlitchMaze && player.config.isOnMazeTrigger == false)
        {
            // instead of updating maze timer, just apply damage over time
            // 10 damage / sec, default 100 health = 10 seconds, same as we had previously
            player.TakeDamage(10f * Time.deltaTime);
        }
        
    }
}
