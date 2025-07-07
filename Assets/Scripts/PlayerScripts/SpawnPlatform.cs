using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpawnPlatform : MonoBehaviour, IResettable {

    // Assign in Inspector
    public GameObject platformPrefab;
    public float spawnOffset = -1f;
    public float platformLifetime = 5f;
    public int maxPlatforms = 2;
    public AudioClip spawnSound;
    public LayerMask exceptionLayer;
    public float minSpawnDelay = 0.2f;
    //

    private PlayerStateHandler h;
    private List<GameObject> activePlatforms = new();
    private float lastSpawnTime = 0f;

    private void Start() {
        h = GetComponent<PlayerStateHandler>();
    }

    public void Spawn(InputAction.CallbackContext context) {
        if (!LevelManager.instance.abilityUnlocked[Shrine.Ability.SpawningPlatforms]) return;
        if (h.CompareTag("Anna") && context.performed) {
            if (Time.time - lastSpawnTime < minSpawnDelay) return;

            Vector3 spawnPosition = GetCloudSpawnPosition();

            // Check if position is empty, OverlapPoint doesn't work
            Collider2D overlap = Physics2D.OverlapBox(spawnPosition, new Vector2(0.01f, 0.01f), 0, exceptionLayer);
            if (overlap != null) {
                // Debug.Log($"Cannot spawn platform here, position is occupied by {overlap.name}");
                return;
            }

            if (activePlatforms.Count >= maxPlatforms) {
                // Debug.Log("Maximum number of platforms reached, destroying oldest platform.");
                Destroy(activePlatforms[0]);
                activePlatforms.RemoveAt(0);
            }

            SpawnCloud(spawnPosition);
            lastSpawnTime = Time.time;
        }
    }

    // Get position and snap to grid
    private Vector3 GetCloudSpawnPosition() {
        Vector3 playerPos = transform.position;
        float snappedX = Mathf.Round(playerPos.x);
        float snappedY = Mathf.Round(playerPos.y + spawnOffset);
        return new Vector3(snappedX, snappedY, 0);
    }

    private void SpawnCloud(Vector3 spawnPosition) {
        AudioManager.instance.PlaySoundEffect(spawnSound, 1.0f);
        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        activePlatforms.Add(platform);
        StartCoroutine(DestroyPlatformAfterDelay(platform));
    }


    // Destroy the platform after lifetime and remove from list
    private System.Collections.IEnumerator DestroyPlatformAfterDelay(GameObject platform) {
        yield return new WaitForSeconds(platformLifetime);
        if (activePlatforms.Contains(platform)) {
            activePlatforms.Remove(platform);
            Destroy(platform);
        }
    }

    public void ResetWithDeath() {
        return;
    }

    public void ResetWithRoom() {
        activePlatforms.ForEach(platform => Destroy(platform));
        activePlatforms.Clear();
    }

    private void OnDestroy() {
        activePlatforms.ForEach(platform => Destroy(platform));
    }
}
