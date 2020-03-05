using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a monobehavior with a lazy, autobinding .config parameter (eg. PlayerConfig), that's attached to some
/// object, implementing IConfigurable (eg. Player), that actually owns this config instance.
///
/// Basically, this is a convenience class that you can derive from to implement monobehaviors that need
/// access to configuration data that they do not actually own (and that's defined on some other monobehavior
/// and accessed from there using the IConfigurable interface).
/// </summary>
/// <typeparam name="TConfig">The config data (must be a ScriptableObject)</typeparam>
/// <typeparam name="Owner">The class that owns this config instance (and must implement IConfigurable)</typeparam>
public abstract class MonoBehaviourBorrowingConfigFrom<TOwner, TConfig> : MonoBehaviour
    where TOwner : class, IConfigurable<TConfig>
    where TConfig : ScriptableObject
{
    /// used by subclasses to access / read out config data for player, enemies, etc
    protected TConfig config => owner.config;
    
    /// lazy reference to the script that this resource is located on
    private TOwner owner => this.GetEnforcedComponentReference(ref _owner);
    private TOwner _owner = null;
    
    // reset references on script disable; re-get them on re-enable
    protected void OnDisable() { _owner = null; }
}
