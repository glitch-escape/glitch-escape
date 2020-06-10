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
        if (mazeController.inGlitchMaze) {
            var opacity = mazeController.glitchMazeOpacity; 
            var result = EditorGUILayout.Slider(opacity, 0f, 1f);
            if (result != opacity) mazeController.SetMazeGlitchMazeOpacity(result);
            foreach (var obj in GameObject.FindGameObjectsWithTag("GlitchMazePlatform")) {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null) {
                    EditorGUILayout.ObjectField(renderer, typeof(Renderer));
                }
            }
        }
    }
}
