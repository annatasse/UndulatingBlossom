using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shrine : MonoBehaviour {
    public bool unlockable = true;
    public static Shrine activeShrine; // Reference to the current shrine
    public ParticleSystem interactionEffect;
    public ParticleSystem permanentEffect;
    public AudioClip interactionSound;
    public enum Ability {None = -1, Dash = 0, SpawningPlatforms = 1, SwitchingPlatforms = 2};
    public Ability abilityToLock = Ability.None;
    public Ability abilityToUnlock;
    public InputPrompt interactionPrompt;
    public InputPrompt interactionInfo;
    public float infoDisplayDelay = 0;
    public bool allowFelix = true;
    public bool allowAnna = true;
    public GameObject IncorrectCharacterFelixIcon;
    public GameObject IncorrectCharacterAnnaIcon;

    void Awake() {
        permanentEffect.Play();
        interactionInfo.gameObject.SetActive(false);
    }

    // Static method to call from state(handler)
    public static bool TryInteract(Vector3 playerPosition) {
        if (activeShrine == null) return false;
        if (activeShrine.unlockable == false) return false;
        activeShrine.UnlockAbility();
        activeShrine.unlockable = false;
        activeShrine.interactionPrompt.gameObject.SetActive(false);
        activeShrine.Invoke(nameof(DispalyInfoPrompt), activeShrine.infoDisplayDelay);
        return true;
    }

    private void UnlockAbility() {
        Debug.Log("Shrine interaction successful");
        permanentEffect.Stop();
        interactionEffect.Play();
        AudioManager.instance.PlaySoundEffect(interactionSound);

        if (abilityToLock != Ability.None) LevelManager.instance.LockAbility(abilityToLock);
        LevelManager.instance.UnlockAbility(abilityToUnlock);
    }

    // Set shrine active whenever a player is in interaction range
    private void OnTriggerEnter2D(Collider2D other) {
        if (allowFelix && other.CompareTag("Felix") ||
            allowAnna && other.CompareTag("Anna")) {
                Debug.Log("activate shrine");
                activeShrine = this;
        // display incorrect character icon
        } else if (allowAnna && allowFelix) {
            return;
        } else if (allowFelix && other.CompareTag("Anna")) {
            IncorrectCharacterAnnaIcon.SetActive(true);
        } else if (allowAnna && other.CompareTag("Felix")) {
            IncorrectCharacterFelixIcon.SetActive(true);
        }
    }

    // Deactivate shrine when no player is in interaction range
    private void OnTriggerExit2D(Collider2D other) {
        List<Collider2D> overlaps = new();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), overlaps);
        if (!overlaps.Any(collider => collider.CompareTag("Felix") || collider.CompareTag("Anna"))) {
            // Debug.Log("deactivate shrine");
            activeShrine = null;
        }
        IncorrectCharacterAnnaIcon.SetActive(false);
        IncorrectCharacterFelixIcon.SetActive(false);
     }

    private void DispalyInfoPrompt() {
        interactionInfo.gameObject.SetActive(true);
    }
}
