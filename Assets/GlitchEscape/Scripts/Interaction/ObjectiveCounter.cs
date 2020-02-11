using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ObjectiveCounter : MonoBehaviour
{
    // singleton
    private static ObjectiveCounter _instance = null;
    public static ObjectiveCounter instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ObjectiveCounter>();
                if (_instance == null)
                    Debug.LogError("No ObjectiveCounter instance in this scene!");
            }
            return _instance;
        }
    }

    private int _objectiveCounter = 0;
    public int objectiveCounter => _objectiveCounter;

    private TextMeshProUGUI objectiveText;

    void Awake()
    {
        objectiveText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void CountUp()
    {
        _objectiveCounter++;
        // update HUD display
        objectiveText.text = _objectiveCounter.ToString();
    }
}
