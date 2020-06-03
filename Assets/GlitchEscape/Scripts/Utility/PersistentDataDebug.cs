using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersistentDataDebug : MonoBehaviour {
    public TMP_Text debugText;

    void Update() {
        if (debugText) {
            debugText.text = PersistentDataStore.instance.GetSavedObjectDataAsJson();
        }
    }
}
