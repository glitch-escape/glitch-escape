using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentAbility : IResettable {
    /// <summary>
    /// Agent that this ability belongs to
    /// (assumes that <see cref="StartAbility"/> and <see cref="CancelAbility"/> and <see cref="canUseAbility"/>
    /// calls happen through this agent)
    /// </summary>
    IAgent agent { get; }
    
    /// <summary>
    /// Returns true iff this ability is currently active.
    /// <see cref="StartAbility"/>
    /// <seealso cref="CancelAbility"/>
    /// </summary>
    bool isAbilityActive { get; }
    
    /// <summary>
    /// Resource cost of using this ability
    /// Assumed to be >= 0
    /// </summary>
    float resourceCost { get; }
    
    /// <summary>
    /// Should return true iff this ability can be currently used.
    /// Precondition for <see cref="StartAbility"/>
    /// Used by <see cref="BaseAgent{Derived,Config}.TryUseAbility"/>
    /// </summary>
    bool canUseAbility { get; }
    
    /// <summary>
    /// Starts this ability.
    /// Assumes <see cref="canUseAbility"/> returned true; can check this via an enforcement / raise
    /// an exception if this is not true when this is called.
    /// Called by <see cref="BaseAgent{Derived,Config}.TryUseAbility"/> 
    ///
    /// Ability may have already been started, in which case it will be re-started / reset
    /// </summary>
    void StartAbility();

    /// <summary>
    /// Called to cancel (stop) this ability.
    /// Use <see cref="Reset"/> if you want to terminate + fully reset this ability immediately.
    /// </summary>
    void CancelAbility();
}
