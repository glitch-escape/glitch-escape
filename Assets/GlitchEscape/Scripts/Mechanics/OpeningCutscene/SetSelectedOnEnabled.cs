using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SetSelectedOnEnabled : MonoBehaviour {
    public void OnEnabled() {
        GetComponent<Button>()?.Select();
    }
}
