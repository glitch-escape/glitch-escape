using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for attack objects (see <see cref="Projectile"/>)
/// </summary>
public abstract class Attack : MonoBehaviour {
    public abstract AgentType targetType { get; }
}
