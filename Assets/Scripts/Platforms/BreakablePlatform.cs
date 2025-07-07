using UnityEngine;
using System.Linq;
using System.Collections;

public class BreakablePlatform : MonoBehaviour {

    // Assign in Inspector
    public bool isBroken = false;
    public int maxLowers = 5;
    public int lowerCount = 0;
    public float fallSpeed = 5f;
    public float lowerOffset = 0.2f; // How far the platform lowers each time
    public float generosityOffset = 0.4f; // How far above the platform to check for players, should be > lowerOffset
    public AudioClip lowerSound;
    public AudioClip breakSound;
    public Sprite[] breakingStages;
    //

    private Collider2D platformCollider;
    private Rigidbody2D platformRb;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] hits;
    private bool felixIsOnTop = false;
    private bool annaIsOnTop = false;
    private bool felixWasOnTop = false;
    private bool annaWasOnTop = false;
    private bool felixCanLower = true;
    private bool annaCanLower = true;
    private bool bothWereOffTop = true;

    private void Start() {
        platformCollider = GetComponent<Collider2D>();
        platformRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lowerSound = Instantiate(lowerSound, transform);
        breakSound = Instantiate(breakSound, transform);
    }

    private void Update() {
        if (!isBroken) CheckForPlayers();
    }

    private void CheckForPlayers() {
        hits = Physics2D.OverlapBoxAll(
            platformCollider.bounds.center + Vector3.up * generosityOffset,
            platformCollider.bounds.size, 0
        );
        felixIsOnTop = hits.Any(c => c.CompareTag("Felix"));
        annaIsOnTop = hits.Any(c => c.CompareTag("Anna"));

        if (lowerCount < 2) CheckForPlayersStageOne();
        else CheckForPlayersStageTwo();
    }

    private void CheckForPlayersStageOne() {
        felixWasOnTop |= felixIsOnTop;
        annaWasOnTop |= annaIsOnTop;

        felixCanLower |= !felixIsOnTop && felixWasOnTop;
        annaCanLower |= !annaIsOnTop && annaWasOnTop;

        if (felixCanLower && felixIsOnTop && annaCanLower && annaIsOnTop) {
            LowerPlatform();
            felixWasOnTop = false;
            felixCanLower = false;
            annaWasOnTop = false;
            annaCanLower = false;
        }
    }

    private void CheckForPlayersStageTwo() {
        if (felixIsOnTop && annaIsOnTop && bothWereOffTop) {
            LowerPlatform();
            bothWereOffTop = false;
        }
        if (!felixIsOnTop && !annaIsOnTop) {
            bothWereOffTop = true;
        }
    }

    private void LowerPlatform() {
        lowerCount++;
        platformRb.transform.Translate(Vector3.down * lowerOffset);

        if (lowerCount <= breakingStages.Length - 1) {
            spriteRenderer.sprite = breakingStages[lowerCount];
        }

        if (lowerCount > maxLowers) {
            AudioManager.instance.PlaySoundEffect(breakSound, 0.3f);
            StartCoroutine(Break());
        } else {
            AudioManager.instance.PlaySoundEffect(lowerSound, 0.6f);
        }
    }

    private IEnumerator Break() {
        isBroken = true;
        platformRb.bodyType = RigidbodyType2D.Dynamic;
        platformRb.gravityScale = fallSpeed;

        // fun physics effect
        var box = platformCollider as BoxCollider2D;
        if (box != null) {
            box.size = new Vector2(4.8f, box.size.y);
        }

        Destroy(gameObject, 3f);
        yield return null;
    }

    private void OnDrawGizmosSelected() {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = Color.yellow;
        Vector3 center = col.bounds.center + Vector3.up * generosityOffset;
        Vector3 size = col.bounds.size;
        Gizmos.DrawWireCube(center, size);
    }
}
