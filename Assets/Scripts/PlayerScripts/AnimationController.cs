using System;
using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    // Assign in Inspector
    public Animator animator;
    public Animator deathAnimation;
    public float idleDelay;
    [Header("Particle Systems")]
    public ParticleSystem dashTrail;
    public ParticleSystem groundParticles;
    public float emissionRate = 15f;
    [Header("Climb Stamina")]
    public Color staminaDrainColor;
    public float staminaDrainStart; // percentage
    public float staminaFlashingIntervalMin;
    public float staminaFlashingIntervalMax;
    //

    private PlayerStateHandler h;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem.EmissionModule emission;
    private Color spriteColor;
    private bool isIdle = false;
    private bool isCrouching = false;
    private bool isDashing = false;
    private bool isClimbing = false;
    private bool isFlashing = false;
    private float idleTime = 0;


    private void Start() {
        h = GetComponentInParent<PlayerStateHandler>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        emission = groundParticles.emission;
        spriteColor = spriteRenderer.color;
    }

    private void Update() {
        // Idle
        bool currentlyIdling = h.currentStateType == PlayerStateType.OnGround && h.movementInput == Vector2.zero;
        if (currentlyIdling) {
            idleTime += Time.deltaTime;
            if (idleTime >= idleDelay) { isIdle = true; }
        } else {
            idleTime = 0;
            isIdle = false;
        }

        // Crouching
        isCrouching = h.movementInput == Vector2.down;

        // Dash
        bool currentlyDashing = h.currentStateType == PlayerStateType.Dashing;
        if (isDashing != currentlyDashing) {
            isDashing = currentlyDashing;
            if (isDashing) {
                bool shouldFlip = Mathf.Sign(dashTrail.transform.localScale.x) != (h.isFacingRight ? 1 : -1);
                if (shouldFlip) {
                    Flip(dashTrail.transform);
                }
                dashTrail.Play();
            } else {
                dashTrail.Stop();
            }
        }

        // Flip
        int targetScale = 1;
        if (isIdle) {
            // Always face right when idle
            targetScale = h.isFacingRight ? 1 : -1;
        } else if (AirBound() && !WallJumping()) {
            // Face in input direction when air-bound
            bool shouldFlip =
                (h.movementInput.x > 0 && !h.isFacingRight) ||
                (h.movementInput.x < 0 && h.isFacingRight);
            targetScale = shouldFlip ? -1 : 1;
        }
        if (MathF.Sign(transform.localScale.x) != targetScale) {
            Flip(transform);
        }

        // Climb stamina
        isClimbing = h.currentStateType == PlayerStateType.WallHolding;
        if (isClimbing && !isFlashing && (h.climbStamina / h.climbBaseStamina <= staminaDrainStart)) {
            StartCoroutine(nameof(Flashing));
        }
        if (!isClimbing && h.climbStamina <= 0) {
            isFlashing = false;
            spriteRenderer.color = staminaDrainColor;
        } else if (!isClimbing && h.climbStamina > staminaDrainStart) {
            isFlashing = false;
            spriteRenderer.color = spriteColor;
        }

        // Animator parameters
        animator.SetInteger("State", (int) h.currentStateType);
        animator.SetBool("isMovingX", h.movementInput.x != 0);
        animator.SetBool("isMovingY", h.movementInput.y != 0);
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isCrouching", isCrouching);
    }

    private bool AirBound() {
        return h.currentStateType
            is PlayerStateType.Jumping
            or PlayerStateType.WallJumping
            or PlayerStateType.Falling;
    }

    private bool WallJumping() {
        float timeSinceWallJump = Time.time - h.lastStateEnterTime[PlayerStateType.WallJumping];
        return (h.currentStateType == PlayerStateType.WallJumping) && (timeSinceWallJump < h.wallJumpMoveDelay);
    }

    private void Flip(Transform targetTransform) {
        Vector3 scale = targetTransform.localScale;
        targetTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    public void PlayDeathAnimation() {
        deathAnimation.SetTrigger("playerDied");
    }

    private IEnumerator Flashing() {
        isFlashing = true;
        float normalized;
        float flashInterval;
        while (isClimbing && h.climbStamina > 0) {
            normalized = Mathf.Clamp((staminaDrainStart - (h.climbStamina / h.climbBaseStamina)) / staminaDrainStart, 0, 1);
            flashInterval = staminaFlashingIntervalMax - (staminaFlashingIntervalMax - staminaFlashingIntervalMin) * normalized;
            spriteRenderer.color = staminaDrainColor;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = spriteColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
