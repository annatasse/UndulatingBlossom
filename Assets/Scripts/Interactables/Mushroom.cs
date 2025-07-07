using System;
using System.Linq;
using UnityEngine;

public class Mushroom : MonoBehaviour {

    // Assign in Inspector
    public bool allowFelix = true;
    public bool allowAnna = true;

    [Header("Movement")]
    public float amp;
    public float freq;

    [Header("Following")]
    public float followSpeed;
    public float minFollowDistance;
    public float maxFollowDistance;
    public Vector3 followOffset = new Vector3(0, 1f, 0);

    [Header("Audio")]
    public AudioClip pickupSound;
    //

    public bool isCollected {get; private set;} = false;
    private Vector3 initialPosition;
    private bool isFollowing = false;
    private Transform playerTransform;
    private PlayerStateHandler handler;
    private CharacterName character;
    private int initialDeathCount;

    private void Start() {
        initialPosition = transform.position;
    }

    private void Update() {
        if (!isFollowing) {
            Vector3 movement = new Vector3(0, MathF.Sin(Time.time * freq) * amp, 0);
            transform.position = initialPosition + movement;
        } else {
            Vector3 targetPos = playerTransform.position + followOffset;
            Vector3 direction = targetPos - transform.position;
            float distance = direction.magnitude;

            // faster when further away
            if (distance > maxFollowDistance) {
                direction = direction.normalized * maxFollowDistance;
                targetPos = transform.position + direction;
            }

            // no movement when too close
            if (distance > minFollowDistance) {
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            }

            // prevent collection when dying
            if (initialDeathCount < LevelManager.instance.deathCounts[character]) {
                isFollowing = false;
                return;
            }
            // prevent abuse with spawning platforms
            if (!isCollected && handler.isGrounded &&
                !handler.groundColliders.Any(c => c.CompareTag("FloatingPlatform"))) {
                Collect();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!(
            allowFelix && other.CompareTag("Felix") ||
            allowAnna && other.CompareTag("Anna")
        )) return;

        if (!isFollowing) {
            isFollowing = true;
            playerTransform = other.transform;
            handler = other.GetComponent<PlayerStateHandler>();
            character = other.CompareTag("Felix") ? CharacterName.Felix : CharacterName.Anna;
            initialDeathCount = LevelManager.instance.deathCounts[character];
        }
    }

    private void Collect() {
        isCollected = true;
        AudioManager.instance.PlaySoundEffect(pickupSound, 0.2f);
        LevelManager.instance.CollectMushroom();
        gameObject.SetActive(false);
    }
}
