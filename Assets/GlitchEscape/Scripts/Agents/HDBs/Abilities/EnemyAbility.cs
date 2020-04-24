using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemy action(movement + attacks).
///
/// Includes variables for stamina costs, ability durations, and variable ability strength
/// (any / all of which can, optionally, be based on variable press durations w/ a designer driven animation curve)
/// </summary>
public abstract class EnemyAbility : BaseAbility, IEnemyEventSource {
    protected abstract bool hasSetDuration { get; }

    public abstract bool AbilityFinished(out EnemyBehaviorState nextAction);

    /// <summary>
    /// Enemy reference (<see cref="Enemy"/>)
    /// </summary>
    [InjectComponent] public Enemy enemy;
    public override IAgent agent => enemy;

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

    protected override void Update() {
        if (hasSetDuration) {
            base.Update();
        }
        else if(isAbilityActive) {
            OnAbilityUpdate();
        }
    }
}
