using UnityEngine;

public class Trampoline : MonoBehaviour {

    // Assign in Inspector
    public float bounceForceNoJump;
    public float bounceForceJump;
    public Animator animator;
    public AudioClip bounceSound;
    //

    private Collider2D player;
    private Collider2D trampoline;
    private bool bounceStarted;
    private bool bounceReady;

    private void Start() {
        trampoline = GetComponent<EdgeCollider2D>();
        bounceStarted = false;
        bounceReady = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!bounceStarted) {
            player = other;
            bounceStarted = true;
            animator.SetTrigger("OnPlayerCollision");
        }
    }

    private void OnTriggerExit2D (Collider2D other) {
        if (bounceReady) {
            // Debug.Log("exit");
            AudioManager.instance.PlaySoundEffect(bounceSound, 0.3f);
            bounceReady = false;
            player.attachedRigidbody.AddForceY(bounceForceJump, ForceMode2D.Impulse);
        }
    }

    public void OnBounceReady() {
        bounceReady = true;
    }

    // Sometimes player jumps really high, I don't get why. It seems to happen on finish, not exit.
    public void OnBounceFinish() {
        if (Physics2D.IsTouching(trampoline, player)) {
            // Debug.Log("finish");
            player.attachedRigidbody.AddForceY(bounceForceNoJump, ForceMode2D.Impulse);
        }
        bounceStarted = false;
        bounceReady = false;
    }
}
