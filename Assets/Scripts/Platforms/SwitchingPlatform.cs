using UnityEngine;

public class SwitchingPlatform : MonoBehaviour {

    private bool isActive;
    public float inactiveTransparency = 0.2f;
    private Collider2D platformCollider;
    private Renderer platformRenderer;  // encompasses both SpriteRenderer and TilemapRenderer
    private Color color;

    private void Start() {
        isActive = CompareTag("SwitchingPlatformOn");
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<Renderer>();
        color = platformRenderer.material.color;
        platformCollider.enabled = isActive;
        color.a = isActive ? 1 : inactiveTransparency;
        platformRenderer.material.color = color;
        // gameObject.SetActive(isActive);
    }

    public void Toggle() {
        isActive = !isActive;
        platformCollider.enabled = isActive;
        color.a = isActive ? 1 : inactiveTransparency;
        platformRenderer.material.color = color;
        // gameObject.SetActive(isActive);
    }

    // Static method that can be called from PlayerStateHandler
    public static void ToggleAll() {
        if (!LevelManager.instance.abilityUnlocked[Shrine.Ability.SwitchingPlatforms]) { return; }
        SwitchingPlatform[] platforms = FindObjectsByType<SwitchingPlatform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        // Debug.Log($"Toggling all switching platforms: {platforms.Length}");
        foreach (SwitchingPlatform platform in platforms) {
            platform.Toggle();
        }
    }
}
