using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peekaboo : MonoBehaviour {

    public float fadeDuration = 1;
    private List<SpriteRenderer> spriteRenderers = new();
    private Color[] originalColors;

    private void Start() {
        GetComponentsInChildren(spriteRenderers);
        if (TryGetComponent(out SpriteRenderer spriteRenderer)) {
            spriteRenderers.Add(spriteRenderer);
        }
        originalColors = new Color[spriteRenderers.Count];
        for (int i = 0; i < spriteRenderers.Count; i++) {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(nameof(Fade));
    }

    // private void OnTriggerExit2D(Collider2D other) {
    //     if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;
    //     for (int i = 0; i < spriteRenderers.Count; i++) {
    //         spriteRenderers[i].color = originalColors[i];
    //     }
    // }

    private IEnumerator Fade() {
        float duration = 0;
        float fadePercentage;
        yield return 0;
        while (duration < fadeDuration) {
            duration += Time.deltaTime;
            fadePercentage = duration / fadeDuration;
            for (int i = 0; i < spriteRenderers.Count; i++) {
                 originalColors[i].a = 1 - fadePercentage;
                spriteRenderers[i].color = originalColors[i];
            }
            yield return 0;
        }
        Destroy(gameObject);
    }
}
