using System.Collections;
using UnityEngine;

public class BoostUp : MonoBehaviour {
    public Room targetRoom; // Assign in inspector - the specific room where this boost should work
    public float deactivationDelay = 1f;
    private AreaEffector2D areaEffector;

    void Start() {
        areaEffector = GetComponent<AreaEffector2D>();
        areaEffector.enabled = false;
    }

    void Update() {
        if (LevelManager.instance.currentRoom == targetRoom) {
            areaEffector.enabled = true;
        } else {
            if (areaEffector.enabled) {
                StartCoroutine(DeactivateEffector());
            }
        }
    }

    private IEnumerator DeactivateEffector() {
        yield return new WaitForSeconds(deactivationDelay);
        areaEffector.enabled = false;
    }
}
