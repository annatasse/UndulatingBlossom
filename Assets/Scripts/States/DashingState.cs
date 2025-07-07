using UnityEngine;
using UnityEngine.InputSystem;

public class DashingState : BasePlayerState {

    private float remainingDashTime;
    private Vector2 dashDirection;
    private bool isJumping;
    public DashingState(PlayerStateHandler handler, Rigidbody2D rb) : base(handler, rb) { }

    public override void OnUpdate() {
        if (remainingDashTime > 0) {
            remainingDashTime -= Time.deltaTime;
            return;
        }
        if (isJumping && !h.isGrounded && !h.isWalled) {
            return;
        }
        h.ChangePlayerState(PlayerStateType.Falling);
    }

    public override void OnFixedUpdate() {}

    public override void OnEnterState() {
        dashDirection = h.movementInput.normalized; // normalize to reduce diagonal boost, test
        if (dashDirection == Vector2.zero) {
            dashDirection = h.isFacingRight ? Vector2.right : Vector2.left;
        }

        rb.linearVelocity = dashDirection * h.dashSpeed;
        rb.gravityScale = 0;
        remainingDashTime = h.dashTime;
        isJumping = false;
    }

    public override void OnExitState() {
        rb.gravityScale = h.baseGravity;
        rb.linearVelocityX /= 4; // x is immediately overwritten in fallingstate
        rb.linearVelocityY /= 4; // feels good for now
    }

    public override void OnJump(InputAction.CallbackContext context) {
        if (h.isGrounded && (dashDirection == Vector2.left || dashDirection == Vector2.right)) {
            // AudioManager.instance.PlaySoundEffect(h.jumpSound, 0.2f);
            if (h.CompareTag("Felix")) { SwitchingPlatform.ToggleAll(); }
            isJumping = true;
            rb.AddForceY(h.jumpPower, ForceMode2D.Impulse);
            rb.gravityScale = 2 * h.baseGravity; // feels good enough for now
        }
    }
}
