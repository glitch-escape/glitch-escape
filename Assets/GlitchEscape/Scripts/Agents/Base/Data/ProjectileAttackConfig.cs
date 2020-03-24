using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ProjectileAttackConfig", menuName = "GameConfigs/ProjectileAttackConfig", order = 1)]
public class ProjectileAttackConfig : BaseAttackConfig {
    public OldEnemyProjectile oldEnemyProjectile;
    public float range = 30f;
}
