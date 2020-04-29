using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace GlitchEscape.Scripts.DebugUI {
    public class PlayerDebugUI : MonoBehaviour {
        private IPlayerDebug[] debugUIComponents;
        private HashSet<string> activeComponents { get; } = new HashSet<string>();
        public bool visible = false;
        private int selectionIndex = 0;
        
        private void OnEnable() {
            debugUIComponents = GetComponentsInChildren<IPlayerDebug>();
        }
        void Update() {
            if (Gamepad.current?.dpad.left.wasPressedThisFrame ?? false) visible = !visible;
            if (debugUIComponents.Length == 0) return;
            if (selectionIndex >= debugUIComponents.Length) selectionIndex = debugUIComponents.Length - 1;
            if (selectionIndex < 0) selectionIndex = 0;
            int i = selectionIndex;
            var expanded = activeComponents.Contains(debugUIComponents[i].debugName);
            if (Gamepad.current != null) {
                if (Gamepad.current.dpad.right.wasPressedThisFrame) {
                    if (expanded) {
                        activeComponents.Remove(debugUIComponents[i].debugName);
                    }
                    else {
                        activeComponents.Add(debugUIComponents[i].debugName);
                    }
                }

                if (Gamepad.current.dpad.down.wasPressedThisFrame) {
                    var j = i + 1;
                    if (j >= debugUIComponents.Length) j = 0;
                    selectionIndex = j;
                }

                if (Gamepad.current.dpad.up.wasPressedThisFrame) {
                    var j = i - 1;
                    if (j < 0) j = debugUIComponents.Length - 1;
                    selectionIndex = j;
                }
            }
        }

        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI() {
            if (!visible) return;
            GUILayout.Label("debug info: (use dpad to navigate)");
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (var i = 0; i < debugUIComponents.Length; ++i) {
                var selected = i == selectionIndex;
                var expanded = activeComponents.Contains(debugUIComponents[i].debugName);
                if (selected) GUILayout.Button(debugUIComponents[i].debugName);
                else GUILayout.Box(debugUIComponents[i].debugName);
                if (expanded) debugUIComponents[i].DrawDebugUI();
            }
            GUILayout.EndScrollView();
        }
    }
}