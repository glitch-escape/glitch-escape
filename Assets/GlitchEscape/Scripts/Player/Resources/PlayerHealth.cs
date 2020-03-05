using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements player health and provides mappings from values in PlayerConfig to the generic RegeneratingResource impl.
/// Used by Player, and uses values pulled from PlayerConfig via MonoBehaviorUsingConfig<Player, PlayerConfig>
/// </summary>
public class PlayerHealth : RegeneratingResource<Player, PlayerConfig> {
    public override float defaultValue => config.health.maximum;
    public override float minimum => config.health.minimum;
    public override float maximum => config.health.maximum;
    public override bool regenEnabled => true;
    public override float regenPerSec => config.healthRegen;
    public override float regenDelay => config.healthRegenDelay;
    public override AnimationCurve regenCurve => config.healthRegenCurve;
}
