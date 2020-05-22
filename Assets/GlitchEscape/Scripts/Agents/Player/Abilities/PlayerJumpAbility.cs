using System;
using GlitchEscape.Effects;
using GlitchEscape.Scripts.DebugUI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpAbility : PlayerAbility, IPlayerDebug {
    [InjectComponent] public PlayerMovement playerMovement;
    
    #region PlayerAbilityProperties
    public override float resourceCost => 0f; // jumping does not use stamina
    public override float cooldownTime => 0.01f; // jumping does not have any effective cooldown
    protected override float abilityDuration => 0f; // the jump action itself does not take any time
    protected override PlayerControls.HybridButtonControl inputButton => null; // control using PlayerController instead
    #endregion PlayerAbilityProperties
    #region Variable State
    public uint jumpCount = 0;
    private float jumpStartTime = 0f;
    public bool isJumping { get; private set; } = false;
    private GameObject lastWallJumpedOffOf = null;
    #endregion
    
    public float elapsedJumpTime => isJumping ? Time.time - jumpStartTime : 0f;

    /// <summary>
    /// should reset all player jump state
    /// </summary>
    protected override void OnAbilityReset() {
        jumpCount = 0;
        isJumping = false;
        dirtyRaycastInfo = true;
        lastWallJumpedOffOf = null;
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
            if (player.config.canWallJump && isPlayerNearNewWall) return JumpAbilityUseStatus.CanWallJump;
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
                jumpCount = 0;
                isJumping = true;
                jumpStartTime = Time.time;
                lastWallJumpedOffOf = null;
                playerMovement.JumpToHeight(player.config.jumpHeight);
                FireEvent(PlayerEvent.Type.FloorJump);
                break;
            case JumpAbilityUseStatus.CanAirJump:
                jumpCount += 1;
                isJumping = true;
                jumpStartTime = Time.time;
                playerMovement.JumpToHeight(player.config.jumpHeight);
                FireEvent(PlayerEvent.Type.AirJump);
                break;
            case JumpAbilityUseStatus.CanWallJump:
                jumpCount = 0;
                isJumping = true;
                lastWallJumpedOffOf = wallHitInfo.collider?.gameObject;
                playerMovement.JumpToHeightWithWallJump(
                    player.config.jumpHeight, 
                    currentWallNormal,
                    player.config.wallJumpMultiplier);
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
    public bool isPlayerGrounded {
        get {
            if (dirtyRaycastInfo) UpdateRaycastInfo();
            return hitGround;
        }
    }

    /// <summary>
    /// returns true iff player is currently near a wall (using raycast detection from player center forward)
    /// Fires raycasts iff raycasts were not already executed this frame.
    /// </summary>
    public bool isPlayerNearWall {
        get {
            if (dirtyRaycastInfo) UpdateRaycastInfo();
            return hitWall;
        }
    }
    public bool isPlayerNearNewWall => isPlayerNearWall && lastWallJumpedOffOf != wallHitInfo.collider?.gameObject;
    

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
            player.transform.position, 
            Vector3.down,
            out groundHitInfo,
            player.config.playerRayDistanceToGround);
        hitWall = Physics.Raycast(
            player.transform.position, 
            transform.forward, 
            out wallHitInfo, 
            player.config.wallRaycastDistance,
            wallLayer);
    }

    /// <summary>
    /// Physics update:
    /// - invalidates raycast info (effectively fires new raycasts to determine if player is near wall / floor)
    /// - fires <see cref="PlayerEvent.Type.EndJump"/> when player first touches the floor
    /// </summary>
    void FixedUpdate() {
        dirtyRaycastInfo = true;
        UpdateRaycastInfo();

        // check if we're currently grounded
        // if we're not grounded and were previously jumping, update isJumping + fire an end jumping event
        // && playerMovement.rigidbody.velocity(in ther vertical direction) == 0 */ potentially add this to fix jump reset right after jumping
        if (isPlayerGrounded && isJumping) {
            isJumping = false;
            jumpCount = 0;
            FireEvent(PlayerEvent.Type.EndJump);
        }
    }
    public string debugName => this.GetType().Name;
    public void DrawDebugUI() {
        GUILayout.Label("is jumping? " + isJumping);
        GUILayout.Label("jump count: " + jumpCount + " / " + player.config.maxJumps);
        GUILayout.Label("can jump? " + jumpAbilityStatus);

        var jumpHeight = player.config.jumpHeight;
        GUILayout.Label("jump height: " + jumpHeight);
        GUILayout.Label("gravity: " + player.gravity.standingGravity);
        GUILayout.Label("expected velocity: " + Mathf.Sqrt(player.gravity.standingGravity * 2f * jumpHeight));
        GUILayout.Label("calculated jump velocity " + playerMovement.CalculateJumpVector(jumpHeight));
        GUILayout.Label("calculated wall jump velocity " + playerMovement.CalculateWallJumpVector(
                            jumpHeight, currentWallNormal, player.config.wallJumpMultiplier));

        Debug.DrawRay(player.transform.position, Vector3.down * groundHitInfo.distance, Color.green, 1.0f, false);
        Debug.DrawRay(player.transform.position, Vector3.forward * wallHitInfo.distance, Color.green, 1.0f, false);

        var currentVelocity = playerMovement.rigidbody.velocity;
        GUILayout.Label("current velocity: " + currentVelocity);
        GUILayout.Label("time since jump started: " + elapsedJumpTime);
        GUILayout.Label("is on ground? " + isPlayerGrounded);
        GUILayout.Label("ground hit info " + groundHitInfo);
        GUILayout.Label("is near wall? " + isPlayerNearWall);
        GUILayout.Label("is near new wall? " + isPlayerNearNewWall);
        GUILayout.Label("last wall jumped off of: " + lastWallJumpedOffOf);
        GUILayout.Label("wall normal: " + currentWallNormal);
        GUILayout.Label("current gravity: " + (GetComponent<PlayerGravity>()?.gravity ?? 0f));
    }
}
