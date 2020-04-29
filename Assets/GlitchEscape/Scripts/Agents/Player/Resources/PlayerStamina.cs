using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements player stamina and provides mappings from values in PlayerConfig to the generic RegeneratingResource impl.
/// Used by Player, and uses values pulled from PlayerConfig via MonoBehaviorUsingConfig<Player, PlayerConfig>
/// </summary>
public class PlayerStamina : RegeneratingResource<Player, PlayerConfig> {
    public override string name => "stamina";
    public override float defaultValue => config.stamina.maximum;
    public override float minimum => config.stamina.minimum;
    public override float maximum => config.stamina.maximum;
    public override bool regenEnabled => true;
    public override float regenPerSec => config.staminaRegen;
    public override float regenDelay => config.staminaRegenDelay;
    public override AnimationCurve regenCurve => config.staminaRegenCurve;
}
