using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : DeadlySurface {

    // Assign in Inspector
    public ParticleSystem waterSplash;
    public AudioClip normalWaterSound;
    public AudioClip caveWaterSound;
    public Vector3 offset;
    public float waterSpeed;
    public LayerMask theplayerwillstartfloatingupifnotilefromtheselayersisdirectlyabovetheplayer;
    //

    private List<Collider2D> invinciblePlayersInside;

    private void Start() {
        waterSplash = Instantiate(waterSplash);
        invinciblePlayersInside = new();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;
        // if (other.CompareTag("Felix") && PlayerManager.instance.IsInvincible(CharacterName.Felix)) return;
        // if (other.CompareTag("Anna") && PlayerManager.instance.IsInvincible(CharacterName.Anna)) return;
        if (deadPlayers.Contains(other)) return;
        if (invinciblePlayersInside.Contains(other)) return;

        if (
            (other.CompareTag("Felix") && PlayerManager.instance.IsInvincible(CharacterName.Felix)) ||
            (other.CompareTag("Anna") && PlayerManager.instance.IsInvincible(CharacterName.Anna))
        ) {
            // Debug.Log("invincible enter");
            StartCoroutine(OnPlayerCollisionInvincible(other));
        } else {
            StartCoroutine(OnPlayerCollision(other, Time.time + respawnDelay));
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!invinciblePlayersInside.Contains(other)) return;
        // Debug.Log("invincible exit");
        invinciblePlayersInside.Remove(other);
    }

    override protected IEnumerator OnPlayerCollision(Collider2D player, float respawnTime) {
        PlaySoundEffect();
        AnimateSplash(player);
        KillPlayer(player);

        player.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.attachedRigidbody.gravityScale = -1;

        while (Time.time < respawnTime) {
            if (player.attachedRigidbody.linearVelocityY >= waterSpeed) {
                player.attachedRigidbody.linearVelocityY = waterSpeed;
                player.attachedRigidbody.gravityScale = 0;
            }
            yield return null;
        }
        player.attachedRigidbody.gravityScale = player.GetComponent<PlayerStateHandler>().baseGravity;
        // DeathAnimation(player);
        RespawnPlayer(player);
    }

    private IEnumerator OnPlayerCollisionInvincible(Collider2D player) {
        invinciblePlayersInside.Add(player);
        PlaySoundEffect();
        AnimateSplash(player);

        player.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.attachedRigidbody.gravityScale = 1;

        Debug.Log(player.attachedRigidbody.linearVelocityY);

        while (invinciblePlayersInside.Contains(player)) {
            Vector2 v = player.attachedRigidbody.linearVelocity;
            v.x = Mathf.Clamp(v.x, -Mathf.Abs(waterSpeed), +Mathf.Abs(waterSpeed));
            v.y = Mathf.Clamp(v.y, -Mathf.Abs(waterSpeed), +15); // max dash speed is ~25
            player.attachedRigidbody.linearVelocity = v;

            // make player float if near surface
            Vector2 checkPos = player.attachedRigidbody.position;
            checkPos.x += 0.5f;
            if (!Physics2D.OverlapPoint(checkPos, theplayerwillstartfloatingupifnotilefromtheselayersisdirectlyabovetheplayer)) {
                player.attachedRigidbody.gravityScale = -1;
            }
            yield return null;
        }
        player.attachedRigidbody.gravityScale = player.GetComponent<PlayerStateHandler>().baseGravity;
    }

    private void AnimateSplash(Collider2D player) {
        waterSplash.transform.position = player.transform.position - offset;
        waterSplash.Play();
    }

    private void PlaySoundEffect() {
        AudioManager.instance.PlaySoundEffect(
            LevelManager.instance.currentRoom.isCave ? caveWaterSound : normalWaterSound
        );
    }
}
