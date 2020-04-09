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

    /// <summary>
    /// Reference to this script's EnemyController (should be parented to this)
    /// </summary>
    public OldEnemyController controller => this.GetEnforcedComponentReferenceInParent(ref m_controller);
    private OldEnemyController m_controller;

    // references to components on the enemy object
    [InjectComponent] public NavMeshAgent navMeshAgent;
    [InjectComponent] public Animator animator;
    [InjectComponent] public EnemyHealth health;
    [InjectComponent] public EnemyStamina stamina;


}