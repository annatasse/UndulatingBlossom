using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BasePlayerState {

    protected PlayerStateHandler h;
    protected Rigidbody2D rb;

    protected BasePlayerState(PlayerStateHandler handler, Rigidbody2D rb) {
        this.h = handler;
        this.rb = rb;
    }

    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public virtual void OnEnterState() {
        Debug.Log($"Entering {h.currentStateType}");
    }
    public virtual void OnExitState() {
        Debug.Log($"Exiting {h.currentStateType}");
    }

    // empty instead of NotImplementedError so you can press all buttons without causing unnecessary error logs
    public virtual void OnAbilityUse(InputAction.CallbackContext context) { }
    public virtual void OnInteract(InputAction.CallbackContext context) { }
    public virtual void OnJump(InputAction.CallbackContext context) { }
    public virtual void OnJumpCanceled(InputAction.CallbackContext context) { }
    public virtual void OnMove(InputAction.CallbackContext context) { }
    public virtual void OnMoveCanceled(InputAction.CallbackContext context) { }


    // These might belong in StateHandler instead
    protected void GroundMovement() {
        rb.linearVelocityX = h.movementInput.x * h.moveSpeed;
    }

    protected void AirMovement() {
        if (h.movementInput.x == 0) {
            // slow stop in air
            rb.linearVelocityX *= h.airFrictionFactor;
        } else {
            // prevent instant direction change
            float targetSpeed = h.movementInput.x * h.moveSpeed;
            rb.linearVelocityX += h.movementInput.x * h.airDirectionFactor;
            rb.linearVelocityX = h.movementInput.x > 0
                ? MathF.Min(rb.linearVelocityX, targetSpeed)
                : MathF.Max(rb.linearVelocityX, targetSpeed);
        }
    }
}
