﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class EnemyController : EnemyComponent {

    #region Stuff I prob want
    // reference to all enemy actions
    private EnemyAbility[] attackActions => enemy.attack;
    private EnemyAbility[] pursueActions => enemy.chase;
    private EnemyAbility[] searchForPlayerActions => enemy.patrol;
    private EnemyAbility[] idleActions => enemy.idle;

    public bool isHostileToPlayer = true;
    // private IEnemyBehaviorState activeState = null;
    private EnemyAbility activeState = null;
    public EnemyBehaviorState behaviorState => _behaviorState;

    #region BehaviorStateProperties
    private EnemyBehaviorState _behaviorState = EnemyBehaviorState.None;
    public bool isIdle =>
        _behaviorState == EnemyBehaviorState.None ||
        _behaviorState == EnemyBehaviorState.Idle;
    public bool isAttackingPlayer =>
        _behaviorState == EnemyBehaviorState.AttackingPlayer;
    public bool isActivelyChasingPlayer =>
        _behaviorState == EnemyBehaviorState.ChasingPlayer;
    public bool isPassivelyChasingPlayer =>
        _behaviorState == EnemyBehaviorState.SearchingForPlayer;
    public bool isChasingOrAttackingPlayer =>
        isActivelyChasingPlayer ||
        isAttackingPlayer ||
        isPassivelyChasingPlayer;
    #endregion

    #region State Functions
    public void SetState(EnemyBehaviorState state) {
        var prevState = _behaviorState;
        _behaviorState = state;
        if (state != prevState) {
            SetActiveAction(GetActionBehaviorForActionState(state));
          // FireEvent(/*Entered ___ state*/);
        }
    }

    private EnemyAbility GetActionBehaviorForActionState(EnemyBehaviorState state) {
        EnemyAbility[] actionSet = null;
        switch (state) {
            case EnemyBehaviorState.None: break;
            case EnemyBehaviorState.Idle: {
                actionSet = idleActions;
            } break;
            case EnemyBehaviorState.AttackingPlayer: {
                actionSet = attackActions;  
            } break;
            case EnemyBehaviorState.ChasingPlayer: {
                actionSet = pursueActions;  
            } break;
            case EnemyBehaviorState.SearchingForPlayer: {
                actionSet = searchForPlayerActions;  
            } break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        if (actionSet != null) {
            foreach (var action in actionSet) {
                if (action.canUseAbility) {
                    return action;
                }
            }
        }
        return null;
    }
    
    private void SetActiveAction(EnemyAbility action) {
        if (activeState != null) {
            activeState.CancelAbility();
            activeState = null;
        }
        activeState = action;
        if (activeState != null) {
            activeState.Internal_StartAbility();
        }
        else if (_behaviorState != EnemyBehaviorState.None) {
            Debug.LogWarning(
                "no active state for " + action + " on EnemyController for " + gameObject +
                       ": switching to null (None) state");
            _behaviorState = EnemyBehaviorState.None;
        }
    }
    #endregion

    #region Player Detection
    public void OnPlayerDetected(Player player) {
        enemy.player = player;
        if (isHostileToPlayer && (isIdle || isPassivelyChasingPlayer)) {
            SetState(EnemyBehaviorState.ChasingPlayer);
        }
    }
    public void OnPlayerLost() {
        if (isHostileToPlayer && !isIdle) {
            SetState(EnemyBehaviorState.SearchingForPlayer);
        }
    }
    public void OnObjectiveDetected(IEnemyObjectiveMarker objective) { }
    #endregion

    #endregion

    void FixedUpdate() {
        //Debug.Log(behaviorState);
        if (isChasingOrAttackingPlayer) {
            if (activeState != null && isAttackingPlayer && activeState.isAbilityActive) { }
            else {
                foreach (var attack in attackActions) {
                    if (attack.canUseAbility) { 
                        _behaviorState = EnemyBehaviorState.AttackingPlayer;
                        SetActiveAction(attack);
                    }
                }
            }
        }
        // Check: is this action finished? if so, automatically pick + start next action
        if (activeState != null) {
            EnemyBehaviorState nextActionType;
            if (activeState.AbilityFinished(out nextActionType)) {
                // action finished - contextually activate next action
                SetState(nextActionType);
            }
        }
        else {
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
}
