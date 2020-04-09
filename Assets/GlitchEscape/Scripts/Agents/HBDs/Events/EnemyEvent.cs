using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvent : MonoBehaviour {
    /// <summary>
    /// Lists all enemy events that can be emitted by <see cref="PlayerAbility"/> and <see cref="PlayerComponent"/>
    /// </summary>
    public enum Type {
        // TODO: add enemy events, and wire up / use these events
    }

    /// <summary>
    /// Generic event type, used by <see cref="PlayerAbility"/>
    /// </summary>
    /// <param name="eventType"></param>
    public delegate void Event(Type eventType);
}
