using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : RegeneratingResource<Enemy, EnemyConfig> {
    public override string name => "health";
    public override float defaultValue => config.health.maximum;
    public override float minimum => config.health.minimum;
    public override float maximum => config.health.maximum;
    public override bool regenEnabled => false; // not certain if this is what we want
    public override float regenPerSec => config.healthRegen;
    public override float regenDelay => config.healthRegenDelay;
    public override AnimationCurve regenCurve => config.healthRegenCurve;
}