using UnityEngine;

public class GrowingVines : MonoBehaviour {

    // Assign in Inspector
    public Mushroom mushroom;
    //

    private Collider2D vineGollider;
    private Animator animator;
    private bool isGrown = false;

    private void Start() {
        vineGollider = GetComponent<Collider2D>();
        vineGollider.enabled = false;
        animator = GetComponent<Animator>();
    }

    public void Update(){
        if (!isGrown && mushroom.isCollected) {
            animator.SetTrigger("MushroomCollected");
            vineGollider.enabled = true;
            isGrown = true;
        }
    }
}
