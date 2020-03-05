using System;
using System.Collections;
using System.Collections.Generic;
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
public abstract class BaseAgent<Derived, Config, Health, Stamina> : MonoBehaviour, IConfigurable<Config>
    where Health : Resource<Derived, Config, float>
    where Stamina : Resource<Derived, Config, float>
    where Derived : class, IConfigurable<Config>
    where Config : ScriptableObject
{
    [SerializeField]
    public Config _config;
    public Config config {
        get { return _config; }
        set { _config = value; }
    }

    public Health health => this.GetEnforcedComponentReference(ref _health);
    private Health _health = null;

    public Stamina stamina => this.GetEnforcedComponentReference(ref _stamina);
    private Stamina _stamina = null;
    
    public delegate void Listener();
    
    /// <summary>
    /// Called when an ability use fails.
    /// </summary>
    public event Listener OnFailedToUseAbility;
 
    /// <summary>
    /// Called when something successfully killed this agent.
    /// </summary>
    public event Listener OnKilled;

    protected void OnEnable() { health.OnDepleted += Kill; }
    protected void OnDisable() { health.OnDepleted -= Kill; }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damage) {
        health.value -= damage;
    }
    
    /// <summary>
    /// Immediately kill + destroy this agent.
    /// May fail (eg. Player, which will use TryKillAgent() to just respawn instead)
    /// </summary>
    public void Kill() {
        if (TryKillAgent()) {
            OnKilled?.Invoke();
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Called when Kill() attempts to kill the agent.
    /// Returning true will continue (and will destroy the agent's object); returning false aborts.
    /// </summary>
    /// <returns></returns>
    protected abstract bool TryKillAgent();
    
    /// <summary>
    /// Attempt to use an ability, with some stamina cost.
    /// Returns true (and decreases stamina) iff the agent has enough stamina, or false otherwise.
    /// Scripts can subscribe to OnFailedToUseAbility to be notified when that occurs.
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool TryUseAbility(float cost) {
        if (stamina.value < cost) {
            OnFailedToUseAbility?.Invoke();
            return false;
        }
        stamina.value -= cost;
        return true;
    }
}
