using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadMainMenu() {
        Loader.Load(Loader.Scene.MainMenu);   
    }
    public void LoadFirstTutorial() {
        Loader.Load(Loader.Scene.Tutorial_01_Movement);
    }
    public void LoadHub() {
        Loader.Load(Loader.Scene.Hub_Level);
    }
    public void LoadExplorationLevel() {
        Loader.Load(Loader.Scene.Vertical_Main_Level);
    }
    public void LoadPlatformingLevel() {
        Loader.Load(Loader.Scene.Vertical_Platforming_Level);
    }
    public void LoadThirdLevel() {
        Loader.Load(Loader.Scene.Third_Level);
    }
}
