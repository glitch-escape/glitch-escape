using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ObjectiveController : MonoBehaviour
{
    // singleton
    private static ObjectiveController _instance = null;
    public static ObjectiveController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ObjectiveController>();
                if (_instance == null)
                    Debug.LogError("No ObjectiveController instance in this scene!");
            }
            return _instance;
        }
    }

    public bool multiPickUp = false;
    public GameObject keyIcon;

    private int _objectiveCounter = 0;
    public int objectiveCounter => _objectiveCounter;
    private bool _keyInHand = false;
    public bool keyInHand => _keyInHand;

    private TextMeshProUGUI objectiveText;

    void Awake()
    {
        objectiveText = gameObject.GetComponent<TextMeshProUGUI>();
        keyIcon.SetActive(false);
    }

    public void CountUp()
    {
        _keyInHand = false;
        keyIcon.SetActive(false);
        _objectiveCounter++;
        // update HUD display
        objectiveText.text = _objectiveCounter.ToString();
    }

    public void PickUpKey()
    {
        _keyInHand = true;
    }

    public void ShowKey(Color newColor)
    {
        keyIcon.SetActive(true);
        keyIcon.GetComponent<Image>().color = newColor;
    }
}
