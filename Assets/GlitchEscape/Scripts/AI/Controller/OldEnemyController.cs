using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

// Interface that all enemy controller subcomponents must implement
public interface IEnemyControllerComponent {
    void SetupControllerComponent(OldEnemyController controller);
}
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
public enum OldEnemyBehaviorState {
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

public interface IEnemyBehaviorState : IEnemyControllerComponent {
    void StartAction();
    void EndAction();
    void UpdateAction();
    bool ActionFinished(out OldEnemyBehaviorState nextAction);
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
//[RequireComponent(typeof(IEnemyVisionController))]
[RequireComponent(typeof(IEnemyPursuitAction))]
[RequireComponent(typeof(IEnemyIdleAction))]
[RequireComponent(typeof(IEnemyAttackAction))]
[RequireComponent(typeof(IEnemySearchForPlayerAction))]
public class OldEnemyController : MonoBehaviour {
    public OldEnemy oldEnemy;
    public Player player;

    private IEnemyAttackAction[] attackActions;
    private IEnemyPursuitAction[] pursueActions;
    private IEnemySearchForPlayerAction[] searchForPlayerActions;
    private IEnemyIdleAction[] idleActions;
    
    /// <summary>
    /// Called at startup in Awake()
    /// </summary>
    private void InitActionLists() {
        attackActions = GetComponentsInChildren<IEnemyAttackAction>();
        pursueActions = GetComponents<IEnemyPursuitAction>();
        searchForPlayerActions = GetComponents<IEnemySearchForPlayerAction>();
        idleActions = GetComponents<IEnemyIdleAction>();
    }
    
    public bool isHostileToPlayer = true;
    private IEnemyBehaviorState activeState = null;
    public OldEnemyBehaviorState behaviorState => _behaviorState;
    
    #region BehaviorStateProperties
    private OldEnemyBehaviorState _behaviorState = OldEnemyBehaviorState.None;
    public bool isIdle =>
        _behaviorState == OldEnemyBehaviorState.None ||
        _behaviorState == OldEnemyBehaviorState.Idle;
    public bool isAttackingPlayer =>
        _behaviorState == OldEnemyBehaviorState.AttackingPlayer;
    public bool isActivelyChasingPlayer =>
        _behaviorState == OldEnemyBehaviorState.ChasingPlayer;
    public bool isPassivelyChasingPlayer =>
        _behaviorState == OldEnemyBehaviorState.SearchingForPlayer;
    public bool isChasingOrAttackingPlayer =>
        isActivelyChasingPlayer ||
        isAttackingPlayer ||
        isPassivelyChasingPlayer;
    #endregion
    public void SetState(OldEnemyBehaviorState state) {
        var prevState = _behaviorState;
        _behaviorState = state;
        if (state != prevState) {
            SetActiveAction(GetActionBehaviorForActionState(state));
        }
    }
    private IEnemyBehaviorState GetActionBehaviorForActionState(OldEnemyBehaviorState state) {
        switch (state) {
            case OldEnemyBehaviorState.None: break;
            case OldEnemyBehaviorState.Idle: {
                foreach (var action in idleActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case OldEnemyBehaviorState.AttackingPlayer: {
                foreach (var action in attackActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case OldEnemyBehaviorState.ChasingPlayer: {
                foreach (var action in pursueActions) {
                    if (action.CanActivate(player)) {
                        return action;
                    }
                }
            } break;
            case OldEnemyBehaviorState.SearchingForPlayer: {
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
        } else if (_behaviorState != OldEnemyBehaviorState.None) {
            Debug.LogWarning(
                "no active state for "+action+" on OldEnemyController for "+gameObject+
                       ": switching to null (None) state");
            _behaviorState = OldEnemyBehaviorState.None;
        }
    }

    void Update() {
      //  Debug.Log(_behaviorState);
        if (isChasingOrAttackingPlayer) {
            OldEnemyBehaviorState _;
            if (activeState != null && isAttackingPlayer && !activeState.ActionFinished(out _)) {
            } else {
                foreach (var attack in attackActions) {
                    if (attack.CanActivate(player)) {
                        _behaviorState = OldEnemyBehaviorState.AttackingPlayer;
                        SetActiveAction(attack);
                    }
                }
            }
        }
        // Check: is this action finished? if so, automatically pick + start next action
        if (activeState != null) {
            OldEnemyBehaviorState nextActionType;
            if (activeState.ActionFinished(out nextActionType)) {
                // action finished - contextually activate next action
                Debug.Log(nextActionType);
                SetState(nextActionType);
            } else {
                activeState.UpdateAction();
            }
           // Debug.Log(_behaviorState);
        } else {
            ///Debug.Log(_behaviorState);
            switch (_behaviorState) {
                case OldEnemyBehaviorState.None:
                    if (idleActions.Length > 0) {
                        SetState(OldEnemyBehaviorState.Idle);
                    }
                    break;
                case OldEnemyBehaviorState.AttackingPlayer:
                    if (attackActions.Length > 0) {
                        SetState(OldEnemyBehaviorState.AttackingPlayer);
                    }
                    break;
                default: break;
            }
        }
    }

    public void OnPlayerDetected(Player player) {
        if (isHostileToPlayer && (isIdle || isPassivelyChasingPlayer)) {
            SetState(OldEnemyBehaviorState.ChasingPlayer);
        }
    }
    public void OnPlayerLost(Player player) {
        if (isHostileToPlayer && !isIdle) {
            SetState(OldEnemyBehaviorState.SearchingForPlayer);
        }   
    }
    public void OnObjectiveDetected(IEnemyObjectiveMarker objective) { }
    
    private void Awake() {
        // Get all references
        if (!oldEnemy) { Debug.LogError("OldEnemyController: Enemy reference missing!"); }
        if (!player) { player = GameObject.FindObjectOfType<Player>(); }

        // Setup enemy's controller references
        oldEnemy.controller = this;
        SetupSubControllers(oldEnemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
        
        InitActionLists();
    }

    private void OnEnable() {
        SetupSubControllers(oldEnemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
        foreach (Transform child in transform) {
            SetupSubControllers(child.gameObject.GetComponents<IEnemyControllerComponent>());
            foreach (Transform grandkid in child.transform) {
                SetupSubControllers(grandkid.gameObject.GetComponents<IEnemyControllerComponent>());
            }
        }
    }

    private void SetupSubControllers(IEnemyControllerComponent[] components) {
        foreach (var component in components) {
            component.SetupControllerComponent(this);
        }
    }
}
