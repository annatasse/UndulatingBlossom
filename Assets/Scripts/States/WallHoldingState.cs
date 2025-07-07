using UnityEngine;
using UnityEngine.InputSystem;

public class WallHoldingState : BasePlayerState {

    public WallHoldingState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (h.climbStamina <= 0 || !h.isWalled || !h.isWallHolding) {
            h.ChangePlayerState(PlayerStateType.Falling);
        }
        if (!h.isGrounded) { // avoids drain while standing in a corner, ugly
            float currentDrain = (h.movementInput.y != 0) ? h.climbStaminaDrain : h.wallHoldStaminaDrain;
            h.climbStamina -= currentDrain * Time.deltaTime;
        }
    }

    public override void OnFixedUpdate() {
        // Debug.Log("moving");
        rb.linearVelocityY = h.movementInput.y * h.climbSpeed;
    }

    public override void OnEnterState() {
        if (h.climbStamina > 0) {
            rb.gravityScale = 0;
            rb.linearVelocityY = 0;
        } // avoid abuse
    }

    public override void OnExitState() {
        rb.gravityScale = h.baseGravity;
    }

    public override void OnMove(InputAction.CallbackContext context) {

    }

    public override void OnMoveCanceled(InputAction.CallbackContext context) {
        rb.linearVelocityY = 0;
    }

    public override void OnJump(InputAction.CallbackContext context) {
        // AudioManager.instance.PlaySoundEffect(h.jumpSound, 0.2f);
        if (h.CompareTag("Felix")) { SwitchingPlatform.ToggleAll(); }
        h.ChangePlayerState(PlayerStateType.WallJumping);
    }
}
