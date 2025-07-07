using UnityEngine;

public class EnableAbilities : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Felix") || other.CompareTag("Anna")) {
            LevelManager.instance.UnlockAbility(Shrine.Ability.Dash);
            LevelManager.instance.UnlockAbility(Shrine.Ability.SpawningPlatforms);
            LevelManager.instance.UnlockAbility(Shrine.Ability.SwitchingPlatforms);
        }
    }
}
