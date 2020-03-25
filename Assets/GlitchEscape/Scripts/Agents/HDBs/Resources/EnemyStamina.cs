using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStamina : RegeneratingResource<Enemy, EnemyConfig> {
    public override string name => "stamina";
    public override float defaultValue => config.stamina.maximum;
    public override float minimum => config.stamina.minimum;
    public override float maximum => config.stamina.maximum;
    public override bool regenEnabled => true;
    public override float regenPerSec => config.staminaRegen;
    public override float regenDelay => config.staminaRegenDelay;
    public override AnimationCurve regenCurve => config.staminaRegenCurve;
}
