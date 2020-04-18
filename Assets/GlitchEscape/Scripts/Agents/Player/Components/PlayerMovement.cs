using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public interface IEffect : IResettable {
    bool started { get; }
    bool active { get; }
    bool finished { get; }
    void Start();
    void Cancel();
    void Update();
    IEffectManager effectOwner { get; set; }
}
public interface IEffectManager {
    void RegisterEffect(IEffect effect);
    void UnregisterEffect(IEffect effect);
}
public interface IEffectActions {
    void ApplyEffect();
    void UnapplyEffect();
    void UpdateEffect();
}
public struct EffectActions {
    public delegate void Action();
    public Action applyEffect;
    public Action unapplyEffect;
    public Action updateEffect;
}
public interface IEffectState {
    bool finished { get; }
    void OnStarted();
    void OnEnded();
}
public struct EffectState : IEffectState {
    public delegate bool StateCheckCallback();
    public delegate void Callback();
    public StateCheckCallback checkFinished;
    public Callback onStarted;
    public Callback onEnded;
    public bool finished => checkFinished?.Invoke() ?? false;
    public void OnStarted() { onStarted?.Invoke(); }
    public void OnEnded() { onEnded?.Invoke(); }
}


/// <summary>
/// Combines an effect action (ie. what happens when an effect is triggered, updates, and ends) with an effect state
/// (ie. is this effect active?)
///
/// Implements internal state management that implements the interface of IEffect and makes sure that the right action
/// effect callbacks are always called, and always called exactly once, in response to the following events:
/// - <see cref="Effect.Start()"/> (starts an effect; takes effect iff effect was not yet started)
/// - <see cref="Effect.Cancel()"/> (immediately cancels / ends an effect)
/// - <see cref="Effect.Reset()"/> (cancels the effect + resets it so that it can be started again)
/// - the <see cref="IEffectState"/>'s effect ending (ie. <see cref="IEffectState.finished"/> returns true)
/// 
/// Also provides additional callbacks to listen ot effect state changing:
/// <see cref="Effect.onEffectStarted"/>
/// <see cref="Effect.onEffectEnded"/>
/// <see cref="Effect.onEffectCancelled"/>
///
/// Can be managed by an <see cref="EffectManager{TInterface}"/>
/// </summary>
public class Effect : IEffect {
    public IEffectActions actions   { get; set; }
    public IEffectState effectState { get; set; }
    public delegate void Delegate();
    public Delegate onEffectStarted;
    public Delegate onEffectEnded;
    public Delegate onEffectCancelled;

    public static Effect MakeEffect<T>(T actions) where T : IEffectActions {
        return new Effect(actions);
    }
    public static Effect MakeEffect<T>(IEffectActions actions, EffectState.StateCheckCallback checkFinished) {
        return new Effect(actions, new EffectState{checkFinished = checkFinished});
    }
    public static Effect MakeEffect<T>(IEffectActions actions, IEffectState state) {
        return new Effect(actions, state);
    }
    public Effect(IEffectActions actions, IEffectState state) {
        this.actions = actions;
        this.effectState = state;
    }
    public Effect(IEffectActions actions) {
        this.actions = actions;
    }
    public Effect(IEffectState state) {
        this.effectState = state;
    }
    private IEffectManager _owner;
    public IEffectManager effectOwner {
        get => _owner;
        set {
            if (_owner == value) return;
            _owner?.UnregisterEffect(this);
            value?.RegisterEffect(this);
            _owner = value;
        }
    }
    private enum State { None, Started, Finished }
    private State state = State.None;
    private bool effectFinished => effectState?.finished ?? false;
    public bool started  => state != State.None;
    public bool finished => state == State.Finished || effectFinished;
    public bool active => started && !finished;
    public void Start() {
        if (state == State.None) {
            state = State.Started;
            actions?.ApplyEffect();
            effectState?.OnStarted();
            onEffectStarted?.Invoke();
            CheckFinished();
        }
    }
    private void SetFinished() {
        state = State.Finished;
        actions?.UnapplyEffect();
        effectState?.OnEnded();
        onEffectEnded?.Invoke();
    }
    private bool CheckFinished() {
        if (effectFinished) {
            SetFinished();
            return true;
        }
        return false;
    }
    public void Cancel() {
        if (state == State.Started) {
            bool wasFinished = effectFinished;
            SetFinished();
            if (!wasFinished) {
                onEffectCancelled?.Invoke();
            }
        }
    }
    public void Update() {
        if (!CheckFinished()) {
            actions?.UpdateEffect();
        }
    }
    public void Reset() {
        Cancel();
        state = State.None;
    }
}


public class EffectManager<TInterface> where TInterface : IEffectState {
    private HashSet<IEffectState> activeEffects;
    public T ApplyEffect<T>(T effect) where T : TInterface {
        if (!effect.started) {
            effect.Start();
        }
        AddEffect(effect);
        return effect;
    }
    public T AddEffect<T>(T effect) where T : TInterface {
        if (effect.finished) {
            effect.Cancel();
        } else {
            activeEffects.Add(effect);
        }
        return effect;
    }
    public void Clear() {
        foreach (var effect in activeEffects) {
            effect.Cancel();
        }
        activeEffects.Clear();
    }
    public void Update() {
        foreach (var effect in activeEffects) {
            if (effect.finished) {
                effect.Cancel();
                activeEffects.Remove(effect);
            } else {
                effect.UpdateEffect();
            }
        }
    }
}


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : PlayerComponent, IResettable {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public PlayerControls playerInput;
    [InjectComponent] public new Camera camera;

    #region Effects
    private float moveSpeedMultiplier = 1f;
    public Effect IncreaseMoveSpeed(float moveSpeedMultiplier) {
        return CreateEffect(
            () => this.moveSpeedMultiplier += moveSpeedMultiplier,
            () => this.moveSpeedMultiplier -= moveSpeedMultiplier);
    }
    #region EffectImpl (Copy + Paste)
    public class Effect : ABasicDurationEffect {
        public delegate void EffectApplicator();
        private EffectApplicator _applyEffect;
        private EffectApplicator _unapplyEffect;
    
        public Effect(EffectApplicator apply, EffectApplicator unapply) {
            _applyEffect = apply;
            _unapplyEffect = unapply;
        }
        protected override void ApplyEffect() { _applyEffect?.Invoke(); }
        protected override void UnapplyEffect() { _unapplyEffect?.Invoke(); }
    }
    private Effect CreateEffect(Effect.EffectApplicator apply, Effect.EffectApplicator unapply) {
        return effects.AddEffect(new Effect(apply, unapply));
    }
    public EffectManager<Effect> effects { get; private set; } = new EffectManager<Effect>();
    #endregion EffectImpl
    #endregion Effects

    [Tooltip("Player movement mode")] 
    public PlayerMovementMode movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
    public enum PlayerMovementMode {
        TurnToFaceMoveDirection,
    }
    
    public float moveSpeed => player.config.runSpeed * moveSpeedMultiplier;
    public bool isFalling => rigidbody.velocity.y < 0f;

    /// <summary>
    /// Determines if the player is currently moving or not.
    /// Set by calls to <see cref="Move(Vector2)"/>, called by <see cref="PlayerMovement.FixedUpdate()"/>
    /// </summary>
    public bool isMoving {
        get => _isMoving;
        private set {
            var wasMoving = _isMoving;
            _isMoving = value;
            if (value != wasMoving) {
                FireEvent(value ? 
                    PlayerEvent.Type.BeginMovement :
                    PlayerEvent.Type.EndMovement);   
            }
        }
    }
    private bool _isMoving = false;
    
    /// <summary>
    /// Makes the player jump to a target jump height.
    /// Used by <see cref="PlayerJumpAbility"/>
    /// </summary>
    /// <param name="jumpHeight">Peak jump height (in meters), used to calcualte jump force</param>
    /// <param name="direction">Jump direction (should default to player.transform.up)</param>
    public void ApplyJump(float jumpHeight) {
        SetVelocity(CalculateJumpVelocity(jumpHeight));
    }

    /// <summary>
    /// Makes the player jump to a target jump height.
    /// Used by <see cref="PlayerJumpAbility"/>
    /// </summary>
    /// <param name="jumpHeight">Peak jump height (in meters), used to calcualte jump force</param>
    /// <param name="direction">Jump direction (should default to player.transform.up)</param>
    public void ApplyJump(float jumpHeight, Vector3 direction) {
        SetVelocity(CalculateJumpVelocity(jumpHeight, direction));
    }
    
    /// <summary>
    /// Calculates initial jump velocity given jump height
    /// Used by <see cref="ApplyJump(float)"/>
    /// </summary>
    public Vector3 CalculateJumpVelocity(float jumpHeight) {
        return CalculateJumpVelocity(jumpHeight, player.transform.up);
    }

    /// <summary>
    /// Calculates initial jump velocity given jump height + direction
    /// Used by <see cref="ApplyJump(float, Vector3)"/>
    /// </summary>
    public Vector3 CalculateJumpVelocity(float jumpHeight, Vector3 direction) {
        var gravity = Mathf.Abs(Physics.gravity.y);
        var v0 = Mathf.Sqrt(2f * gravity * jumpHeight);
        return v0 * direction;
    }

    /// <summary>
    /// Applies an immediate velocity change to the player
    /// Used to implement <see cref="ApplyJump(float)"/>, etc.
    /// </summary>
    public void SetVelocity(Vector3 velocity) {
        Debug.Log("Set player velocity at "+Time.time+" to "+velocity);
        rigidbody.velocity = velocity;
    }

    /// <summary>
    /// Applies an acceleration to the player
    /// equivalent to calling rigidbody.AddForce(acceleration, ForceMode.VelocityChange)
    /// acceleration should have deltaTime or fixedDeltaTime pre-applied to it
    /// </summary>
    public void ApplyAcceleration(Vector3 acceleration) {
        rigidbody.AddForce(acceleration, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Moves the player given some input.
    /// Called by <see cref="PlayerMovement.FixedUpdate()"/>
    /// Input is assumed to be normalized / mapped to [-1, 1] and then multiplied by Time.time or Time.fixedDeltaTime
    /// </summary>
    private void Move (Vector2 input) {
        // check if player is moving or not, and fire events if state changes
        isMoving = HasInput(input);
        if (!isMoving) return;

        // convert input axes to be relative to camera
        var moveDir = isMoving ? MoveInputToWorldSpaceVector(input) : Vector3.zero;
        switch (movementMode) {
            case PlayerMovementMode.TurnToFaceMoveDirection: {
                var desiredForward = Vector3.RotateTowards(
                    player.transform.forward,
                    moveDir,
                    player.config.turnSpeed * Time.deltaTime, 
                    0f);
                var playerRotation = Quaternion.LookRotation(desiredForward);
                
                rigidbody.MovePosition(rigidbody.position + moveDir * moveSpeed);
                rigidbody.MoveRotation(playerRotation);
            } break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Resets player movement state
    /// </summary>
    public void Reset() { 
        isMoving = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// Updates player's movement vector + applies gravity each physics update
    /// </summary>
    void FixedUpdate() {
        Move(playerInput.moveInput * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Tests if player input is considered to be zero (ie. below some threshold) or not.
    /// Returns true if this input is considered to indicate that the player is "moving", false otherwise.
    /// </summary>
    private bool HasInput(Vector2 input) {
        return input.magnitude > 1e-6;
    }

    /// <summary>
    /// Transforms player input (assume normalized to [-1, 1]) into a world-space movement vector relative
    /// to the current camera position
    /// </summary>
    private Vector3 MoveInputToWorldSpaceVector(Vector2 input) {
        
        // get direction (forward + right vectors) of camera, then clamp to X, Z plane (set Y component to zero),
        // and normalize the result
        var cameraTransform = camera.transform;
        var forward = cameraTransform.forward;
        var right   = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        // resulting movement vector is input relative to the camera facing (on X, Z plane)
        return forward * input.y + right * input.x;
    }
    
    public bool showDebugGui = false;

    void OnGUI() {
        if (!showDebugGui) return;
        GUILayout.Label("PlayerMovement.cs:");
        GUILayout.Label("Movement mode: " + movementMode);

        var input = playerInput.moveInput;
        GUILayout.Label("Raw player input: " + input);
        GUILayout.Label("has player input? " + HasInput(input));

        var moveDir = MoveInputToWorldSpaceVector(input);
        GUILayout.Label("camera-relative input: " + moveDir);
        GUILayout.Label("rigidbody velocity: " + rigidbody.velocity);

        var turnSpeed = player.config.turnSpeed;
        moveDir *= moveSpeed * Time.deltaTime;
        GUILayout.Label("expected delta-v: " + moveDir);
        GUILayout.Label("expected position: " + moveDir + rigidbody.position);
        GUILayout.Label("player move speed: " + moveSpeed);
        GUILayout.Label("player turn speed: " + turnSpeed);
    }
}
