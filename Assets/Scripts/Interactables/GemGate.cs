using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GemGate : MonoBehaviour, IResettable {

    // Assign in Inspector
    public List<Gem> gems;
    public TextMeshProUGUI gemCountDisplay;
    public Vector2 movement;
    public float movementSpeed;
    public AudioClip unlockSound;
    //

    private Rigidbody2D rb;
    private bool isOpen;
    private bool isMoving;
    private Vector2 openPosition;
    private int activeGemCount = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        openPosition = rb.position + movement;
    }

    private void Update() {
        if (activeGemCount != ActiveGemCount()) {
            activeGemCount = ActiveGemCount();
            gemCountDisplay.text = activeGemCount > 0 ? activeGemCount.ToString() : "";
            if (activeGemCount == 0) AudioManager.instance.PlaySoundEffect(unlockSound);
        }
        if (!isOpen && AllGemsCollected()) {
            isOpen = true;
            isMoving = true;
        }
    }

    private void FixedUpdate() {
        if (isMoving) {
            rb.MovePosition(Vector2.MoveTowards(rb.position, openPosition, Time.fixedDeltaTime * movementSpeed));
            if (rb.position == openPosition) { isMoving = false; }
        }
    }

    private bool AllGemsCollected() {
        return gems.All(gem => gem.isCollected);
    }

    private int ActiveGemCount() {
        return gems.Where(gem => !gem.isCollected).Count();
    }

    public void ResetWithDeath() {
        if (!isOpen) {
            gems.ForEach(gem => gem.Activate());
        }
    }

    public void ResetWithRoom() {
        if (!isOpen) {
            gems.ForEach(gem => gem.Activate());
        }
    }
}
