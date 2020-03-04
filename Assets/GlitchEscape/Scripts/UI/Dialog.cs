using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Dialog : MonoBehaviour
{
    // Objects
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI staticText;
    public TextMeshProUGUI floatText;
    private GameObject dialogPannel;
    private GameObject staticPannel;
    private GameObject floatPannel;

    // Public Variables
    public float charDelay = 0.03f;
    public float sentenceDelay = 3;
    public const int MAX_DIALOG_EVENT = 2;

    // Private Variables
    private Transform floatTextArea;
    private string text = "";
    private int index = 0;
    private int currEvent = 0;
    private bool targetNearBy = false;
    private List<DialogContent> currDialogEvent;
    private List<DialogContent>[] dialogEvents = new List<DialogContent>[MAX_DIALOG_EVENT];
    private string speaker;
    private bool dialogRunning = false;
    private IEnumerator coroutineChar;
    private IEnumerator coroutineSent;

    void Start()
    {
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

        // For switching scripts, just call SwitchScript(int);
    }

    void Update()
    {
        if (dialogRunning)
        {
            dialogPannel.gameObject.SetActive(false);
            staticPannel.gameObject.SetActive(false);
            floatPannel.gameObject.SetActive(false);
            dialogText.text = "";
            floatText.text = "";
            staticText.text = "";
            switch (speaker)
            {
                case "Player":
                    // Full size dialog only
                    dialogText.text = text;
                    dialogPannel.gameObject.SetActive(true);
                    break;
                case "Bunny":
                    // Side dialog + floating dialog
                    if (targetNearBy)
                    {
                        floatText.text = text;
                        floatPannel.transform.position = Camera.main.WorldToScreenPoint(floatTextArea.position);
                        floatPannel.gameObject.SetActive(true);
                    }
                    else
                    {
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

    IEnumerator DisplayByChar()
    {
        speaker = currDialogEvent[index].name;
        foreach (char letter in currDialogEvent[index].sentence.ToCharArray())
        {
            text += letter;
            // Line break detection here(not implemented)
            yield return new WaitForSeconds(charDelay);
        }
        coroutineSent = DisplayBySent();
        StartCoroutine(coroutineSent);
    }

    IEnumerator DisplayBySent()
    {
        yield return new WaitForSeconds(sentenceDelay);
        NextSentence();
    }

    void NextSentence()
    {
        if (index < currDialogEvent.Count - 1)
        {
            index++;
            text = "";
            coroutineChar = DisplayByChar();
            StartCoroutine(coroutineChar);
        }
        else
        {
            dialogRunning = false;
            text = "";
            dialogPannel.gameObject.SetActive(false);
            staticPannel.gameObject.SetActive(false);
            floatPannel.gameObject.SetActive(false);
        }
    }

    public void CheckPlayerNearBy(bool nearby, int eventNumber)
    {
        if (eventNumber == currEvent)
            targetNearBy = nearby;
    }

    public void SwitchDialogEvent(int eventNumber, Transform newFloatTextArea)
    {
        if (coroutineChar != null)
            StopCoroutine(coroutineChar);
        if (coroutineChar != null)
            StopCoroutine(coroutineSent);
        floatTextArea = newFloatTextArea;
        dialogRunning = true;
        if (eventNumber > MAX_DIALOG_EVENT)
        {
            Debug.Log("Need to increase MAX_DIALOG_EVENT value in Dialog.cs");
        }
        currDialogEvent = dialogEvents[eventNumber];
        index = 0;
        text = "";
        currEvent = eventNumber;
        coroutineChar = DisplayByChar();
        StartCoroutine(coroutineChar);
    }
}
