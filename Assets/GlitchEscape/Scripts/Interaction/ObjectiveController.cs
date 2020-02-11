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
    public GameObject key;

    private List<Polysphere> keyColors = new List<Polysphere>();
    private GameObject newKey;

    private int _objectiveCounter = 0;
    public int objectiveCounter => _objectiveCounter;
    private bool _keyInHand = false;
    public bool keyInHand => _keyInHand;

    private TextMeshProUGUI objectiveText;

    void Awake()
    {
        objectiveText = gameObject.GetComponent<TextMeshProUGUI>();

        keyColors.Add(new Polysphere(Color.grey, false, null));
        keyColors.Add(new Polysphere(Color.red, false, null));
        keyColors.Add(new Polysphere(Color.yellow, false, null));
        keyColors.Add(new Polysphere(Color.green, false, null));
        keyColors.Add(new Polysphere(Color.blue, false, null));
        keyColors.Add(new Polysphere(Color.cyan, false, null));
        keyColors.Add(new Polysphere(Color.magenta, false, null));
    }

    public void CountUp()
    {
        _keyInHand = false;
        _objectiveCounter++;
        // update HUD display
        objectiveText.text = _objectiveCounter.ToString();
    }

    public void PickUpKey()
    {
        _keyInHand = true;
    }

    public void UpdateKey(Color inputColor, bool setInHand)
    {
        foreach (var keyColor in keyColors)
        {
            // update each cloned ui object's bool with input bool
            if (keyColor.objectiveColor == inputColor) { keyColor.inHand = setInHand; }
        }
        ShowKey();
    }

    // rerendering key ui
    public void ShowKey()
    {
        var pos = key.transform.position;

        foreach (var keyColor in keyColors)
        {
            Destroy(keyColor.keyObject);
            if (keyColor.inHand)
            {
                newKey = Instantiate(key, pos, Quaternion.identity) as GameObject;
                pos.x += 30f;
                newKey.GetComponent<Image>().color = keyColor.objectiveColor;
                keyColor.keyObject = newKey;
                newKey.transform.SetParent(gameObject.transform);
            }
        }
    }
}
