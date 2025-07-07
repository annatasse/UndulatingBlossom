using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Gem : MonoBehaviour {

    // Assign in Inspector
    public CharacterName associatedCharacter;
    public AudioClip pickupSound;
    //

    public bool isCollected {get; private set;} = false;

    private Collider2D attachedCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Light2D glow;
    private Dictionary<CharacterName, Color32> colors = new();


    private void Start() {
        colors.Add(CharacterName.Felix, new Color32(0, 212, 255, 255));
        colors.Add(CharacterName.Anna, new Color32(228, 46, 12, 255));
        attachedCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        glow = GetComponent<Light2D>();
        Activate();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag(associatedCharacter.ToString())) return;

        AudioManager.instance.PlaySoundEffect(pickupSound);
        Deactivate();
    }

    public void Activate() {
        isCollected = false;
        spriteRenderer.color = colors[associatedCharacter];
        glow.color = colors[associatedCharacter];
        glow.enabled = true;
        attachedCollider.enabled = true;
        animator.SetBool("isActive", true);
    }

    private void Deactivate() {
        isCollected = true;
        spriteRenderer.color = Color.gray;
        glow.enabled = false;
        attachedCollider.enabled = false;
        animator.SetBool("isActive", false);
    }
}
