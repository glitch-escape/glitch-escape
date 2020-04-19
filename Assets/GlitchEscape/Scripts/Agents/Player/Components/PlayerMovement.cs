using System;
using System.Collections.Generic;
using GlitchEscape.Effects;
using GlitchEscape.Scripts.DebugUI;
using JetBrains.Annotations;
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
    void Internal_RegisterEffect(IEffect effect);
    void Internal_UnregisterEffect(IEffect effect);
}
public interface IEffectActions {
    void ApplyEffect();
    void UnapplyEffect();
    void UpdateEffect();
}
public struct EffectActions : IEffectActions {
    public delegate void Action();
    public Action applyEffect;
    public Action unapplyEffect;
    public Action updateEffect;

    public void ApplyEffect() { applyEffect?.Invoke(); }
    public void UnapplyEffect() { unapplyEffect?.Invoke(); }
    public void UpdateEffect() { updateEffect?.Invoke(); }
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

    public static Effect MakeEffect(IEffectActions actions) {
        return new Effect(actions);
    }
    public static Effect MakeEffect(IEffectActions actions, EffectState.StateCheckCallback checkFinished) {
        return new Effect(actions, new EffectState{checkFinished = checkFinished});
    }
    public static Effect MakeEffect(IEffectActions actions, IEffectState state) {
        return new Effect(actions, state);
    }
    protected Effect(IEffectActions actions, IEffectState state) {
        this.actions = actions;
        this.effectState = state;
    }
    protected Effect(IEffectActions actions) {
        this.actions = actions;
    }
    private IEffectManager _owner;
    public IEffectManager effectOwner {
        get => _owner;
        set {
            if (_owner == value) return;
            _owner?.Internal_UnregisterEffect(this);
            value?.Internal_RegisterEffect(this);
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

public interface IDurationEffectState : IEffectState {
    float duration { get; set; }
    float startTime { get; }
    float endTime { get; set; }
    float elapsedTime { get; }
    float remainingTime { get; }
}

public abstract class ADurationEffectState : IDurationEffectState {
    public abstract float duration { get; set; }
    private float timeStarted;
    private bool  started = false;

    public bool finished => started && elapsedTime >= duration;
    public float startTime => started ? timeStarted : 0f;
    public float elapsedTime => started ? Time.time - timeStarted : 0f;
    public float remainingTime => started ? duration - elapsedTime : duration;
    public float endTime {
        get => (started ? timeStarted : Time.time) + duration;
        set => duration = value - (started ? timeStarted : Time.time);
    }
    public void OnStarted() { started = true; timeStarted = Time.time; }
    public void OnEnded() { }
}

public class ManagedDurationEffectState : ADurationEffectState {
    public override float duration { get; set; }
    public ManagedDurationEffectState(float duration) {
        this.duration = duration;
    }
}
public class DerivedDurationEffectState : ADurationEffectState {
    private Duration getDuration;
    public override float duration {
        get => getDuration();
        set { }
    }
    public DerivedDurationEffectState(Duration getDuration) {
        this.getDuration = getDuration;
    }
}

public delegate float Duration();
public class DurationEffect : Effect {
    [NotNull] private new IDurationEffectState effectState { get; }
    public float elapsedTime => effectState.elapsedTime;
    public float remainingTime => effectState.remainingTime;
    public float startTime => effectState.startTime;
    public float duration {
        get => effectState.duration;
        set => effectState.duration = value;
    }
    public float endTime {
        get => effectState.endTime;
        set => effectState.endTime = value;
    }
    private DurationEffect(IEffectActions actions, IDurationEffectState state) : base(actions, state) {
        effectState = state;
    }
    public static DurationEffect MakeEffect (IEffectActions actions, float duration) {
        return new DurationEffect(actions, new ManagedDurationEffectState(duration));
    }
    public static DurationEffect MakeEffect(IEffectActions actions, Duration duration) {
        return new DurationEffect(actions, new DerivedDurationEffectState(duration));
    }
}


public class EffectManager : IEffectManager {
    private HashSet<IEffect> effects = new HashSet<IEffect>();
    public void Internal_RegisterEffect(IEffect effect) {
        effects.Add(effect);
    }
    public void Internal_UnregisterEffect(IEffect effect) {
        effects.Remove(effect);
    }
    public T AddEffect<T>(T effect) where T : IEffect {
        effect.effectOwner = this;
        return effect;
    }
    public T ApplyEffect<T>(T effect) where T : IEffect {
        effect.effectOwner = this;
        if (!effect.started) {
            effect.Start();
        }
        return effect;
    }
    public void Clear() {
        foreach (var effect in effects) {
            effect.Cancel();
            effect.effectOwner = null;
        }
        effects.Clear();
    }
    public void Update() {
        foreach (var effect in effects) {
            if (effect.started) {
                if (effect.finished) {
                    effect.Cancel();
                    effect.effectOwner = null;
                } else {
                    effect.Update();
                }
            }
        }
    }

    public Effect AddEffect(IEffectActions actions) {
        return AddEffect(Effect.MakeEffect(actions));
    }
    public Effect AddEffect(IEffectActions actions, IEffectState state) {
        return AddEffect(Effect.MakeEffect(actions, state));
    }
    public Effect AddEffect(IEffectActions actions, EffectState.StateCheckCallback finished) {
        return AddEffect(Effect.MakeEffect(actions, finished));
    }
    public DurationEffect AddEffect(IEffectActions actions, float duration) {
        return AddEffect(DurationEffect.MakeEffect(actions, duration));
    }
    public DurationEffect AddEffect(IEffectActions actions, Duration duration) {
        return AddEffect(DurationEffect.MakeEffect(actions, duration));
    }
}


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : PlayerComponent, IResettable, IPlayerDebug {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public PlayerControls playerInput;
    [InjectComponent] public new Camera camera;

    public class State : EffectState<PlayerMovement, State> {
        public bool enabled;
        public float moveSpeed;
        public float dashSpeed;
        public float moveSpeedMultiplier;
        public State(PlayerMovement owner) : base(owner) { }
        protected override void SetDefaults(PlayerMovement owner) {
            enabled = true;
            moveSpeed = owner.player.config.runSpeed;
            dashSpeed = 0f;
            moveSpeedMultiplier = 1f;
        }
    }
    private State state;
    private void OnEnable() { state = new State(this); }

    struct ApplyDashSpeedEffect : IEffector<PlayerMovement, State> {
        public float dashSpeed;
        public void Apply(State state) { state.dashSpeed += dashSpeed; }
    }
    public IEffectHandle ApplyDashSpeed(float speed) {
        return state.CreateEffect(new ApplyDashSpeedEffect { dashSpeed = speed });
    }
    struct AddMoveSpeedEffect : IEffector<PlayerMovement, State> {
        public float multiplier;
        public void Apply(State state) { state.moveSpeedMultiplier *= multiplier; }
    }
    public IEffectHandle AddMoveSpeed(float multiplier) {
        return state.CreateEffect(new AddMoveSpeedEffect { multiplier = multiplier });
    }
    struct SetMovementEnabledEffect : IEffector<PlayerMovement, State> {
        public bool enabled;
        public void Apply(State state) { state.enabled = enabled; }
    }
    
    [Tooltip("Player movement mode")] 
    public PlayerMovementMode movementMode = PlayerMovementMode.TurnToFaceMoveDirection;
    public enum PlayerMovementMode {
        TurnToFaceMoveDirection,
    }
    public float moveSpeed => state.enabled ? state.moveSpeed * state.moveSpeedMultiplier : 0f;
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
        state.Reset();
        isMoving = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// Updates player's movement vector + applies gravity each physics update
    /// </summary>
    void FixedUpdate() {
        Move(playerInput.moveInput * Time.fixedDeltaTime);
        
        // apply dash, if active
        if (state.dashSpeed > 0f) {
            var dashDir = player.transform.forward;
            rigidbody.MovePosition(rigidbody.position + dashDir * state.dashSpeed * Time.fixedDeltaTime);
        }
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
    
    public string debugName => this.GetType().Name;
    public void DrawDebugUI() {
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
