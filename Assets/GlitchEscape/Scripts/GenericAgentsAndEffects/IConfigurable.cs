using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for a monobehavior that has configuration data associated with it in a ScriptableObject.
/// ie. Player : IConfigurable<PlayerData>, SomeEnemy : IConfigurable<SomeEnemyData>
///
/// scripts that need to access this config (owned by another script) can do so using
///     MonoBehaviorUsingConfig<Owner, Config>
///         where Owner : IConfigurable<Config>
/// </summary>
public interface IConfigurable <out TConfigData> where TConfigData : ScriptableObject {
    TConfigData config { get; }
}
