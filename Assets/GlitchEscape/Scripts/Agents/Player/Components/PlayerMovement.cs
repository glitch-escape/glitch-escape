using System;
using GlitchEscape.Effects;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;

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
    public IEffect ApplyDashSpeed(float speed) {
        return state.CreateEffect(new ApplyDashSpeedEffect { dashSpeed = speed });
    }
    struct AddMoveSpeedEffect : IEffector<PlayerMovement, State> {
        public float multiplier;
        public void Apply(State state) { state.moveSpeedMultiplier *= multiplier; }
    }
    public IEffect AddMoveSpeed(float multiplier) {
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
        var v0 = Mathf.Sqrt(Mathf.Abs(2f * gravity * jumpHeight));
        Debug.Log("AAAAA" + v0);
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
        GUILayout.Label(state.ToString());
    }
}
