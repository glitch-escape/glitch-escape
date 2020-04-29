using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentAbility : IResettable {
    /// <summary>
    /// Agent that this ability belongs to
    /// (assumes that <see cref="Internal_StartAbility"/> and <see cref="CancelAbility"/> and <see cref="canUseAbility"/>
    /// calls happen through this agent)
    /// </summary>
    IAgent agent { get; }
    
    /// <summary>
    /// Returns true iff this ability is currently active.
    /// <see cref="Internal_StartAbility"/>
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
    /// Precondition for <see cref="Internal_StartAbility"/>
    /// Used by <see cref="BaseAgent{Derived,Config}.TryUseAbility"/>
    /// </summary>
    bool canUseAbility { get; }

    /// <summary>
    /// Tries to use (start) this ability.
    /// Returns false if ability use failed for any reason.
    /// </summary>
    bool UseAbility();
    
    /// <summary>
    /// Called by <see cref="BaseAgent{Derived,Config}.TryUseAbility"/> to start this ability
    /// </summary>
    void Internal_StartAbility();
    
    /// <summary>
    /// Called to cancel (stop) this ability.
    /// Use <see cref="Reset"/> if you want to terminate + fully reset this ability immediately.
    /// </summary>
    void CancelAbility();
}
