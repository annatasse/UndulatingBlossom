using UnityEngine;

public class WallJumpingState : BasePlayerState {

    private float remainingMoveDelay;

    public WallJumpingState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (rb.linearVelocityY <= 0 || rb.linearVelocityX == 0) {
            h.ChangePlayerState(PlayerStateType.Falling);
        }
        remainingMoveDelay -= Time.deltaTime;
    }

    public override void OnFixedUpdate() {
        if (remainingMoveDelay > 0) return;
        AirMovement();
    }

    public override void OnEnterState() {
        // Debug.Log("entered walljumpingstate");
        remainingMoveDelay = h.wallJumpMoveDelay;
        int jumpDirection = h.isFacingRight ? -1 : 1;
        rb.linearVelocity = new Vector2(jumpDirection * h.wallJumpPowerX, h.wallJumpPowerY);
    }

    public override void OnExitState() {

    }
}
