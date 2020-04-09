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

    // references to components on the enemy object
    [InjectComponent] public NavMeshAgent navMeshAgent;
    [InjectComponent] public Animator animator;
    [InjectComponent] public EnemyHealth health;
    [InjectComponent] public EnemyStamina stamina;

    // reference to the enemy controller
    [InjectComponent] public EnemyController controller;
    // Add vision controller here?
    // add pursuit and patrol components here

    // enemy actions
    [InjectComponent] public EnemyPatrolAction[] patrol = new EnemyPatrolAction[1];
    /*
     * AttackAction
     * PursuitAction
     * PatrolAction
     */

}