using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for the enemy controller<see cref="EnemyController"/>.
/// </summary>
public class EnemyComponent : MonoBehaviour, IEnemyEventSource {
    /// <summary>
    /// Enemy reference (<see cref="Enemy"/>)
    /// </summary>
    [InjectComponent] public Enemy enemy;

    /// <summary>
    /// Generic event listener (<see cref="EnemyEvent"/>)
    /// dispatches events like eg. movement started / stopped, abilities used, etc.
    /// </summary>
    public event EnemyEvent.Event OnEvent;

    /// <summary>
    /// Fire an event
    /// </summary>
    /// <see cref="OnEvent"/>
    protected void FireEvent(EnemyEvent.Type eventType) {
        OnEvent?.Invoke(eventType);
    }
}
