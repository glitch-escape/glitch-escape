using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlTypes;

public class ButtonRotate : IActiveInteract
{
    public Transform transform = this.gameObject.transform;

    public bool isInteractive = true;

    public bool isInteractive { get; }
    public void OnSelected(Player player);
    public void OnDeselected(Player player);

    //passively triggered
    public void OnPlayerEnterInteractionRadius(Player player);
    public void OnPlayerExitInteractionRadius(Player player);

    public void OnInteract(Player player);
}
