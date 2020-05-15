using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaTextTrigger : MonoBehaviour, IActiveInteract
{
     private TMP_Text text;
     public string message;
     private TMP_Text UIText;
     private void Awake()
     {
          text = text ?? Enforcements.GetComponentInChildren<TMP_Text>(this);
          UIText = GameObject.Find("PlayerCameraRig/UI/TutorialUI").GetComponent<TMP_Text>();
     }
     private void OnEnable() {
          UIText.gameObject.SetActive(false);
          text.gameObject.SetActive(false);
     }
     // non-interactive
     public void OnInteract(Player player) {}

     public void OnPlayerEnterInteractionRadius(Player player) {
          UIText.text = text.text;
          UIText.gameObject.SetActive(true);
     }
     public void OnPlayerExitInteractionRadius(Player player) {
          UIText.gameObject.SetActive(false);
     }

     public bool isInteractive => false;
     public void OnSelected(Player player) {
     }

     public void OnDeselected(Player player) {
     }
}
