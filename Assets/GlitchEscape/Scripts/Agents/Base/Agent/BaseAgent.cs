using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;


/// <summary>
/// Base agent type used as the foundation for Player + Enemy types.
///
/// Implements health + stamina, TakeDamage(), TryUseAbility(), and Kill().
/// Kill() can be aborted by the inheiriting class by returning false from TryKillAgent().
///
/// Implements IConfigurable, ie. ScriptableObject config data associated with this player / enemy, accessible from
/// any reference to this class, and used to
/// 
/// 
/// </summary>
/// <typeparam name="Derived">class that is inheiriting from BaseAgent</typeparam>
/// <typeparam name="Config">Config data class</typeparam>
/// <typeparam name="Health">Health class</typeparam>
/// <typeparam name="Stamina">Stamina class</typeparam>
public abstract class BaseAgent<Derived, Config> : MonoBehaviourWithConfig<Config>, IAgent
    where Derived : class, IConfigurable<Config>
    where Config : ScriptableObject
{
    #region Properties
    
    /// <summary>
    /// The type of this agent
    /// </summary>
    public abstract AgentType agentType { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public abstract bool isTargetableBy(AgentType type);
    
    /// <summary>
    /// List of all abilities. Auto-inject from scripts inheiriting from IAgentAbility
    /// on this gameobject (equiv to GetComponentsInChildren() on Awake()) 
    /// </summary>
    [InjectComponent] public IAgentAbility[] abilities { get; set; }
    
    /// <summary>
    /// abstract reference to the resource (if any) being used for health (takes damage)
    /// if null, this agent cannot be damaged <see cref="TakeDamage"/>, but can still be killed <see cref="Kill"/>
    /// </summary>
    protected abstract Resource<Derived, Config, float> healthResource { get; }
    
    /// <summary>
    /// abstract reference to the resource (if any) being used for stamina
    /// </summary>
    protected abstract Resource<Derived, Config, float> staminaResource { get; }

    public enum KillType {
        KillAndDestroyGameObject,
        KillAndResetAgent,
        Unkillable
    }
    /// <summary>
    /// determines if this agent can be killed (and if so, what the effects of killing this agent are)
    /// </summary>
    protected abstract KillType killType { get; }
    
    #endregion Properties
    
    #region Callbacks
    public delegate void Listener();
    
    /// <summary>
    /// Called when the agent fails to use an ability
    /// </summary>
    public event Listener OnFailedToUseAbilityDueToLowStamina;
    
    /// <summary>
    /// Called when the agent took damage
    /// </summary>
    public event Listener OnDamageTaken;

    /// <summary>
    /// Called when something successfully killed this agent.
    /// </summary>
    public event Listener OnKilled;

    /// <summary>
    /// Called when Reset() is called
    /// </summary>
    public event Listener OnReset;
    
    #endregion Callbacks

    #region Methods
    
    /// <summary>
    /// Resets all components in self or children that have a Reset() method,
    /// and calls OnReset()
    /// </summary>
    public void Reset() {
        var self = (IResettable) this;
        foreach (var component in GetComponentsInChildren<IResettable>()) {
            if (component != self) {
                component.Reset();   
            }
        }
        OnReset?.Invoke();
    }
    
    /// <summary>
    /// Applies damage to this agent; sufficient damage may kill this agent, provided that:
    /// - the agent is not invulnerable (has a <see cref="healthResource"/> that is non-null)
    /// - the agent is not unkillable (has a <see cref="killType"/> that is <see cref="KillType.Unkillable"/>)
    /// When damage is taken calls the <see cref="OnDamageTaken"/> event, and calls <see cref="Kill"/>
    /// if health resource (if present) drops below <see cref="healthResource.minimum"/>
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage) {
        if (healthResource != null) {
            // note: clamping, etc., is handled by Resource
            var currentValue = healthResource.value -= damage;
            OnDamageTaken?.Invoke();
            if (currentValue < healthResource.minimum) {
                Kill();
            }
        }
    }

    /// <summary>
    /// Immediately kill this agent, bypassing damage
    /// </summary>
    public void Kill() {
        switch (killType) {
            case KillType.Unkillable: break;
            case KillType.KillAndResetAgent:
                OnKilled?.Invoke();
                Reset();
                break;
            case KillType.KillAndDestroyGameObject:
                OnKilled?.Invoke();
                Destroy(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Attempt to use (and if so, starts) an ability.
    /// Assumes that the ability belongs to this agent (ie. script is attached to this gameObject,
    /// and the ability is in the <see cref="abilities"/> array)
    /// </summary>
    /// <param name="ability">
    ///    Ability to use. Calls ability.StartAbility()
    ///     iff agent.canUseAbility and the agent can afford ability.resourceCost
    /// </param>
    /// <returns>true iff ability was successful</returns>
    public bool TryUseAbility(IAgentAbility ability) {
        if (ability == null || !ability.canUseAbility) return false;
        if (staminaResource != null) {
            if (ability.resourceCost <= staminaResource.valueRemaining) {
                staminaResource.value -= ability.resourceCost;
            } else {
                OnFailedToUseAbilityDueToLowStamina?.Invoke();
                return false;
            }
        }
        ability.StartAbility();
        return true;
    }
    #endregion Methods
}
