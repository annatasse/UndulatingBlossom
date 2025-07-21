using UnityEngine;

public class EnableAbilities : MonoBehaviour {

    private bool activated;
    private Room room;

    public void Start() {
        activated = false;
        room = GetComponent<Room>();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;

        if (!activated && LevelManager.instance.currentRoom == room) {
            LevelManager.instance.UnlockAbility(Shrine.Ability.Dash);
            LevelManager.instance.UnlockAbility(Shrine.Ability.SpawningPlatforms);
            LevelManager.instance.UnlockAbility(Shrine.Ability.SwitchingPlatforms);
            activated = true;
        }
    }
}
