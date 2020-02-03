using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Dialog : MonoBehaviour
{
    public Transform floatTextArea;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI staticText;
    public TextMeshProUGUI floatText;
    public string[] sentences;
    public float charSpeed = 0.03f;
    public float sentenceSpeed = 3;

    private string text = "";
    private int index = 0;
    private int dialogType = 0;

    void Start()
    {
        StartCoroutine(DisplayByChar());
    }

    void Update()
    {
        dialogText.text = "";
        staticText.text = "";
        floatText.text = "";
        switch (dialogType)
        {
            case 0:
                dialogText.text = text;
                break;
            case 1:
                staticText.text = text;
                break;
            case 2:
                Vector3 dialogPos = Camera.main.WorldToScreenPoint(floatTextArea.position);
                floatText.transform.position = dialogPos;
                floatText.text = text;
                break;
            default:
                break;
        }
    }

    IEnumerator DisplayByChar()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            text += letter;
            // Line break detection here(not implemented)
            yield return new WaitForSeconds(charSpeed);
        }
        StartCoroutine(DisplayBySent());
    }

    IEnumerator DisplayBySent()
    {
        yield return new WaitForSeconds(sentenceSpeed);
        nextSentence();
    }

    void nextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            text = "";
            StartCoroutine(DisplayByChar());
        }
        else
        {
            text = "";
        }
    }

    public void changeDialogType(int i)
    {
        dialogType = i;
    }

}
