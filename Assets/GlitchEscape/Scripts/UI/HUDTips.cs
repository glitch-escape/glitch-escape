using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTips : MonoBehaviour
{
    private HUDManager hudManager;

    void Awake()
    {
        hudManager = GetComponent<HUDManager>();
    }

    public void OnControlTips()
    {
        hudManager.controlTips.SetActive(!hudManager.controlTips.activeInHierarchy);
    }
}
