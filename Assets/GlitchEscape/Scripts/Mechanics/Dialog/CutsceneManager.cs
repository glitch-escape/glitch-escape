using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should only be place in a scene solely dedicated for cutscenes.
/// - Prevents an instance of the PlayCamRig from being active while the cutscene is playing
/// - Allows timeline to decide when the cutscene ends
/// </summary>
public class CutsceneManager : MonoBehaviour {

    private PlayerController player;

    void Awake() {
        player = FindObjectOfType<PlayerController>();
        if(player) { player.gameObject.SetActive(false); }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
