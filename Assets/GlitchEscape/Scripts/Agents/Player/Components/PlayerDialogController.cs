using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class PlayerDialogController : MonoBehaviourWithConfig<DialogConfig>
{
    //[InjectComponent] public Player player;
    
    private IEnumerator coroutineSent;
    // These are outside of the player gameObject(in UI part of prefab), 
    // so I'm not sure if InjectComponent works
    private DialogueRunner dr;
    private DialogueUI dUI;

    // Variables for icon
    private string curCharacter;
    private Image icon;

    private string curSpeaker;
    private PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.interact;

    
    private void Start() {
        dUI = FindObjectOfType<DialogueUI>();
        dr = FindObjectOfType<DialogueRunner>();
        if (dUI) dUI.textSpeed = config.textSpeed;
        if (dr)  dr.Add(config.coreText);
    }
    
    void Update() {
        // Start/Continue dialog if input was pressed with a defined speaker
        if((inputButton?.wasPressedThisFrame ?? false) && curSpeaker != null) {
            if (!dr.IsDialogueRunning)  dr.StartDialogue(curSpeaker);
            else                        dUI.MarkLineComplete();
        }

        // Update the textbox portrait based on name of current speaker
        if(icon && dUI.curCharacter != curCharacter) {
            for(int i = 0; i < config.portraits.Length; i ++) {
                if(dUI.curCharacter == config.portraits[i].name) {
                    icon.sprite = config.portraits[i].icon;
                    break;
                }
            }
            curCharacter = dUI.curCharacter;
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
        curSpeaker = dialogNode;
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
       icon = display;
    }

    IEnumerator WaitAndHide(GameObject text) {
        yield return new WaitForSeconds(config.sentenceDelay);
        text.SetActive(false);
    }
    
    #endregion
}
