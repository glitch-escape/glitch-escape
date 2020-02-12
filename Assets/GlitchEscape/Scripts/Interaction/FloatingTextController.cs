using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class FloatingTextController : MonoBehaviour
{
    // singleton
    private static FloatingTextController _instance = null;
    public static FloatingTextController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FloatingTextController>();
                if (_instance == null)
                    Debug.LogError("No FloatingTextController instance in this scene!");
            }
            return _instance;
        }
    }

    private TextMeshProUGUI floatText;
    private GameObject floatPanel;
    private Transform floatTextArea;

    void Awake()
    {
        floatText = gameObject.GetComponent<TextMeshProUGUI>();
        floatPanel = this.transform.parent.gameObject;
        floatPanel.SetActive(false);
        floatTextArea = null;
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
