using UnityEngine;

public class AbilityArea : MonoBehaviour {

    // Assign in Inspector
    public Shrine.Ability ability;
    public bool allowFelix = true;
    public bool allowAnna = true;
    //

    private void OnTriggerEnter2D(Collider2D other) {
        if ((other.CompareTag("Anna") && allowAnna) || (other.CompareTag("Felix") && allowFelix)) {
            LevelManager.instance.UnlockAbility(ability);
          }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if ((other.CompareTag("Anna") && allowAnna) || (other.CompareTag("Felix") && allowFelix)) {
            LevelManager.instance.LockAbility(ability);
        }
    }
}
