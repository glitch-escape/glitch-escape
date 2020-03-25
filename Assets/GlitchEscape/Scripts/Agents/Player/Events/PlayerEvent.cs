using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a generic listener + event enum for all events that some components (<see cref="PlayerAudioController"/>)
/// can listen to to eg. implement audio clip playback when different player events happen
///
/// Exists so that we can use one event callback method, subscribed to all events, w/ a switch statement, instead
/// of n separate callbacks (w/ n event subscriptions + unsubscriptions) for n events.
/// </summary>
/// <see cref="PlayerAudioController"/>
/// <see cref="PlayerAbility"/>
public static class PlayerEvent {
    /// <summary>
    /// Lists all player events that can be emitted by <see cref="PlayerAbility"/> and <see cref="PlayerComponent"/>
    /// </summary>
    public enum Type {
        // movement
        BeginMovement, EndMovement, TouchedWall, TouchedFloor,

        // jump
        FloorJump, AirJump, WallJump, EndJump,
        
        // dash
        BeginDash, EndDash, RegainedEnoughStaminaForDash,

        // interact
        Interact,
        
        // maze switch
        MazeSwitchToGlitchMaze, MazeSwitchToNormalMaze,
        
        // shoot
        Shoot, RegainedEnoughStaminaForShoot,
        
        // manifest
        BeginManifest, EndManifest, RegainedEnoughStaminaForManifest,
        
        // health + stamina
        PlayerTookDamage, PlayerDeath, PlayerRespawn, NotEnoughStamina,
        BeginHealthRegen, EndHealthRegen,
        BeginStaminaRegen, EndStaminaRegen,

        // TODO: add any other events, and wire up / use these events
    }
    /// <summary>
    /// Generic event type, used by <see cref="PlayerAbility"/>
    /// </summary>
    /// <param name="eventType"></param>
    public delegate void Event (Type eventType);
}
