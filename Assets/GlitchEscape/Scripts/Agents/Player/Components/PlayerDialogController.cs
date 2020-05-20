﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerDialogController : MonoBehaviourWithConfig<DialogConfig>
{
    //[InjectComponent] public Player player;
    
    private IEnumerator coroutineSent;
    // These are outside of the player gameObject(in UI part of prefab), 
    // so I'm not sure if InjectComponent works
    private DialogueRunner dr;
    private DialogueUI dUI;

    
    private void Start() {
        dUI = FindObjectOfType<DialogueUI>();
        if (dUI) {
            if(!config.isCutscene) dUI.onLineFinishDisplaying.AddListener(WaitForNextLine);
            dUI.textSpeed = config.textSpeed;
        }
        dr = FindObjectOfType<DialogueRunner>();
        if (dr) dr.Add(config.coreText);
    }
    
    // For testing purposes [NEEDS TO BE REMOVED]
    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            BeginDialog("HDB-Act1");
        } 
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

    private void WaitForNextLine() {
        coroutineSent = DisplayNext();
        StartCoroutine(coroutineSent);
    }

    IEnumerator DisplayNext() {
        yield return new WaitForSeconds(config.sentenceDelay);
        FindObjectOfType<DialogueUI>().MarkLineComplete();
        print(dr);
    }

     IEnumerator WaitAndHide(GameObject text) {
        yield return new WaitForSeconds(config.sentenceDelay);
        text.SetActive(false);
    }

    #region Public Functions for Dialog Runner to call
    /// <summary>
    /// Moves dialog onto next sentence. Function is for cutscene usage
    /// </summary>
    public void ContinueDialog() {
        FindObjectOfType<DialogueUI>().MarkLineComplete();
    }

    public void WaitToHideText(GameObject text) {
        StartCoroutine(WaitAndHide(text));
    }

    public void SetIcon(Sprite display) {
       // print(dr.text);
    }
    #endregion
}
