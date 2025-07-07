using UnityEngine;
using UnityEngine.InputSystem;

// in air while boosting upwards
public class JumpingState : BasePlayerState {

    public JumpingState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (rb.linearVelocityY <= 0 || (h.isWalled && h.pressAgainstWall && SolidAbove())) {
            h.ChangePlayerState(PlayerStateType.Falling);
        }
    }

    public override void OnFixedUpdate() {
        // Debug.Log($"Moving in {h.currentStateType}: {h.movementInput}");
        AirMovement();
    }

    public override void OnEnterState() {
        // Debug.Log("Entering JumpingState");
        rb.AddForceY(h.jumpPower, ForceMode2D.Impulse);
    }

    public override void OnExitState() {
        // Debug.Log("Exiting JumpingState");
    }

    public override void OnJumpCanceled(InputAction.CallbackContext context) {
        rb.linearVelocityY /= 2;
        h.ChangePlayerState(PlayerStateType.Falling);
    }

    private bool SolidAbove() {
        Vector2 checkPos = h.wallCheckPos.position;
        checkPos.y += 1f;
        float radius = 0.05f;
        bool isSolidAbove = Physics2D.OverlapCircle(checkPos, radius, h.solidLayer);
        // Debug.Log(isSolidAbove ? "Solid above detected" : "No solid above");
        return isSolidAbove;
    }
}
