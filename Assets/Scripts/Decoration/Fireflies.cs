using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fireflies : MonoBehaviour {

    // Assign in Inspector
    public ParticleSystem pSystem;
    public Light2D fireflyPrefab;
    //
    private ParticleSystem.Particle[] particles;
    private List<Light2D> fireflies = new();
    private float fadeoutTime = 0.5f;

    void Start() {
        particles = new ParticleSystem.Particle[pSystem.main.maxParticles];
        while (fireflies.Count < pSystem.main.maxParticles) {
            fireflies.Add(Instantiate(fireflyPrefab, pSystem.transform));
        }
    }

    void LateUpdate() {
        SetLights();
    }

    private void SetLights() {
        int particleCount = pSystem.GetParticles(particles);
        for (int i = 0; i < fireflies.Count; i++) {
            if (i < particleCount) {
                fireflies[i].intensity = fireflyPrefab.intensity * DetermineLightIntensityFactor(particles[i]);
                fireflies[i].transform.localPosition = particles[i].position;
                fireflies[i].enabled = true;
            } else {
                fireflies[i].enabled = false;
            }
        }
    }

    private float DetermineLightIntensityFactor(ParticleSystem.Particle particle) {
        float remainingLifetime = particle.remainingLifetime;
        float startLifetime = particle.startLifetime;

        if (remainingLifetime < fadeoutTime) {
            return remainingLifetime / fadeoutTime;
        }
        // } else if (startLifetime - remainingLifetime < fadeoutTime) {
        //     return 1 - (startLifetime - remainingLifetime) / fadeoutTime;
        // }
        return 1;
    }
}
