using Unity.Cinemachine;
using UnityEngine;

public class Sunrise : MonoBehaviour {

    // Assign in Inspector
    public Collider2D room;
    public CinemachineCamera cam;
    public float heightFullLight;
    public float heightCaveLight;
    //

    void Update() {
        float currentPosition = cam.transform.position.y;
        if (currentPosition < heightCaveLight) {
            LevelManager.instance.globalLight.intensity = 0.3f;
        } else if (currentPosition > heightFullLight) {
            LevelManager.instance.globalLight.intensity = 1f;
        } else {
            LevelManager.instance.globalLight.intensity = 0.3f + (currentPosition - heightCaveLight) * 0.7f / (heightFullLight - heightCaveLight);
        }
    }
}
