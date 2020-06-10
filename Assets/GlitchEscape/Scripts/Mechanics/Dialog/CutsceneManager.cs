using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should only be place in a scene solely dedicated for cutscenes.
/// - Prevents an instance of the PlayCamRig from being active while the cutscene is playing
/// - Allows timeline to decide when the cutscene ends and load different levels
/// </summary>
public class CutsceneManager : MonoBehaviour {

    private PlayerController playerController;
    public Loader.Scene levelToLoad = Loader.Scene.MainMenu;
    public Loader.Scene endCutsceneToLoad = Loader.Scene.MainMenu;
    public Virtue[] endingVirtuesRequired;

    void Awake() {
        playerController = FindObjectOfType<PlayerController>();
        if(playerController) playerController.gameObject.SetActive(false); 
    }

    public void EndScene() {
        if(playerController) { 
            playerController.gameObject.SetActive(true); 
            /*
            bool hasAllVirtues = true;
            for(int i = 0; i < endingVirtuesRequired.Length; i++) {
                if(!playerController.player.fragments.IsVirtueCompleted(endingVirtuesRequired[i])) {
                    hasAllVirtues = false;
                    break;
                }
            }
            */
            if(endCutsceneToLoad != Loader.Scene.None && playerController.player.fragments.HasCompletedAllVirtues) {
                Application.LoadLevel(endCutsceneToLoad.ToString());
                return;
            }
        }
        
        if (levelToLoad != Loader.Scene.None) {
            Application.LoadLevel(levelToLoad.ToString());
        }
    }

    // Update is called once per frame
    void Update() {
        
        if(Input.GetKeyDown(KeyCode.Escape)) {
            EndScene();
        }
        
    }
}
