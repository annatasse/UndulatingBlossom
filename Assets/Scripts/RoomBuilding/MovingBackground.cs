using Unity.Cinemachine;
using UnityEngine;

public class MovingBackground : MonoBehaviour {

    // Assign in Inspector
    public CinemachineCamera cam;
    public SpriteRenderer sprite;
    public Collider2D roomCollider;
    //

    private float yMin;
    private float yMax;

    private void Start() {
        yMin = roomCollider.bounds.min.y + sprite.size.y / 2;
        yMax = roomCollider.bounds.max.y - sprite.size.y / 2;
    }

    void Update() {
        Vector3 position  = transform.position;
        position.y = Mathf.Clamp(cam.transform.position.y, yMin, yMax);
        transform.position = position;
    }
}
