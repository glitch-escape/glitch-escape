using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlitchEscape;
using GlitchEscape.Effects;
using GlitchEscape.Scripts.DebugUI;
    
/// <summary>
/// Implements player gravity.
/// Player gravity can be disabled by disabling this component or setting gravityEnabled = false.
/// Provides properties to change gravity direction + strength, which could be used to implement
/// various game effects, game mechanics, and player abilities.
/// TODO: 1) add capability to change player orientation along w/ gravity (possibly in PlayerMovement...?)
/// TODO: 2) add an effects system to implement gravity effects that temporarily change gravity properties
/// TODO: and both restore prev state AND support multiple effects simultaneously
/// </summary>
public class PlayerGravity : PlayerComponent, IResettable, IPlayerDebug {
    [InjectComponent] public PlayerMovement playerMovement;

    /// <summary>
    /// Owns all mutable state for this class.
    /// Enables temporary effects to be applied + unapplied in a safe and controlled fashion.
    /// Effect machinery implemented in <see cref="EffectState{TOwner,TState}"/>.
    /// This provides <see cref="EffectState{TOwner,TState}.CreateEffect(StateEffector{TOwner,TState})"/> to create effects.
    ///
    /// A basic explanation of the effect system is as follows:
    /// - effects are tracked in a list owned by <see cref="EffectState{TOwner,TState}"/>
    /// - creating an effect results in the effect being added to this list
    /// - an <see cref="Effect{TOwner,TState}"/> is basically a wrapper around a function that modifies state, with the ability
    /// to enable / disable itself through the <see cref="Effect{TOwner,TState}"/> reference, and to delete itself / remove itself
    /// from the effect list
    /// - state is built by taking the initial state (defined by <see cref="SetDefaults"/>), and applying all active
    /// state changes to it in sequence
    /// - state is rebuilt anytime any <see cref="Effect{TOwner,TState}"/> is enabled / disabled, or added / removed, and is handled
    /// by the state effect impl
    /// - state can be fully reset (with all current effects destroyed) by calling <see cref="EffectState{TOwner,TState}.Reset()"/>
    ///
    /// Note:
    /// - this description should be familiar if you're familiar with the theoretical concepts behind functional
    /// programming (and specifically, how you can implement mutable state in a pure FP language)
    /// - it's used artificially in this context in an attempt to reduce bugs (once the effect impl is fully
    /// finished and tested, anyways)
    /// - it's also (conceptually) similar how the react library (js) works to render html
    /// (in fact, that's one of the key inspirations behind this specific design)
    ///
    /// Note that this state effect impl is *unidirectional*, not *bidirectional*.
    /// in a more conventional (and non-FP) bidirectional system, we'd need to track 'do' and 'undo' actions
    /// to execute + unexecute state changes.
    /// instead we ONLY track 'do' actions and reapply *all* of the active changes sequentially, starting from
    /// some initial / reset state, to fully rebuild the current state any time that the state changes.
    ///
    /// Either way, there are some obvious consequences to this that you should be aware of if you're not familiar
    /// with functional programming:
    /// - state should be considered immutable, ie. directly modifying state variables outside of the effect system
    /// is illegal and WILL cause bugs.
    /// - game systems that can be interacted with by multiple abilities, etc., should probably be moved to the
    /// effect system and have all of their mutable state (ie. variables) encapsulated within a State class
    /// (iff it makes sense to do so, but ideally there should not be any free-floating member variables in
    /// game system classes, outside of component references)
    /// - access to state can be done via readonly properties (ie. state is only exposed through immutable values)
    ///
    /// PlayerGravity, while simple, is a pretty good example of how this can work (in theory) for purely mechanical
    /// elements., ie. a bunch of float + vec3 properties can get changed + reset by any other number of systems
    /// at the same time, ie:
    /// - player jump changes gravity when falling (player ability)
    /// - player dash temporarily disables gravity while dashing (and then re-applies "lost" gravity as a velocity
    /// change when the effect ends) - another player ability
    /// - a gravity effector field could be added that changes the player's orientation + gravity (level mechanic)
    /// - another effector field could increase or decrease the force of gravity on the player (level mechanic)
    /// - gravity will likely play a role in other abilities + game mechanics, etc.
    /// - all of these effects need to interact simultaneously
    /// - all of these effects need to be cancelled when the player respawns
    /// - not having to deal with hunting down obscure bugs related to the above (RIP QA team, lol) would be nice
    /// (that said, not having to hunt down obscure bugs due to the effect system is... also not guaranteed, but
    /// but at the least you could always print out all the active effects into some debug gui or something if
    /// that became necessary)
    /// </summary>
    public class State : EffectState<PlayerGravity, State> {
        public float gravity = 9.81f;
        public float gravityMultipliers = 1f;
        public Vector3 direction = Vector3.down;

        protected override void SetDefaults(PlayerGravity user) {
            gravity = user.player.config.gravity;
            gravityMultipliers = 1f;
            direction = Vector3.down;
        }

        public State(PlayerGravity owner) : base(owner) { }
    }
    private State state;
    void OnEnable() { state = new State(this); }
    public void Reset() { state.Reset(); }
    
    public struct ModifyGravityEffect : IEffector<PlayerGravity, State> {
        public float strength;
        public void Apply(State state) {
            state.gravityMultipliers *= strength;
        }
    }
    
    /// <summary>
    /// Applies a cumulative gravity multiplier (note: can use strength = 0f to disable gravity entirely)
    /// </summary>
    public IEffect ModifyGravity(float strength) {
        return state.CreateEffect(new ModifyGravityEffect { strength = strength});
    }

    public struct SetGravityDirectionEffect : IEffector<PlayerGravity, State> {
        public Vector3 direction;
        public void Apply(State state) {
            state.direction = direction;
        }
    }
    
    /// <summary>
    /// Changes the direction gravity is applied in.
    /// Assumes this direction is a normalized vector; may have unexpected results otherwise.
    /// </summary>
    public IEffect SetGravityDirection(Vector3 direction) {
        return state.CreateEffect(new SetGravityDirectionEffect { direction = direction });
    }


    /// <summary>
    /// is gravity currently enabled...?
    /// </summary>
    public bool gravityEnabled => Mathf.Abs(gravity) >= 1e-6f;

    /// <summary>
    /// Gravity direction, relative to player
    /// </summary>
    public Vector3 gravityDirection => state.direction;

    /// <summary>
    /// Current gravity value, in meters / sec^2
    /// Changes so player has a different effective gravity when falling - used to
    /// make jumping (ie. player has y-axis velocity + is not falling) balanced while
    /// still falling quickly
    /// </summary>
    public float gravity => state.gravity * state.gravityMultipliers;

    /// <summary>
    /// Applies gravity each time step, if it is enabled
    /// </summary>
    private void FixedUpdate() {
        if (gravityEnabled) {
            playerMovement.ApplyAcceleration(gravity * Time.deltaTime * gravityDirection);
        }

        if (Mathf.Abs(gravityDirection.magnitude) - 1f > 1e-6f) {
            Debug.LogWarning("Gravity direction is non-normalized and may be zero! on "
                             + this + ": " + gravityDirection);
        }
    }
    
    public string debugName => this.GetType().Name;
    public void DrawDebugUI() {
        GUILayout.Label("current gravity: " + gravity);
        GUILayout.Label("gravity enabled: " + gravityEnabled);
        GUILayout.Label("gravity strength: " + state.gravityMultipliers);
        GUILayout.Label("gravity direction: " + gravityDirection);
        GUILayout.Label(state.ToString());
    }
}
