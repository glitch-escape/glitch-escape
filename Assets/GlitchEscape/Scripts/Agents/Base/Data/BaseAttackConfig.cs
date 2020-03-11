using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackConfig : ScriptableObject {
    public float damage = 10f;
    public float speed = 30f;
    public float staminaCost = 5f;
    public float attacksPerSec = 2f;
}
