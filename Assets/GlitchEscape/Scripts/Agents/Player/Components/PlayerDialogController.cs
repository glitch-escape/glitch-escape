﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using UnityEngine.UI;

public class PlayerDialogController : MonoBehaviourWithConfig<DialogConfig>
{
    //[InjectComponent] public Player player;
    
    private IEnumerator coroutineSent;
    
    // These are outside of the player gameObject(in UI part of prefab), 
    // so I'm not sure if InjectComponent works
    private DialogueRunner dr => _dr ?? (_dr = FindObjectOfType<DialogueRunner>());
    private DialogueUI dUI => _dUI ?? (_dUI = FindObjectOfType<DialogueUI>());
    private DialogueRunner _dr;
    private DialogueUI _dUI;

    // Variables for icon
    private string curCharacter;
    private Image icon;

    private string curSpeaker;
    private PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.interact;
    private PlayerControls.HybridButtonControl inputButton2 => PlayerControls.instance.nextDialog;
    private bool beginDialogInput => (inputButton?.wasPressedThisFrame ?? false);
    private bool nextDialogInput => (inputButton?.wasPressedThisFrame ?? false) 
                                    || (inputButton2?.wasPressedThisFrame ?? false) 
                                    || Keyboard.current.spaceKey.wasPressedThisFrame;

    
    private void Awake() {
        if (dUI) dUI.textSpeed = config.textSpeed;
        if (dr)  dr.Add(config.coreText);

        SearchForIcon();
    }
    
    void Update() {
        // Start/Continue dialog if input was pressed with a defined speaker 
        if(!config.isCutscene){ // Don't do this during a cutscene
            if(!dr.IsDialogueRunning) {
                if(beginDialogInput && curSpeaker != null) {
                    dr.StartDialogue(curSpeaker); 
                }
            }
            else if(nextDialogInput) {
                dUI.MarkLineComplete();
            }  
        }

        // Update the textbox portrait based on name of current speaker
        if(dUI.curCharacter != curCharacter) {
            SearchForIcon(); // make sure icon is set
            if(!icon) return;
            for(int i = 0; i < config.portraits.Length; i ++) {
                if(dUI.curCharacter == config.portraits[i].name) {
                    icon.sprite = config.portraits[i].icon;
                    break;
                }
            }
            curCharacter = dUI.curCharacter;
            
        }
    }

    private void SearchForIcon() {
        // Try to find the textbox if the icon hasn't been set yet
        if(!icon) {
            Image i = GameObject.Find("PlayerCameraRig/UI/HUD/InteractFloatPanel/Image").GetComponent<Image>();
            if(i) icon = i;
        }
    }


    /// <summary>
    /// Let other scripts know if the movement should be locked
    /// </summary>
    public bool PreventMovement() {
        return dr.IsDialogueRunning;
    }

    /// <summary>
    /// Set the current node of text that should be played using YarnSpinner
    /// </summary>
    public void SetSpeaker(string dialogNode) {
        this.curSpeaker = dialogNode;
        //print("AAA: " + curSpeaker);
        //print("CAA " + gameObject.name);
        SearchForIcon();
    }

    #region Functions to be called by Dialog Runner or Animation Timeline

    /// <summary>
    /// Begins to display dialog, provided a node of text was given
    /// </summary>
    /// <param name="dialogNode"></param>
    public void BeginDialog(string dialogNode) {
        if (!dr.IsDialogueRunning) {
            dr.StartDialogue(dialogNode);
        }
    }

    /// <summary>
    /// Moves dialog onto next sentence.
    /// </summary>
    public void ContinueDialog() {
        if(dr.IsDialogueRunning) {
            dUI.MarkLineComplete();
        }
    }

    public void WaitToHideText(GameObject text) {
        StartCoroutine(WaitAndHide(text));
    }

    public void SetIcon(Image display) {
        if(config.isCutscene) icon = display;
    }

    IEnumerator WaitAndHide(GameObject text) {
        yield return new WaitForSeconds(config.sentenceDelay);
        text.SetActive(false);
    }
    
    #endregion
}
