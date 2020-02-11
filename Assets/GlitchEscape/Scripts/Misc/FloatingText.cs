using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI floatText;
    private GameObject floatPannel;
    private Transform floatTextArea;

    void Awake()
    {
        // floatText = this.GetComponents<TextMeshProUGUI>();
        floatText = gameObject.GetComponent<TextMeshProUGUI>();
        floatPannel = this.transform.parent.gameObject;
        floatPannel.SetActive(false);
        floatTextArea = null;
    }

    void Update()
    {
        if (floatTextArea != null)
        {
            floatPannel.transform.position = Camera.main.WorldToScreenPoint(floatTextArea.position);
        }
    }

    public void EnableText(Transform targetTransform, string text)
    {
        floatText.text = text;
        floatTextArea = targetTransform;
        floatPannel.SetActive(true);
    }

    public void DisableText(Transform targetTransform)
    {
        if (floatTextArea == targetTransform)
        {
            floatTextArea = null;
            floatPannel.SetActive(false);
        }
    }
}
