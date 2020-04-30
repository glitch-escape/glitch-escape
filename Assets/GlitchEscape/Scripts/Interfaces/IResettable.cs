using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for stuff (typ. monobehaviors) that have resetting functionality
/// (eg. how player respawning works)
/// </summary>
public interface IResettable {
    void Reset();
}
