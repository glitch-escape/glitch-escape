using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotationPuzzleWheel : AInteractiveObject {
    public RotationPuzzleRotator target;
    public override void OnInteract(Player player) {
        target?.StartStopRotation();
    }
    public override void OnFocusChanged(bool focused) {}
}
