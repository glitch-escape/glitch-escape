using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public interface IOneShotEffect {
    bool active { get; }
    void Start();
    void Cancel();
}
public interface IDurationEffect {
    float duration { get; set; }
    float endTime { get; set; }
    float startTime { get; }
    void Start();
    void Cancel();
}
public interface IEffectState {
    bool started { get; }
    bool finished { get; }
    void Start();
    void Cancel();
    void UpdateEffect();
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

public abstract class ABaseEffect : IEffectState, IResettable {
    public bool active => started && !finished;
    protected abstract bool isFinished { get; }
    protected abstract void ApplyEffect();
    protected abstract void UnapplyEffect();
    public abstract void UpdateEffect();
    
    private enum State { None, Started, Finished }
    private State state = State.None;
    public bool started  => state != State.None;
    public bool finished => state == State.Finished || isFinished;
    public virtual void Start() {
        if (state == State.None) {
            state = State.Started;
            ApplyEffect();
            if (finished) {
                state = State.Finished;
                UnapplyEffect();
            }
        }
    }
    public void Cancel() {
        if (state == State.Started) {
            state = State.Finished;
            UnapplyEffect();
        }
    }
    public void Reset() {
        Cancel();
        state = State.None;
    }
}

public abstract class AOneShotEffect : ABaseEffect, IOneShotEffect {
    public override void UpdateEffect() {}
}
public abstract class ADurationEffect : ABaseEffect, IDurationEffect {
    public abstract float duration { get; set; }
    public float startTime { get; private set; }
    public float elapsedTime => Time.time - startTime;
    protected override bool isFinished => elapsedTime >= duration;
    public override void Start() {
        startTime = Time.time;
        base.Start();
    }
    public float endTime {
        get => startTime + duration;
        set => duration = value - startTime;
    }
}

public abstract class OneShotEffect<T> : AOneShotEffect {
    public delegate void EffectApplicator(T target);
    private T _target;
    private EffectApplicator _applyEffect;
    private EffectApplicator _unapplyEffect;
    public OneShotEffect(T target, EffectApplicator apply, EffectApplicator unapply) {
        _target = target;
        _applyEffect = apply;
        _unapplyEffect = unapply;
    }
    protected override void ApplyEffect()   { _applyEffect?.Invoke(_target); }
    protected override void UnapplyEffect() { _unapplyEffect?.Invoke(_target); }
}

public abstract class ABasicDurationEffect : ADurationEffect {
    public override float duration { get; set; } = 0f;
    public override void UpdateEffect() {}
}
public class DurationEffect : ABasicDurationEffect {
    public delegate void EffectApplicator();
    private EffectApplicator _applyEffect;
    private EffectApplicator _unapplyEffect;
    public DurationEffect(float duration, EffectApplicator apply, EffectApplicator unapply) {
        this.duration = duration;
        _applyEffect = apply;
        _unapplyEffect = unapply;
    }
    protected override void ApplyEffect() { _applyEffect?.Invoke(); }
    protected override void UnapplyEffect() { _unapplyEffect?.Invoke(); }
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
