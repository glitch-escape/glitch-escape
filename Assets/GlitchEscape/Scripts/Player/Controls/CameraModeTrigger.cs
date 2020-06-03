using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModeTrigger : MonoBehaviour
{
    [InjectComponent] public CameraModeController cameraController;
    [InjectComponent] public Player player;
    public float customHeading;
    public FocusDirection focusDirection;

    public enum FocusDirection
    {
        NorthFocus,
        SouthFocus,
        EastFocus,
        WestFocus,
        CustomFocus,
        DisableFocus,
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<Player>() == player)
        {
            CallNewFocus(focusDirection);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponentInParent<Player>() == player)
        {
            cameraController.ResetFocus();
        }
    }

    void CallNewFocus(FocusDirection direction)
    {
        switch (direction)
        {
            case FocusDirection.NorthFocus:
                cameraController.FixedFocus(0f);
                break;
            case FocusDirection.SouthFocus:
                cameraController.FixedFocus(180f);
                break;
            case FocusDirection.EastFocus:
                cameraController.FixedFocus(90f);
                break;
            case FocusDirection.WestFocus:
                cameraController.FixedFocus(-90f);
                break;
            case FocusDirection.CustomFocus:
                cameraController.FixedFocus(customHeading);
                break;
            case FocusDirection.DisableFocus:
                cameraController.NoHeadingMode();
                break;
        }
    }
}
