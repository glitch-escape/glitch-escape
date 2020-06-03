using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingTextController instance { get; private set; }

    [InjectComponent] public TextMeshProUGUI floatText;
    private GameObject floatPanel;
    private Transform floatTextArea;

    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            print("called");
        }
        floatPanel = this.transform.parent.gameObject;
        floatPanel.SetActive(false);
        floatTextArea = null;
    }

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }
    private void OnDisable() {
        instance = null;
    }

    void Update()
    {
        if (floatTextArea != null)
        {
            floatPanel.transform.position = Camera.main.WorldToScreenPoint(floatTextArea.position);
        }
    }

    public void EnableText(Transform targetTransform, string text)
    {
        floatText.text = text;
        floatTextArea = targetTransform;
        floatPanel.SetActive(true);
    }

    public void DisableText(Transform targetTransform)
    {
        // only disable if there is the last in-range interactable item.
        if (IsCurrentTarget(targetTransform))
        {
            floatTextArea = null;
            floatPanel.SetActive(false);
        }
    }

    // check if player's current target is the one stored
    public bool IsCurrentTarget(Transform targetTransform) => floatTextArea == targetTransform;
}
