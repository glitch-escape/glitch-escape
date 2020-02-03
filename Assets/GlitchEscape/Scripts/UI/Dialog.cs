using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public string[] sentences;
    public float charSpeed = 0.03f;
    public float sentSpeed = 3;

    // private bool showNextSentence;
    // private int length;
    private int index;

    void Awake()
    {
        // showNextSentence = true;
    }

    void Start()
    {
        // length = sentences.Length;
        StartCoroutine(DisplayByChar());
    }

    void update()
    {
        // for (int i = 0; i < length; i++)
        // {
        //     if (showNextSentence) { nextSentence(); };
        // }
    }

    IEnumerator DisplayByChar()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(charSpeed);
        }
        StartCoroutine(DisplayBySent());
    }

    IEnumerator DisplayBySent()
    {
        yield return new WaitForSeconds(sentSpeed);
        nextSentence();
    }

    void nextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            dialogText.text = "";
            StartCoroutine(DisplayByChar());
        }
        else
        {
            dialogText.text = "";
        }
    }

}
