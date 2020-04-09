using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

//All interactive cases share
public interface IInteract
{
    Transform transform { get; }
    bool isInteractive { get; }
    void OnSelected(Player player);
    void OnDeselected(Player player);
    //passively triggered
    void OnPlayerEnterInteractionRadius(Player player);
    void OnPlayerExitInteractionRadius(Player player);
}

//actively interacted with
public interface IActiveInteract : IInteract //extends
{
    void OnInteract(Player player);
}