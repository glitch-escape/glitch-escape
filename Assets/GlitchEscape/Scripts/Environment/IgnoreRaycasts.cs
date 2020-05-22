using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRaycasts : MonoBehaviour
{
    void Awake()
    {
        gameObject.layer = 2;
    }
}
