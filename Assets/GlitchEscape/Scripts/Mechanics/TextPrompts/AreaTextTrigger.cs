using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AreaTextTrigger : MonoBehaviour, IActiveInteract {
     private TMP_Text text;
     private GameObject UI;
     private TMP_Text UIText;
     public string keyboard_message;
     public string controller_message;
     private void Awake()
     {
          text = text ?? Enforcements.GetComponentInChildren<TMP_Text>(this);
          UI = GameObject.Find("PlayerCameraRig/UI/UITextbox");
          UIText = GameObject.Find("PlayerCameraRig/UI/UITextbox").GetComponentInChildren<TMP_Text>();
     }
     private void OnEnable() {
          text.gameObject.SetActive(false);
     }
     // non-interactive
     public void OnInteract(Player player) {}

     public void OnPlayerEnterInteractionRadius(Player player) {
          if (IsControllerInput())
               UIText.text = controller_message;
          else if (IsKeyboardInput())
               UIText.text = keyboard_message;
          UI.gameObject.SetActive(true);
     }
     public void OnPlayerExitInteractionRadius(Player player) {
          UI.gameObject.SetActive(false);
     }
     
     private bool IsControllerInput() {
          if (Gamepad.current != null)
               return true;
          return false;
     }
    
     private bool IsKeyboardInput() {
          if (Keyboard.current != null)
               return true;
          return false;
     }
     public bool isInteractive => false;
     public void OnSelected(Player player) {
     }

     public void OnDeselected(Player player) {
     }
}
