using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Yarn.Unity;

public class Dialog : MonoBehaviour {

    // Objects
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI staticText;
    public TextMeshProUGUI floatText;
    private GameObject dialogPannel;
    private GameObject staticPannel;
    private GameObject floatPannel;

    // Public Variables
    public float sentenceDelay = 3;

    // I have no idea how we want this to be set up so these are temporary
    public YarnProgram test;
    public string textNode;
    private DialogueRunner dr;
    private DialogueUI dUI;

    /*
     * Notes:
     * - prob need to make a DialogConfig
     *     - takes in a float for sentence delay
     *     - takes in yarn object that contains all the dialog
     */
    
    private void Start() {
        dialogPannel = dialogText.transform.parent.gameObject;
        staticPannel = staticText.transform.parent.gameObject;
        floatPannel = floatText.transform.parent.gameObject;
        dialogPannel.gameObject.SetActive(false);
        staticPannel.gameObject.SetActive(false);
        floatPannel.gameObject.SetActive(false);

        dr = FindObjectOfType<DialogueRunner>();
        if(dr) dr.Add(test);
        dUI = FindObjectOfType<DialogueUI>();
        if(dUI) dUI.onLineFinishDisplaying.AddListener(WaitForNextLine);
    }

    void Update() {
            // For testing purposes
        if (Input.GetKeyDown(KeyCode.I)) {
            dr.StartDialogue(textNode);
        } 
    }

    /// <summary>
    /// Begins to display dialog, provided a node of text was given
    /// </summary>
    /// <param name="dialogNode"></param>
    public void BeginDialouge(string dialogNode) {
        dr.StartDialogue(dialogNode);
    }

    private void WaitForNextLine() {
        coroutineSent = DisplayNext();
        StartCoroutine(coroutineSent);
    }

    IEnumerator DisplayNext() {
        yield return new WaitForSeconds(sentenceDelay);
        FindObjectOfType<DialogueUI>().MarkLineComplete();
    }

    #region Old Stuff
    public float charDelay = 0.03f;

    public const int MAX_DIALOG_EVENT = 5;

    // Private Variables
    private Transform floatTextArea;
    private string text = "";
    private int index = 0;
    private int currEvent = 0;
    private bool targetNearBy = false;
    private bool isPlayer = false;
    private List<DialogContent> currDialogEvent;
    private List<DialogContent>[] dialogEvents = new List<DialogContent>[MAX_DIALOG_EVENT];
    private string speaker;
    private bool dialogRunning = false;
    private IEnumerator coroutineChar;
    private IEnumerator coroutineSent;
    /*
    void Start() {
        dialogPannel = dialogText.transform.parent.gameObject;
        staticPannel = staticText.transform.parent.gameObject;
        floatPannel = floatText.transform.parent.gameObject;

        // Easy way for now, better way need to build a system, not for Demo.
        // For start:
        dialogEvents[0] = new List<DialogContent>();
        dialogEvents[0].Add(new DialogContent("Player", "What is this place…? It’s hard to even grasp where I am right now."));
        dialogEvents[0].Add(new DialogContent("Bunny", "You have transcended to the astral realm, a place in between higher dimensions when you left your physical body."));
        dialogEvents[0].Add(new DialogContent("Bunny", "I was formed when you entered this space. All I know is that I am here to help you. That is why I was brought here."));
        dialogEvents[0].Add(new DialogContent("Player", "My stuffed bunny is talking to me… I’m trapped in this strange place… I want to know what’s going on!"));
        dialogEvents[0].Add(new DialogContent("Bunny", "Well standing here isn’t going to get us anywhere."));
        dialogEvents[0].Add(new DialogContent("Bunny", "It’s best we get our bearings around here and find out as much as we can about this place. Now hop to it."));
        dialogEvents[0].Add(new DialogContent("Player", "Hrm..."));

        // For polysphere puzzle:
        dialogEvents[1] = new List<DialogContent>();
        dialogEvents[1].Add(new DialogContent("Player", "What’s going on with this thing? It’s completely broken into pieces!"));
        dialogEvents[1].Add(new DialogContent("Bunny", "hm… It seems like some sort of puzzle. The pieces are broken in a very specific sort of way though."));
        dialogEvents[1].Add(new DialogContent("Bunny", "Perhaps if you could piece them together somehow, it will likely form something!"));
        dialogEvents[1].Add(new DialogContent("Player", "The pieces on its own won’t budge…"));
        dialogEvents[1].Add(new DialogContent("Bunny", "Try looking at it differently. Maybe find a different angle to see if that changes anything."));

        // For "Integrity" tank
        dialogEvents[2] = new List<DialogContent>();
        dialogEvents[2].Add(new DialogContent("Player", "My best friends would always tell me that I’m a kind-hearted person."));
        dialogEvents[2].Add(new DialogContent("Player", "They felt that they could trust me. It made me really happy…"));
        dialogEvents[2].Add(new DialogContent("Player", "I’ve always tried to talk honestly to people, but I try to be sensitive at the same time so I don’t hurt their feelings."));
        dialogEvents[2].Add(new DialogContent("Player", "I don’t really talk or stand out that much, but I don’t want to be hated."));
        dialogEvents[2].Add(new DialogContent("Player", "It feels really nice when I do talk to someone. That’s why I want to be as kind and sincere to them as possible."));
        dialogEvents[2].Add(new DialogContent("Player", "When I go back home… I want to try to talk to more people… Slowly but surely… Thank you."));

        // For "Forgiveness" tank
        dialogEvents[3] = new List<DialogContent>();
        dialogEvents[3].Add(new DialogContent("Player", "I’ve never stood out that much, and I don’t have a lot of accomplishments."));
        dialogEvents[3].Add(new DialogContent("Player", "But I don’t like to boast about them. That isn’t me. I just do the best that I can."));
        dialogEvents[3].Add(new DialogContent("Player", "If you live solely based on accomplishments and boast about them, you become more prone to the feeling of failure."));
        dialogEvents[3].Add(new DialogContent("Player", "I think failure can be a good thing too. It can make success feel that much better."));
        dialogEvents[3].Add(new DialogContent("Player", "Even if I don’t stand out that much… I can take pride in the things I can accomplish, but…"));
        dialogEvents[3].Add(new DialogContent("Player", "I can’t let that feeling control me… You understand right? … I know you do."));

        // For "Humility" tank
        dialogEvents[4] = new List<DialogContent>();
        dialogEvents[4].Add(new DialogContent("Player", "I’ve had my fair share of mistakes. Everyone does."));
        dialogEvents[4].Add(new DialogContent("Player", "And I think it’s fine to get angry every once in a while. I know I’ve gotten angry quite a few times…"));
        dialogEvents[4].Add(new DialogContent("Player", "But I don’t want my mistakes and anger to define who I am you know? I’m sure other people feel the same way…"));
        dialogEvents[4].Add(new DialogContent("Player", "I’ve always forgiven people that have done something wrong to me. I couldn’t know what they were thinking or what they’ve been going through."));
        dialogEvents[4].Add(new DialogContent("Player", "I don’t want to be a victim, but I don’t want them to feel like a culprit either."));
        dialogEvents[4].Add(new DialogContent("Player", "No matter what someone has done, forgiving them can help them a lot… Don’t you agree?"));

        // For switching scripts, just call SwitchScript(int);
    }

    void Update() { 
        if (dialogRunning) {
            dialogPannel.gameObject.SetActive(false);
            staticPannel.gameObject.SetActive(false);
            floatPannel.gameObject.SetActive(false);
            dialogText.text = "";
            floatText.text = "";
            staticText.text = "";
            switch (speaker) {
                case "Player":
                    // Full size dialog only
                    dialogText.text = text;
                    dialogPannel.gameObject.SetActive(true);
                    break;
                case "Bunny":
                    // Side dialog + floating dialog
                    if (targetNearBy) {
                        floatText.text = text;
                        floatPannel.transform.position = Camera.main.WorldToScreenPoint(floatTextArea.position);
                        floatPannel.gameObject.SetActive(true);
                    }
                    else {
                        staticText.text = text;
                        staticPannel.gameObject.SetActive(true);
                    }
                    break;
                case "Enemy":
                    // Floating dialog only
                    floatText.text = text;
                    floatPannel.transform.position = Camera.main.WorldToScreenPoint(floatTextArea.position);
                    floatPannel.gameObject.SetActive(true);
                    break;
                case "Other":
                    // What?
                    break;

                default:
                    break;
            }
        }
    }
    */
    IEnumerator DisplayByChar() {
        speaker = currDialogEvent[index].name;
        foreach (char letter in currDialogEvent[index].sentence.ToCharArray()) {
            text += letter;
            // Line break detection here(not implemented)
            yield return new WaitForSeconds(charDelay);
        }
        coroutineSent = DisplayBySent();
        StartCoroutine(coroutineSent);
    }

    IEnumerator DisplayBySent() {
        yield return new WaitForSeconds(sentenceDelay);
        NextSentence();
    }

    void NextSentence() {
        if (index < currDialogEvent.Count - 1) {
            index++;
            text = "";
            coroutineChar = DisplayByChar();
            StartCoroutine(coroutineChar);
        }
        else {
            dialogRunning = false;
            text = "";
            dialogPannel.gameObject.SetActive(false);
            staticPannel.gameObject.SetActive(false);
            floatPannel.gameObject.SetActive(false);
        }
    }

    public void CheckPlayerNearBy(bool nearby, int eventNumber) {
        if (eventNumber == currEvent)
            targetNearBy = nearby;
    }

    public void SwitchDialogEvent(int eventNumber, Transform newFloatTextArea) {
        if (coroutineChar != null)
            StopCoroutine(coroutineChar);
        if (coroutineChar != null)
            StopCoroutine(coroutineSent);
        floatTextArea = newFloatTextArea;
        dialogRunning = true;
        if (eventNumber > MAX_DIALOG_EVENT) {
            Debug.Log("Need to increase MAX_DIALOG_EVENT value in Dialog.cs");
        }
        currDialogEvent = dialogEvents[eventNumber];
        index = 0;
        text = "";
        currEvent = eventNumber;
        coroutineChar = DisplayByChar();
        StartCoroutine(coroutineChar);
    }
    
    #endregion
}
