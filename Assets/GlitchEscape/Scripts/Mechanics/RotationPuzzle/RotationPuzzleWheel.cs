using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotationPuzzleWheel : AInteractiveObject {
    private Player player;
    public PossibleRotation target;
    void OnEnable() {
        player = FindObjectOfType<Player>();
    }
    private void OnDisable() {
        player = null;
    }
    public override void OnInteract(Player player) {
        target?.flickIt();
    }
    public override void OnFocusChanged(bool focused) {}
}
