using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaTextTrigger : MonoBehaviour, IActiveInteract
{
     public TMP_Text text;
     public string message;
     private void Awake() {
     }
     private void OnEnable() {
          text.text = message;
          text.gameObject.SetActive(false);
     }

     // non-interactive
     public void OnInteract(Player player) {}

     public void OnPlayerEnterInteractionRadius(Player player) {
          text.text = message;
          text.gameObject.SetActive(true);
     }
     public void OnPlayerExitInteractionRadius(Player player) {
          text.gameObject.SetActive(false);
     }

     public bool isInteractive => false;
     public void OnSelected(Player player) {
     }

     public void OnDeselected(Player player) {
     }
}
