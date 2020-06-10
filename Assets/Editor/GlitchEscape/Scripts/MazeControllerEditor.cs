using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[CustomEditor(typeof(SceneMazeController))]
public class MazeControllerEditor : Editor {
    private SceneMazeController.ActiveMaze prevActiveMaze;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var mazeController = (SceneMazeController) target;
        if (mazeController.activeMaze != prevActiveMaze) {
            mazeController.SetActiveMaze(prevActiveMaze = mazeController.activeMaze);   
        }
    }
}
