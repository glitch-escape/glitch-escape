using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

// Interface that all enemy controller subcomponents must implement
public interface IEnemyControllerComponent {
    void SetupControllerComponent(EnemyController controller);
}

public interface IEnemyBehavior : IEnemyControllerComponent {
    
}
public interface IEnemyIdleBehavior : IEnemyBehavior {}
public interface IEnemyChaseBehavior : IEnemyBehavior {}
public interface IEnemyAttackBehavior : IEnemyBehavior {}

public interface IEnemyObjectiveMarker {}

public interface IEnemyVisionController : IEnemyControllerComponent {
    bool CanSeePlayer();
    bool HasLastKnownPlayerPosition(out Vector3 lastPosition);
    void SetVisionDistanceFactor(float factor);
    void DebugShowDetectionRadius(bool enabled);
    List<IEnemyObjectiveMarker> GetKnownObjectiveMarkers();
    void ClearKnownObjectiveMarkers();
}

/// <summary>
/// All AI behavior states
/// </summary>
enum EnemyBehaviorState {
    /// <summary>
    /// no current active state - will transition to an Idle state
    /// </summary>
    None,
    
    /// <summary>
    /// idle w/ a currently active task (ie. guarding, patrolling, etc)
    /// </summary>
    IdleWithTask,
    
    /// <summary>
    /// idle w/out a currently active task - should search for a task to return to
    /// </summary>
    IdleUntasked,
    
    /// <summary>
    /// spotted / currently chasing the player
    /// </summary>
    ChasingPlayer,
    
    /// <summary>
    /// currently attacking the player
    /// </summary>
    AttackingPlayer,
    
    /// <summary>
    /// lost sight of the player - attempt to find the player, or return to a previously active task
    /// </summary>
    SearchingForPlayer,
}

public interface IEnemyBehaviorState {
    
}

/// <summary>
/// An AI controller for an enemy object.
///
/// Uses attached IEnemyBehaviors to determine how this agent acts + behaves, and in turn manages + switches between
/// different IEnemyBehaviorStates that represent specific actions that this agent is executing right now.
/// </summary>
public class EnemyController : MonoBehaviour {
    
    public Enemy enemy;
    public Player player;

    public void OnPlayerDetected(Player player) { }
    public void OnPlayerLost(Player player) { }
    public void OnObjectiveDetected(IEnemyObjectiveMarker objective) { }
    
    private void Awake() {
        // Get all references
        if (!enemy) { Debug.LogError("EnemyController: Enemy reference missing!"); }
        if (!player) { player = GameObject.FindObjectOfType<Player>(); }

        // Setup enemy's controller references
        enemy.controller = this;
        SetupSubControllers(enemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
    }

    private void OnEnable() {
        SetupSubControllers(enemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
    }

    private void SetupSubControllers(IEnemyControllerComponent[] components) {
        foreach (var component in components) {
            component.SetupControllerComponent(this);
        }
    }
}
