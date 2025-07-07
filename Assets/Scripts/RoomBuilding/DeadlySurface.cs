using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlySurface : MonoBehaviour {

    // Assign in Inspector
    public float respawnDelay;
    //

    protected List<Collider2D> deadPlayers = new();

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;
    //     if (other.CompareTag("Felix") && PlayerManager.instance.IsInvincible(CharacterName.Felix)) return;
    //     if (other.CompareTag("Anna") && PlayerManager.instance.IsInvincible(CharacterName.Anna)) return;
    //     if (deadPlayers.Contains(other)) return;
    //     StartCoroutine(OnPlayerCollision(other, Time.time + respawnDelay));
    // }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.collider.CompareTag("Felix") && !collision.collider.CompareTag("Anna")) return;
        if (collision.collider.CompareTag("Felix") && PlayerManager.instance.IsInvincible(CharacterName.Felix)) return;
        if (collision.collider.CompareTag("Anna") && PlayerManager.instance.IsInvincible(CharacterName.Anna)) return;
        if (deadPlayers.Contains(collision.collider)) return;
        StartCoroutine(OnPlayerCollision(collision.collider, Time.time + respawnDelay));
    }

    protected virtual IEnumerator OnPlayerCollision(Collider2D player, float respawnTime) {
        DeathAnimation(player);
        KillPlayer(player);
        yield return new WaitForSeconds(respawnDelay);
        RespawnPlayer(player);
    }

    protected void DeathAnimation(Collider2D player) {
        // Debug.Log("Death");
        LevelManager.instance.PlayDeathAnimation(player.gameObject);
    }

    protected void KillPlayer(Collider2D player) {
        deadPlayers.Add(player);
        LevelManager.instance.KillPlayer(player.gameObject);
    }

    protected void RespawnPlayer(Collider2D player) {
        deadPlayers.Remove(player);
        LevelManager.instance.RespawnPlayer(player.gameObject);
    }
}
