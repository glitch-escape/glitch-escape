using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpAbility : PlayerAbility {
    [InjectComponent] public new Rigidbody rigidbody;
    [InjectComponent] public Animator animator;
    [InjectComponent] public new Camera camera;

    #region PlayerAbilityProperties
    public override float resourceCost => 0f; // jumping does not use stamina
    public override float cooldownTime => 0.01f; // jumping does not have any effective cooldown
    protected override float abilityDuration => 0f; // the jump action itself does not take any time
    protected override PlayerControls.HybridButtonControl inputButton => PlayerControls.instance.jump;
    #endregion PlayerAbilityProperties
    #region Variable State
    public uint jumpCount = 0;
    private float jumpStartTime = 0f;
    private bool isJumping = false;
    #endregion
    
    public float elapsedJumpTime => isJumping ? Time.time - jumpStartTime : 0f;

    /// <summary>
    /// initial jump velocity, calculated using jump height + effective gravity
    /// </summary>
    private float jumpVelocity => (float) Math.Sqrt(2 * Mathf.Abs(currentUpGravity.y) * player.config.jumpHeight);
    private Vector3 currentUpGravity =>
        player.config.useGravityModifications 
            ? Physics.gravity * player.config.upGravityMultiplier 
            : Physics.gravity;

    /// <summary>
    /// should reset all player jump state
    /// </summary>
    protected override void OnAbilityReset() {
        jumpCount = 0;
        isJumping = false;
        dirtyRaycastInfo = true;
    }

    private enum JumpAbilityUseStatus {
        CannotJump,
        CanGroundJump,
        CanWallJump,
        CanAirJump,
    }

    /// <summary>
    /// Logic for whether player can jump or not (and if so, what kind of jump they're executing)
    /// </summary>
    JumpAbilityUseStatus jumpAbilityStatus {
        get {
            if (!player.config.canJump) return JumpAbilityUseStatus.CannotJump;
            if (isPlayerGrounded) return JumpAbilityUseStatus.CanGroundJump;
            if (player.config.canWallJump && isPlayerNearWall) return JumpAbilityUseStatus.CanWallJump;
            if (player.config.canAirJump && jumpCount < player.config.maxJumps) return JumpAbilityUseStatus.CanAirJump;
            return JumpAbilityUseStatus.CannotJump;
        }
    }

    /// <summary>
    /// Prevents this ability from triggering until the player can jump
    /// </summary>
    protected override bool CanStartAbility() {
        return jumpAbilityStatus != JumpAbilityUseStatus.CannotJump;
    }

    /// <summary>
    /// Called by BaseAbility when this ability can start.
    /// Triggers the effects of a jump action.
    /// Will not be called unless <see cref="PlayerJumpAbility.CanStartAbility()"/> returned true.
    /// </summary>
    protected override void OnAbilityStart() {
        // determine which kind of jump we just executed and trigger the correct actions
        switch (jumpAbilityStatus) {
            case JumpAbilityUseStatus.CannotJump:
                throw new Exception(
                    "Player jump ability started but CanStartAbility() should have returned false!");
            case JumpAbilityUseStatus.CanGroundJump:
                jumpCount = 1;
                isJumping = true;
                rigidbody.velocity = Vector3.up * jumpVelocity;
                jumpStartTime = Time.time;
                FireEvent(PlayerEvent.Type.FloorJump);
                break;
            case JumpAbilityUseStatus.CanAirJump:
                jumpCount += 1;
                isJumping = true;
                rigidbody.velocity = Vector3.up * jumpVelocity;
                jumpStartTime = Time.time;
                FireEvent(PlayerEvent.Type.AirJump);
                break;
            case JumpAbilityUseStatus.CanWallJump:
                jumpCount = 1;
                isJumping = true;
                var wallPushVelocity = currentWallNormal * jumpVelocity * player.config.wallJumpMultiplier;
                var normalJumpVelocity = Vector3.up * jumpVelocity;
                wallPushVelocity += normalJumpVelocity;
                if (wallPushVelocity.y > normalJumpVelocity.y)
                    wallPushVelocity.y = normalJumpVelocity.y;
                rigidbody.velocity = wallPushVelocity;
                jumpStartTime = Time.time;
                FireEvent(PlayerEvent.Type.WallJump);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// returns true iff player is currently grounded (using raycast detection from player center downwards)
    /// Fires raycasts iff raycasts were not already executed this frame.
    /// </summary>
    private bool isPlayerGrounded {
        get {
            if (dirtyRaycastInfo) UpdateRaycastInfo();
            return hitGround;
        }
    }

    /// <summary>
    /// returns true iff player is currently near a wall (using raycast detection from player center forward)
    /// Fires raycasts iff raycasts were not already executed this frame.
    /// </summary>
    private bool isPlayerNearWall {
        get {
            if (dirtyRaycastInfo) UpdateRaycastInfo();
            return hitWall;
        }
    }

    /// <summary>
    /// returns the hit wall normal iff wall raycast hit something, else Vector3.zero.
    /// Fires raycasts if raycasts were not already sent this frame.
    /// </summary>
    private Vector3 currentWallNormal {
        get {
            if (dirtyRaycastInfo) UpdateRaycastInfo();
            return hitWall && wallHitInfo.collider != null ? wallHitInfo.normal : Vector3.zero;
        }
    }

    /// <summary>
    /// raycast dirty flag, set every frame on FixedUpdate().
    /// Exists so that:
    /// - we do raycasts at most once per frame
    /// - player jump has the most accurate, up to date raycast info, even if (for whatever reason)
    /// 
    /// </summary>
    private bool dirtyRaycastInfo = true;
    private RaycastHit groundHitInfo;
    private RaycastHit wallHitInfo;
    private bool hitGround;
    private bool hitWall;

    private void UpdateRaycastInfo() {
        dirtyRaycastInfo = false;
        var wallLayer = LayerMask.GetMask("Wall");
        hitGround = Physics.Raycast(
            rigidbody.position, 
            Vector3.down, 
            out groundHitInfo,
            player.config.playerRayDistanceToGround);
        hitWall = Physics.Raycast(
            rigidbody.position, 
            transform.forward, 
            out wallHitInfo, 
            player.config.wallRaycastDistance,
            wallLayer);
    }

    /// <summary>
    /// Physics update:
    /// - invalidates raycast info (effectively fires new raycasts to determine if player is near wall / floor)
    /// - fires <see cref="PlayerEvent.Type.EndJump"/> when player first touches the floor
    /// - applies gravity modifications, if enabled
    /// </summary>
    void FixedUpdate() {
        // invalidate prev raycast info (force a call to UpdateRaycastInfo() on the next property access)
        dirtyRaycastInfo = true;
        
        // check if we're currently grounded
        // if we're not grounded and were previously jumping, update isJumping + fire an end jumping event
        if (isPlayerGrounded && isJumping) {
            isJumping = false;
            jumpCount = 0;
            FireEvent(PlayerEvent.Type.EndJump);
        }
        
        // apply gravity modifications (TODO: move to a PlayerGravityController class)
        if (player.config.useGravityModifications && rigidbody.velocity.y != 0f && !isPlayerGrounded) {
            if (rigidbody.velocity.y <= 0f) {
                rigidbody.velocity += Physics.gravity * (player.config.downGravityMultiplier - 1f) * Time.fixedDeltaTime;
            } else {
                rigidbody.velocity -= Physics.gravity * (player.config.upGravityMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }
    }
}
