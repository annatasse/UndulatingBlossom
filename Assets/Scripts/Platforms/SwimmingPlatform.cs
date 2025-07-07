using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingPlatform : MonoBehaviour {
    // Assign in Inspector
    public float sinkingDistance;
    public float sinkingDuration;
    public float risingDuration;
    public float amp;
    public float freq;
    public AudioClip landingSound;
    //

    private List<GameObject> parentedObjects;
    private Vector3 initialPosition;
    private Vector3 sinkingOffset;
    private Coroutine activeSinkingCoroutine;
    private Vector3 anchorPosition;


    private void Start() {
        initialPosition = transform.position;
        anchorPosition = initialPosition;
        sinkingOffset = new Vector3(0, sinkingDistance, 0);
        parentedObjects = new();
    }

    private void Update() {
        if (activeSinkingCoroutine == null) { // avoid snapping up/down when starting rising/sinking
            Vector3 movement = new Vector3(0, MathF.Sin(Time.time * freq) * amp, 0);
            transform.position = anchorPosition + movement;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Felix") && !collision.gameObject.CompareTag("Anna")) return;

        if (!parentedObjects.Contains(collision.gameObject)) {
            AudioManager.instance.PlaySoundEffect(landingSound);
            ParentIncomingObject(collision);
            StartSinking(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Felix") && !collision.gameObject.CompareTag("Anna")) return;

        if (parentedObjects.Contains(collision.gameObject)) {
            ResetParent(collision);
        }
        if (parentedObjects.Count == 0) {
            StartSinking(false);
        }
    }

    private void ParentIncomingObject(Collision2D collision) {
        collision.transform.SetParent(gameObject.transform);
        parentedObjects.Add(collision.gameObject);
    }

    private void ResetParent(Collision2D collision) {
        collision.transform.SetParent(PlayerManager.instance.transform);
        parentedObjects.Remove(collision.gameObject);
    }

    private void StartSinking(bool sink) {
        if (activeSinkingCoroutine != null) {
            StopCoroutine(activeSinkingCoroutine);
        }
        activeSinkingCoroutine = StartCoroutine(SinkRoutine(sink));
    }

    private IEnumerator SinkRoutine(bool sink) {
        Vector3 start = transform.position;
        anchorPosition = sink ? initialPosition - sinkingOffset : initialPosition;

        float duration = sink ? sinkingDuration : risingDuration;
        float elapsed = 0f;

        while (elapsed < duration) {
            transform.position = Vector3.Lerp(start, anchorPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // transform.position = end;
        activeSinkingCoroutine = null;
    }
}
