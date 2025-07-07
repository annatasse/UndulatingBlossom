using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateHandler : MonoBehaviour {

    // Assign in Inspector
    public Rigidbody2D rb;
    public Collider2D playerCollider;
    public PlayerStateType prevStateType;
    public PlayerStateType currentStateType;
    public BasePlayerState currentState;
    public Dictionary<PlayerStateType, BasePlayerState> states;
    public Collider2D[] groundColliders;
    public Collider2D[] wallColliders;

    [Header("Room Logic")]
    public bool isInRoom = true;
    public Vector2 roomDirection = Vector2.zero;

    [Header("Friction")]
    public float airFrictionFactor = 0.95f;     // 0: instant stop, 1: no stop, closer to 1: slower stop
    public float airDirectionFactor = 0.80f;    // movement change per frame (0: no change, higher means faster change)

    [Header("Movement")]
    public bool isFacingRight = true;
    public bool isMovingRight = false;
    public float moveSpeed = 6f;
    public Vector2 movementInput;

    [Header("Jump")]
    public float jumpPower = 12f;
    public Collider2D ignoredPlatform;

    [Header("Dash")]
    public bool canDash = false;
    public float dashSpeed = 25;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.05f;
    public float dashLastUse = 0f;

    [Header("Climb")]
    public bool isWallHolding = false;
    public float climbSpeed = 3f;
    public float climbBaseStamina = 10f;
    public float climbStamina = 0f;
    public float climbStaminaDrain = 5f;
    public float wallHoldStaminaDrain = 2f;

    [Header("WallJump")]
    public float wallJumpPowerX = 4f;
    public float wallJumpPowerY = 10f;
    public float wallJumpMoveDelay = 0.2f;

    [Header("GroundCheck")]
    public bool isGrounded = false;
    public LayerMask solidLayer;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize;

    [Header("WallCheck")]
    public bool isWalled = false;
    public bool pressAgainstWall = false;
    public Transform wallCheckPos;
    public Vector2 wallCheckSize;

    [Header("Gravity")]
    public float baseGravity = 2.5f;
    public float fallSpeedMultiplier = 1.5f;
    public float maxFallSpeed = 15f;
    public float wallSlideGravity = 0.4f;

    [Header("StateChanges")]
    public float lastStateChangeTime = 0f;
    public Dictionary<PlayerStateType, float> lastStateEnterTime = new Dictionary<PlayerStateType, float>();
    public Dictionary<PlayerStateType, float> lastStateExitTime = new Dictionary<PlayerStateType, float>();

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerCollider = rb.GetComponent<Collider2D>();

        states = new Dictionary<PlayerStateType, BasePlayerState> {
            { PlayerStateType.OnGround, new OnGroundState(this, rb) },
            { PlayerStateType.Jumping, new JumpingState(this, rb) },
            { PlayerStateType.Falling, new FallingState(this, rb) },
            { PlayerStateType.Dashing, new DashingState(this, rb) },
            { PlayerStateType.WallSliding, new WallSlidingState(this, rb) },
            { PlayerStateType.WallJumping, new WallJumpingState(this, rb) },
            { PlayerStateType.WallHolding, new WallHoldingState(this, rb) },
        };

        foreach (PlayerStateType stateType in Enum.GetValues(typeof(PlayerStateType))) {
            // Debug.Log("adding state type: " + stateType);
            lastStateEnterTime.Add(stateType, 0f);
            lastStateExitTime.Add(stateType, 0f);
        }

        ChangePlayerState(PlayerStateType.Falling);
    }

    // Update is called once per frame
    void Update() {
        GetColliders();
        GroundCheck();
        WallCheck();
        PressAgainstWallCheck();
        TryEnterWallHoldingState();
        EnableBuoyancy();

        currentState.OnUpdate();
    }

    // physics bad
    void FixedUpdate() {
        if (Math.Abs(rb.linearVelocityX) <= 1e-1f) rb.linearVelocityX = 0;
        if (Math.Abs(rb.linearVelocityY) <= 1e-5f) rb.linearVelocityY = 0;
        UpdateFacingDirection();
        currentState.OnFixedUpdate();
    }

    public void ChangePlayerState(PlayerStateType newState) {
        currentState?.OnExitState();
        prevStateType = currentStateType;
        currentStateType = newState;
        currentState = states[newState];
        currentState.OnEnterState();

        float now = Time.time;
        lastStateChangeTime = now;
        lastStateEnterTime[currentStateType] = now;
        lastStateExitTime[prevStateType] = now;
    }

    public void Move(InputAction.CallbackContext context) {
        if (context.performed && (isInRoom || context.ReadValue<Vector2>() == roomDirection)) {
            movementInput = context.ReadValue<Vector2>();
            if (movementInput.magnitude == 0f) return;
            // Debug.Log($"moving in state handler {movementInput}");
            currentState.OnMove(context);
        } else if (context.canceled) {
            // Debug.Log("move canceled in state handler");
            movementInput = Vector2.zero;
            currentState.OnMoveCanceled(context);
        } else if (context.started) {
            // Debug.Log("move started");
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && isInRoom) {
            // Debug.Log("jumping in state handler");
            currentState.OnJump(context);
        } else if (context.canceled) {
            // Debug.Log("jump canceled in state handler");
            currentState.OnJumpCanceled(context);
        }
    }

    // add check to avoid recharge when falling down besides ground tiles
    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && LevelManager.instance.abilityUnlocked[Shrine.Ability.Dash] && isInRoom) {
            if ((Time.time > dashLastUse + dashTime + dashCooldown) && (canDash || PlayerManager.instance.CanInfiniteDash(GetCharacter()))) {
                // Debug.Log("dashing in state handler");
                AudioManager.instance.PlaySoundEffect(dashSound, 0.5f);
                dashLastUse = Time.time;
                canDash = false;
                ChangePlayerState(PlayerStateType.Dashing);
            }
        }
    }

    public void AbilityUse(InputAction.CallbackContext context) {
        if (context.performed && isInRoom) {
            Debug.Log("using ability in state handler");
            currentState.OnAbilityUse(context);
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed && isInRoom) {
            // Debug.Log("interacting in state handler" + context.performed);
            currentState.OnInteract(context);
        }
    }

    public void Pause(InputAction.CallbackContext context) {
        if (context.performed) {
            MenuManager.instance.OpenMenu(MenuManager.Menu.Pause);
        }
    }

    public void WallHold(InputAction.CallbackContext context) {
        if (context.performed) {
            // Debug.Log("Holding in state handler");
            isWallHolding = true;
        } else if (context.canceled) {
            isWallHolding = false;
        }
    }

    public void Respawn(InputAction.CallbackContext context) {
        // The whole dying inmplementation seems a bit roundabout, where is the best place to implement dying?
        if (context.performed && LevelManager.instance != null) {
            LevelManager.instance.PlayDeathAnimation(gameObject);
            LevelManager.instance.KillPlayer(gameObject);
            LevelManager.instance.RespawnPlayer(gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        // collider for smooth corner jumping
        Gizmos.color = Color.yellow;
        Vector2 checkPos = wallCheckPos.position;
        checkPos.y += 1f;
        float radius = 0.05f;
        Gizmos.DrawWireSphere(checkPos, radius);
    }

    private void GetColliders() {
        groundColliders = Physics2D.OverlapBoxAll(groundCheckPos.position, groundCheckSize, 0, solidLayer);
        wallColliders = Physics2D.OverlapBoxAll(wallCheckPos.position, wallCheckSize, 0, solidLayer);
    }

    private void GroundCheck() {
        // this prevents infinitely jumping/dashing inside one-way platforms
        isGrounded = groundColliders.Any(c =>
            c != ignoredPlatform && (
                !c.CompareTag("OneWayPlatform") ||
                playerCollider.IsTouching(c)
            )
        );
    }

    private void WallCheck() {
        isWalled = wallColliders.Any(c =>
            !c.CompareTag("OneWayPlatform") &&
            !c.CompareTag("FloatingPlatform") &&
            !c.CompareTag("FloatingPlatformStatic") &&
            !c.CompareTag("Felix") &&
            !c.CompareTag("Anna") &&
            !c.CompareTag("NonClimbable")
        );
    }

    private void PressAgainstWallCheck() {
        pressAgainstWall = (isFacingRight && movementInput.x > 0) || (!isFacingRight && movementInput.x < 0);
    }

    public void FlipPlayer(bool right) {
        // Debug.Log("Rotating player" + currentStateType);
        int direction = right ? 1 : -1;
        transform.localScale = new Vector3(direction * MathF.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void UpdateFacingDirection() {
        if (isMovingRight && rb.linearVelocityX < 0) { isMovingRight = false; }
        else if (!isMovingRight && rb.linearVelocityX > 0) { isMovingRight = true; }

        // they're not moving so the variable name is confusing
        if (rb.linearVelocityX == 0 && movementInput.x > 0) { isMovingRight = true; }
        else if (rb.linearVelocityX == 0 && movementInput.x < 0) { isMovingRight = false; }

        if ((currentStateType != PlayerStateType.WallHolding) && (isMovingRight != isFacingRight)) {
            isFacingRight = isMovingRight;
            FlipPlayer(isFacingRight);
        }
    }

    private void TryEnterWallHoldingState() {
        if (currentStateType == PlayerStateType.WallHolding) return;
        if ((Time.time - lastStateEnterTime[PlayerStateType.WallJumping]) < 0.1) return;

        if (isWallHolding && isWalled && climbStamina > 0) {
            ChangePlayerState(PlayerStateType.WallHolding);
        }
    }

    private void EnableBuoyancy() {
        foreach (GameObject platform in GameObject.FindGameObjectsWithTag("FloatingPlatform")) {
            Collider2D platformCollider = platform.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(
                playerCollider,
                platformCollider,
                groundColliders.Length == 0 // counterintuitive logic
            );
        }
    }

    private CharacterName GetCharacter() {
        return CompareTag("Felix") ? CharacterName.Felix : CharacterName.Anna;
    }

    public void Die() {
        // Debug.Log("Player died </3");
        canDash = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Respawn() {
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ChangePlayerState(PlayerStateType.Falling);
    }

    public void OnRoomExit(Vector2 exitDirection) {
        isInRoom = false;
        roomDirection = - exitDirection;
        movementInput = Vector2.zero;
    }

    public void OnRoomEnter() {
        isInRoom = true;
        roomDirection = Vector2.zero;
    }
}
