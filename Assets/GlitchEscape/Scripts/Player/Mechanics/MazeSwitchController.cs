using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSwitchController : MonoBehaviour, IPlayerControllerComponent {
    private PlayerController controller;
    private Player player;
    public void SetupControllerComponent(PlayerController controller) {
        this.controller = controller;
        player = controller.player;
        if (!defaultMaze) { Debug.LogError("MazeSwitchController: default Maze missing!"); }
        if (!glitchMaze) { Debug.LogError("MazeSwitchController: glitch Maze missing!"); }
        if (activeMaze == ActiveMaze.None) {
            SetMazeActive(ActiveMaze.Default);
        }
    }
    public Maze defaultMaze;
    public Maze glitchMaze;
    public enum ActiveMaze {
        None,
        Default,
        Glitch
    };
    public ActiveMaze activeMaze;

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
        }
    }
    public void SwitchMazes() {
        SetMazeActive(activeMaze == ActiveMaze.Default ? ActiveMaze.Glitch : ActiveMaze.Default);
    }
}
