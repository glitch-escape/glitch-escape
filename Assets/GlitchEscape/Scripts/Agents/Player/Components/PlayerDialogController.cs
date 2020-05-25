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
        if (dUI) {
            //if(!config.isCutscene) dUI.onLineFinishDisplaying.AddListener(WaitForNextLine);
            dUI.textSpeed = config.textSpeed;
        }
        dr = FindObjectOfType<DialogueRunner>();
        if (dr) dr.Add(config.coreText);
    }
    
    // For testing purposes [NEEDS TO BE REMOVED]
    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.L)) {
            ContinueDialog();
            BeginDialog("HDB-Act1");
        } 
        */

        if((inputButton?.wasPressedThisFrame ?? false) && curSpeaker != null) {
            if (!dr.IsDialogueRunning) {
                dr.StartDialogue(curSpeaker);
                // Lock player movement

            }
            else {
                dUI.MarkLineComplete();
            }
        }

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

    public bool PreventMovement() {
        return dr.IsDialogueRunning;
    }

    /// <summary>
    /// Set the current node of text that should be played using YarnSpinner
    /// </summary>
    public void SetSpeaker(string dialogNode) {
        curSpeaker = dialogNode;
    }

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

    #region Public Functions for Dialog Runner to call
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
