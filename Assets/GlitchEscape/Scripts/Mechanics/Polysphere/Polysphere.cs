using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polysphere
{
    public Color objectiveColor;
    public bool inHand;
    public GameObject keyObject;

    public Polysphere(Color newObjectiveColor, bool newInHand, GameObject newKeyObject)
    {
        objectiveColor = newObjectiveColor;
        inHand = newInHand;
        keyObject = newKeyObject;

    }
}
