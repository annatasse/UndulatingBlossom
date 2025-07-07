using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room20Init : MonoBehaviour {

    // Assign in Inspector
    public Collider2D room;
    public int speedupDistanceX;
    public float movementSpeedX;
    public float movementSpeedY;
    public float targetPositionFelix;
    public float targetPositionAnna;
    public float groundHeight;
    public GameObject blockFirstPlayer;
    // public Collider2D[] collidersToIgnore;
    //

    private bool scriptActive = true;
    private bool playersFalling = false;
    private bool felixFalling = false;
    private bool annaFalling = false;
    private Dictionary<CharacterName, Collider2D> players = new();

    private void Start() {
        targetPositionFelix = room.transform.position.x + targetPositionFelix;
        targetPositionAnna = room.transform.position.x + targetPositionAnna;
        groundHeight = room.transform.position.y + groundHeight;
    }

    private void FixedUpdate() {
        if (felixFalling && annaFalling && !playersFalling) {
            playersFalling = true;
            blockFirstPlayer.SetActive(false);
            Physics2D.IgnoreLayerCollision(6, 6, true);
        }
        if (playersFalling) {
            MoveToTarget(CharacterName.Felix, targetPositionFelix);
            MoveToTarget(CharacterName.Anna, targetPositionAnna);

            if (felixFalling) {
                felixFalling = players[CharacterName.Felix].attachedRigidbody.position.y > groundHeight;
            }
            if (annaFalling) {
                annaFalling = players[CharacterName.Anna].attachedRigidbody.position.y > groundHeight;
            }
            if (!felixFalling && !annaFalling) {
                Physics2D.IgnoreLayerCollision(6, 6, false);
                LevelManager.instance.LockAbility(Shrine.Ability.SpawningPlatforms);
                players[CharacterName.Felix].GetComponent<PlayerInput>().ActivateInput();
                players[CharacterName.Anna].GetComponent<PlayerInput>().ActivateInput();
                playersFalling = false;
                scriptActive = false;
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!scriptActive) return;

        if (other.CompareTag("Felix")) {
            players.Add(CharacterName.Felix, other);
            felixFalling = true;
        } else if (other.CompareTag("Anna")) {
            players.Add(CharacterName.Anna, other);
            annaFalling = true;
        } else {
            return;
        }
        other.GetComponent<PlayerInput>().DeactivateInput();
        other.GetComponent<PlayerStateHandler>().ChangePlayerState(PlayerStateType.Falling);
        // IgnoreCollisions(other, true);
    }

    private bool MoveToTarget(CharacterName character, float targetX) {
        Vector2 position = players[character].attachedRigidbody.position;
        float speedFactor = Mathf.Abs(position.x - targetX) > speedupDistanceX ? 2 : 1;
        Vector2 newPosition = Vector2.MoveTowards(position, new Vector2(targetX, position.y), movementSpeedX * speedFactor * Time.fixedDeltaTime);
        newPosition.y -= movementSpeedY * Time.fixedDeltaTime;
        players[character].attachedRigidbody.MovePosition(newPosition);
        return players[character].attachedRigidbody.position.x == targetX;
    }

    // private bool TryLanding(Collider2D player) {
    //     if (player.attachedRigidbody.position.y <= groundHeight) {
    //         IgnoreCollisions(player.GetComponent<Collider2D>(), false);
    //         player.GetComponent<PlayerInput>().ActivateInput();
    //         return true;
    //     }
    //     return false;
    // }

    // private void IgnoreCollisions(Collider2D player, bool ignore) {
    //     foreach (Collider2D collider in collidersToIgnore) {
    //         Physics2D.IgnoreCollision(player, collider, ignore);
    //     }
    // }
}
