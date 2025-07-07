using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// Idle + Running
public class OnGroundState : BasePlayerState {

    public OnGroundState(PlayerStateHandler h, Rigidbody2D rb) : base(h, rb) { }

    public override void OnUpdate() {
        if (!h.isGrounded) {
            h.ChangePlayerState(PlayerStateType.Falling);
        }
    }

    public override void OnFixedUpdate() {
        // Debug.Log($"Moving in RunningState: {h.movementInput:F8} {rb.linearVelocity:F8}");
        GroundMovement();
    }

    public override void OnEnterState() {
        // Debug.Log($"Entering RunningState: {h.movementInput}");
        if (!h.groundColliders.Any(c => c.CompareTag("FloatingPlatform"))) {
            h.canDash = true; // doing this in the state handler allows unwanted double jumps
        }
        h.climbStamina = h.climbBaseStamina;
    }

    public override void OnExitState() {
        // Debug.Log($"Exiting RunningState: {h.movementInput}");
    }

    public override void OnInteract(InputAction.CallbackContext context) {
        Debug.Log("I feel touched");
        Shrine.TryInteract(rb.position);
    }

    public override void OnJump(InputAction.CallbackContext context) {
        // jumping down while standing on only one one-way platform
        if (OneWayPlatformDown()) {
            DisablePlatformCollision();
            h.ChangePlayerState(PlayerStateType.Falling);
        } else {
            // AudioManager.instance.PlaySoundEffect(h.jumpSound, 0.2f);
            if (h.CompareTag("Felix")) { SwitchingPlatform.ToggleAll(); }
            h.ChangePlayerState(PlayerStateType.Jumping);
        }
    }

    // TODO: doesn't work with 2 platforms above each other
    private bool OneWayPlatformDown() {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(h.groundCheckPos.position, h.groundCheckSize, 0, h.solidLayer);
        return h.movementInput == Vector2.down && colliders.Length == 1 && colliders[0].CompareTag("OneWayPlatform");
    }

    private void DisablePlatformCollision() {
        Collider2D playerCollider = rb.GetComponent<Collider2D>();
        Collider2D platformCollider = Physics2D.OverlapBoxAll(h.groundCheckPos.position, h.groundCheckSize, 0, h.solidLayer)[0];

        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        Debug.Log($"Ignoring collision with {platformCollider.name}");

        h.ignoredPlatform = platformCollider;
        h.StartCoroutine(ReenableCollision(playerCollider, platformCollider));
    }

    // Wait until player is no longer overlapping the platform
    // other methods like IsTouching() don't work here since collision is ignored
    private IEnumerator ReenableCollision(Collider2D playerCollider, Collider2D platformCollider) {
        float T = Time.time;
        bool stillOverlapping = true;
        List<Collider2D> overlaps = new();
        while (stillOverlapping) {
            Physics2D.OverlapCollider(platformCollider, overlaps);
            if (overlaps.Find(collider => collider == playerCollider) == null) {
                stillOverlapping = false;
            }
            yield return null;
        }
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        Debug.Log($"Re-enabled collision after {Time.time - T} seconds");
        h.ignoredPlatform = null;
    }
}
