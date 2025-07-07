using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : BasePlayerState {

    public FallingState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (h.isGrounded) {
            h.ChangePlayerState(PlayerStateType.OnGround);
        } else if (h.isWalled && h.pressAgainstWall) {
            h.ChangePlayerState(PlayerStateType.WallSliding);
        }
    }

    public override void OnFixedUpdate() {
        rb.linearVelocityY = MathF.Max(rb.linearVelocityY, -h.maxFallSpeed);
        AirMovement();
    }

    public override void OnEnterState() {
        // Debug.Log("entered falling");
        // Fast falling
        rb.gravityScale = h.baseGravity * h.fallSpeedMultiplier;
    }

    public override void OnExitState() {
        rb.gravityScale = h.baseGravity;
    }

    public override void OnJump(InputAction.CallbackContext context) {
        if (h.isWalled) {
            // AudioManager.instance.PlaySoundEffect(h.jumpSound, 0.2f);
            if (h.CompareTag("Felix")) { SwitchingPlatform.ToggleAll(); }
            h.ChangePlayerState(PlayerStateType.WallJumping);
        }
    }
}
