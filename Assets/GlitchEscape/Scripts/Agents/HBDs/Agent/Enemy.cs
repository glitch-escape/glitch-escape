using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BaseAgent<Enemy, EnemyConfig> {

    #region AgentProperties
    public override AgentType agentType => AgentType.Enemy;
    public override bool isTargetableBy(AgentType type) { return type != agentType; }
    protected override Resource<Enemy, EnemyConfig, float> healthResource => health;
    protected override Resource<Enemy, EnemyConfig, float> staminaResource => stamina;
    protected override KillType killType => KillType.KillAndDestroyGameObject; // Not certain if this is what we want
    #endregion

    // reference to the player (null until player if found using detection script)
    [HideInInspector] public Player player;

    // references to components on the enemy object
    [InjectComponent] public NavMeshAgent navMeshAgent;
    [InjectComponent] public Animator animator;
    [InjectComponent] public EnemyHealth health;
    [InjectComponent] public EnemyStamina stamina;

    // reference to the enemy controller
    [InjectComponent] public EnemyController controller;
    [InjectComponent] public EnemyVisionController detection;

    // enemy actions
    [InjectComponent] public EnemyAbility[] idle;
    [InjectComponent] public EnemyAbility[] patrol;
    [InjectComponent] public EnemyAbility[] chase;
    [InjectComponent] public EnemyAbility[] attack;

    private void Awake() {
        navMeshAgent.speed = config.moveSpeed;
    }
}