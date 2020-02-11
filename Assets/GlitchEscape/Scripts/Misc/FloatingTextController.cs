using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingTextController _instance = null;
    public static FloatingTextController instance
    {
        get { return _instance; }
    }

    private TextMeshProUGUI floatText;
    private GameObject floatPanel;
    private Transform floatTextArea;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            if (_instance == null)
                Debug.LogError("No FloatingTextController instance in this scene!");
        }

        // floatText = this.GetComponents<TextMeshProUGUI>();
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
        if (floatTextArea == targetTransform)
        {
            floatTextArea = null;
            floatPanel.SetActive(false);
        }
    }
}
