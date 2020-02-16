using System;
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
public enum EnemyBehaviorState {
    /// <summary>
    /// no current active state - will transition to an Idle state
    /// </summary>
    None,
    
    /// <summary>
    /// idle behavior (ie. guarding, patrolling, etc)
    /// </summary>
    Idle,
    
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

public interface IEnemyBehaviorState : IEnemyBehavior {
    void StartAction();
    void EndAction();
    void UpdateAction();
    bool ActionFinished(out EnemyBehaviorState nextAction);
    bool CanActivate(Player player);
}
public interface IEnemyAttackAction : IEnemyBehaviorState { }
public interface IEnemyPursuitAction : IEnemyBehaviorState { }
public interface IEnemySearchForPlayerAction : IEnemyBehaviorState { }
public interface IEnemyIdleAction : IEnemyBehaviorState { }


/// <summary>
/// An AI controller for an enemy object.
///
/// Uses attached IEnemyBehaviors to determine how this agent acts + behaves, and in turn manages + switches between
/// different IEnemyBehaviorStates that represent specific actions that this agent is executing right now.
/// </summary>
[RequireComponent(typeof(IEnemyVisionController))]
[RequireComponent(typeof(IEnemyPursuitAction))]
[RequireComponent(typeof(IEnemyIdleAction))]
[RequireComponent(typeof(IEnemyAttackAction))]
[RequireComponent(typeof(IEnemySearchForPlayerAction))]
public class EnemyController : MonoBehaviour {
    private Enemy enemy;
    private Player player;

    private IEnemyAttackAction[] attackActions;
    private IEnemyPursuitAction[] pursueActions;
    private IEnemySearchForPlayerAction[] searchForPlayerActions;
    private IEnemyIdleAction[] idleActions;
    
    /// <summary>
    /// Called at startup in Awake()
    /// </summary>
    private void InitActionLists() {
        attackActions = GetComponents<IEnemyAttackAction>();
        pursueActions = GetComponents<IEnemyPursuitAction>();
        searchForPlayerActions = GetComponents<IEnemySearchForPlayerAction>();
        idleActions = GetComponents<IEnemyIdleAction>();
    }
    
    public bool isHostileToPlayer = true;
    private IEnemyBehaviorState activeState = null;
    
    #region BehaviorStateProperties
    private EnemyBehaviorState _behaviorState = EnemyBehaviorState.None;
    public bool isIdle =>
        _behaviorState == EnemyBehaviorState.None ||
        _behaviorState == EnemyBehaviorState.Idle;
    public bool isAttackingPlayer =>
        _behaviorState == EnemyBehaviorState.AttackingPlayer;
    public bool isActivelyChasingPlayer =>
        _behaviorState == EnemyBehaviorState.AttackingPlayer;
    public bool isPassivelyChasingPlayer =>
        _behaviorState == EnemyBehaviorState.SearchingForPlayer;
    public bool isChasingOrAttackingPlayer =>
        isActivelyChasingPlayer ||
        isAttackingPlayer ||
        isPassivelyChasingPlayer;
    #endregion
    public void SetState(EnemyBehaviorState state) {
        var prevState = _behaviorState;
        _behaviorState = state;
        if (state != prevState) {
            SetActiveAction(GetActionBehaviorForActionState(state));
        }
    }
    private IEnemyBehaviorState GetActionBehaviorForActionState(EnemyBehaviorState state) {
        switch (state) {
            case EnemyBehaviorState.None: break;
            case EnemyBehaviorState.Idle: {
                foreach (var action in idleActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case EnemyBehaviorState.AttackingPlayer: {
                foreach (var action in attackActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case EnemyBehaviorState.ChasingPlayer: {
                foreach (var action in pursueActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case EnemyBehaviorState.SearchingForPlayer: {
                foreach (var action in searchForPlayerActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        return null;
    }
    private void SetActiveAction(IEnemyBehaviorState action) {
        if (activeState != null) {
            activeState.EndAction();
            activeState = null;
        }
        activeState = action;
        if (activeState != null) {
            activeState.StartAction();
        } else if (action != EnemyBehaviorState.None) {
            Debug.LogWarning(
                "no active state for "+action+" on EnemyController for "+gameObject+
                       ": switching to null (None) state");
            _behaviorState = EnemyBehaviorState.None;
        }
    }

    void Update() {
        if (isChasingOrAttackingPlayer) {
            EnemyBehaviorState _;
            if (activeState != null && isAttackingPlayer && !activeState.ActionFinished(out _)) {
            } else {
                foreach (var attack in attackActions) {
                    if (attack.CanActivate(player)) {
                        _behaviorState = EnemyBehaviorState.AttackingPlayer;
                        SetActiveAction(attack);
                    }
                }
            }
        }
        // Check: is this action finished? if so, automatically pick + start next action
        if (activeState != null) {
            EnemyBehaviorState nextActionType;
            if (activeState.ActionFinished(out nextActionType)) {
                // action finished - contextually activate next action
                SetState(nextActionType);
            } else {
                activeState.UpdateAction();
            }
        } else {
            switch (_behaviorState) {
                case EnemyBehaviorState.None:
                    if (idleActions.Length > 0) {
                        SetState(EnemyBehaviorState.Idle);
                    }
                    break;
                case EnemyBehaviorState.AttackingPlayer:
                    if (attackActions.Length > 0) {
                        SetState(EnemyBehaviorState.AttackingPlayer);
                    }
                    break;
                default: break;
            }
        }
    }
    
    public void OnPlayerDetected(Player player) {
        if (isHostileToPlayer && isIdle) {
            SetState(EnemyBehaviorState.ChasingPlayer);
        }
    }
    public void OnPlayerLost(Player player) {
        if (isHostileToPlayer && !isIdle) {
            SetState(EnemyBehaviorState.SearchingForPlayer);
        }   
    }
    public void OnObjectiveDetected(IEnemyObjectiveMarker objective) { }
    
    private void Awake() {
        // Get all references
        if (!enemy) { Debug.LogError("EnemyController: Enemy reference missing!"); }
        if (!player) { player = GameObject.FindObjectOfType<Player>(); }

        // Setup enemy's controller references
        enemy.controller = this;
        SetupSubControllers(enemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
        
        InitActionLists();
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
