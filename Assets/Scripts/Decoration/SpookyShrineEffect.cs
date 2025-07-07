using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpookyShrineEffect : MonoBehaviour {

    // Assign in Inspector
    public Light2D glow;
    public Color adjustedColor;
    public float fadeTime;
    public Shrine shrine;
    //

    private bool lightAdjusted = false;

    void Update() {
        if (shrine.unlockable || lightAdjusted) { return; }
        lightAdjusted = true;
        StartCoroutine(nameof(FadeLight));
    }

    private IEnumerator FadeLight() {
        yield return new WaitForSeconds(shrine.interactionSound.length - fadeTime);
        float fadeDuration = 0;
        Color originalColor = glow.color;
        while (fadeDuration < fadeTime) {
            fadeDuration += Time.deltaTime;
            glow.color = Color.Lerp(originalColor, adjustedColor, fadeDuration/fadeTime);
            yield return 0;
        }
    }
}
