using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PlayerDialogController : MonoBehaviourWithConfig<DialogConfig>
{
    [InjectComponent] public Player player;
    
    private IEnumerator coroutineSent;
    // These are outside of the player gameObject(in UI part of prefab), 
    // so I'm not sure if InjectComponent works
    private DialogueRunner dr;
    private DialogueUI dUI;
    
    private void Start() {
        dUI = FindObjectOfType<DialogueUI>();
        if (dUI) {
            dUI.onLineFinishDisplaying.AddListener(WaitForNextLine);
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
        dr.StartDialogue(dialogNode);
    }
    
    private void WaitForNextLine() {
        coroutineSent = DisplayNext();
        StartCoroutine(coroutineSent);
    }

    IEnumerator DisplayNext() {
        yield return new WaitForSeconds(config.sentenceDelay);
        FindObjectOfType<DialogueUI>().MarkLineComplete();
    }
}
