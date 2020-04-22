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
    [HideInInspector] [InjectComponent] public NavMeshAgent navMeshAgent;
    [HideInInspector] [InjectComponent] public Animator animator;
    [HideInInspector] [InjectComponent] public EnemyHealth health;
    [HideInInspector] [InjectComponent] public EnemyStamina stamina;

    // reference to the enemy controller
    [HideInInspector] [InjectComponent] public EnemyController controller;
    [InjectComponent] public EnemyVisionController detection;

    // enemy actions
    [InjectComponent] public EnemyAbility[] idle;
    [InjectComponent] public EnemyAbility[] patrol;
    [InjectComponent] public EnemyAbility[] chase;
    [HideInInspector] [InjectComponent] public EnemyAttackAbility[] attack = new EnemyAttackAbility[3];

    private void Awake() {
        navMeshAgent.speed = config.moveSpeed;
        // Initialize attack data
        EnemyAttackAbility[] attackTemp = new EnemyAttackAbility[config.attacks.Length];
        for (int i = 0; i < attack.Length; i++) {
            //         attack[i] = new EnemyAttackAbility();
           /// attack[i] = gameObject.AddComponent<EnemyAttackAbility>();
            
            attack[i].SetConfigID(i);
            attackTemp[i] = attack[i];
        }
        attack = attackTemp;
    }
}