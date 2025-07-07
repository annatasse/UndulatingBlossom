using UnityEngine;
using UnityEngine.InputSystem;

public class WallSlidingState : BasePlayerState {

    public WallSlidingState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (h.isGrounded) {
            h.ChangePlayerState(PlayerStateType.OnGround);
        } else if (!h.isWalled || !h.pressAgainstWall) {
            h.ChangePlayerState(PlayerStateType.Falling);
        }
    }

    public override void OnFixedUpdate() {
        // rb.gravityScale = h.wallSlideGravity; // revert if bug found
    }

    public override void OnEnterState() {
        rb.gravityScale = h.wallSlideGravity;
        rb.linearVelocityY = 0;
    }

    public override void OnExitState() {
        rb.gravityScale = h.baseGravity;
    }

    public override void OnJump(InputAction.CallbackContext context) {
        // AudioManager.instance.PlaySoundEffect(h.jumpSound, 0.2f);
        if (h.CompareTag("Felix")) { SwitchingPlatform.ToggleAll(); }
        h.ChangePlayerState(PlayerStateType.WallJumping);
    }
}
