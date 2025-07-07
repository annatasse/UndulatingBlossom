using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GoldenMushroom : MonoBehaviour {

    // Assign in Inspector
    public Room finalRoom;
    public AudioClip spawnSound;
    public GameObject goldenMushroomCompletionist; // for collecting all regular mushrooms
    public GameObject[] goldenMushroomsZeroDeaths; // with player-specific death counts
    public Sprite goldenMushroomNormal;
    public Sprite goldenMushroomOutline;
    public float spawnDelay;
    //

    private bool spawned = false;

    void Start() {
        goldenMushroomCompletionist.SetActive(false);
        foreach (GameObject mushroom in goldenMushroomsZeroDeaths) {
            mushroom.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;

        if (!spawned && LevelManager.instance.currentRoom == finalRoom) {
            spawned = true;
            Invoke(nameof(SpawnMushrooms), spawnDelay);
        }
    }

    private void SpawnMushrooms() {
        // spawn completionist shroom
        goldenMushroomCompletionist.GetComponentInChildren<InputPrompt>().suffix = $"{LevelManager.instance.mushroomsCollected} / {LevelManager.instance.totalMushroomCount}";
        if (LevelManager.instance.mushroomsCollected == LevelManager.instance.totalMushroomCount) {
            SpawnUnlocked(goldenMushroomCompletionist);
        } else {
            SpawnLocked(goldenMushroomCompletionist);
        }

        // spawn zero death shrooms
        foreach (GameObject mushroom in goldenMushroomsZeroDeaths) {
            CharacterName character = mushroom.CompareTag("Felix") ? CharacterName.Felix : CharacterName.Anna;
            Debug.Log(LevelManager.instance.deathCounts[character].ToString());
            Debug.Log(LevelManager.instance.deathCounts[character]);
            mushroom.GetComponentInChildren<InputPrompt>().suffix = LevelManager.instance.deathCounts[character].ToString();
            if (LevelManager.instance.deathCounts[character] == 0) {
                SpawnUnlocked(mushroom);
            } else {
                SpawnLocked(mushroom);
            }
        }
        AudioManager.instance.PlaySoundEffect(spawnSound);
    }

    private void SpawnUnlocked(GameObject mushroom) {
        mushroom.GetComponent<SpriteRenderer>().sprite = goldenMushroomNormal;
        mushroom.SetActive(true);
        mushroom.GetComponent<ParticleSystem>().Play();
    }

    private void SpawnLocked(GameObject mushroom) {
        mushroom.GetComponent<SpriteRenderer>().sprite = goldenMushroomOutline;
        mushroom.GetComponent<Light2D>().enabled = false;
        mushroom.GetComponent<CircleCollider2D>().enabled = false;
        mushroom.SetActive(true);
    }
}
