using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for player abilities <see cref="PlayerAbility"/> + other player controller components,
/// eg. movement, audio controller, etc.
/// </summary>
public abstract class PlayerComponent : MonoBehaviour, IPlayerEventSource {
    /// <summary>
    /// Player reference (<see cref="Player"/>)
    /// </summary>
    [InjectComponent] public Player player;

    /// <summary>
    /// Generic event listener (<see cref="PlayerEvent"/>)
    /// dispatches events like eg. movement started / stopped, abilities used, etc.
    /// </summary>
    public event PlayerEvent.Event OnEvent;

    /// <summary>
    /// Fire an event
    /// </summary>
    /// <see cref="OnEvent"/>
    protected void FireEvent(PlayerEvent.Type eventType) {
        OnEvent?.Invoke(eventType);
    }
}
