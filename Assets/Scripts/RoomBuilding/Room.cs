using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour, IResettable {

    // Assign in Inspector
    public GameObject cam;
    public BoxCollider2D room;
    public GameObject background;
    public bool isCave = false;
    public bool isOversize = false;
    public GameObject blockRight;
    public GameObject blockLeft;
    public GameObject blockUp;
    public GameObject blockDown;
    public SpriteRenderer exitArrow;
    public List<Component> shouldReset;
    public RespawnArea defaultRespawnArea;
    //

    public Dictionary<CharacterName, Vector3> spawnPoints {get; private set;} = new();

    private enum Direction {Right, Left, Up, Down, Vanished};
    private List<PlayerInput> playersInside = new();
    private PlayerInput playerOutside;
    private Direction exitDirection;
    private Dictionary<Direction, GameObject> blocks;
    private List<IResettable> resettables = new();
    private bool isActive = false;
    private float roomGap = 0.125f;
    private Bounds roomBoundsWithGap;

    private void Start() {
        roomBoundsWithGap = new Bounds(room.bounds.center, room.bounds.size + new Vector3(0, roomGap, 0));
        blocks = new Dictionary<Direction, GameObject>();
        if (blockRight != null) {blocks.Add(Direction.Right, blockRight);}
        if (blockLeft != null) {blocks.Add(Direction.Left, blockLeft);}
        if (blockUp != null) {blocks.Add(Direction.Up, blockUp);}
        if (blockDown != null) {blocks.Add(Direction.Down, blockDown);}
        DeactivateBlocks();

        shouldReset.ForEach(candidate => {
            IResettable resettable = candidate.GetComponent<IResettable>();
            if (resettable != null) {
                resettables.Add(resettable);
            } else {
                Debug.LogWarning("Object in shouldReset have to implement IResettable: " + candidate.name);
            }
        });

        spawnPoints[CharacterName.Felix] = defaultRespawnArea.GetSpawnPoint(CharacterName.Felix);
        spawnPoints[CharacterName.Anna] = defaultRespawnArea.GetSpawnPoint(CharacterName.Anna);

        #if !UNITY_EDITOR
        if (room.gameObject.name == "Room1") {
            // Debug.Log("Room1 is active");
            cam.SetActive(true);
        }
        #endif
        if (isOversize) {
            StartCoroutine(InitializeOversizedRoom());
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;
        if (!IsFullyInside(other)) return;

        PlayerInput playerInput = other.gameObject.GetComponent<PlayerInput>();

        if (playersInside.Contains(playerInput)) return;

        // Debug.Log("Player entered room");
        playersInside.Add(playerInput);
        if (isActive && playerInput == playerOutside) {
            playerInput.GetComponent<PlayerStateHandler>().OnRoomEnter();
            playerOutside = null;
            RemoveExitArrow();
            DeactivateBlocks();
        }
        if (!isActive && playersInside.Count == 2) {
            ActivateRoom();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Felix") && !other.CompareTag("Anna")) return;

        PlayerInput playerInput = other.gameObject.GetComponent<PlayerInput>();

        if (!playersInside.Remove(playerInput)) {
            // Debug.Log("Player left room, but was never fully inside");
            return;
        }
        if (!isActive) return;
        exitDirection = ExitDirection(other);
        if (exitDirection == Direction.Vanished) {
            Debug.LogWarning("Player Died");
            playersInside.Add(playerInput);
            return;
        }
        if (playerOutside == null) {
            // Debug.Log("First player left room");
            playerInput.GetComponent<PlayerStateHandler>().OnRoomExit(DirectionToVector2(exitDirection));
            playerOutside = playerInput;
            ActivateBlocks(exitDirection);
            SetExitArrow(playerInput, exitDirection);
        } else {
            // Debug.Log("Second player left room");
            DeactivateRoom();
        }
    }

    private void ActivateRoom() {
        // Debug.Log("Activate Camera");
        isActive = true;
        cam.SetActive(true);
        DeactivateBlocks();
        LevelManager.instance.SetCurrentRoom(this);
        LevelManager.instance.globalLight.intensity = isCave ? 0.3f : 1f;
    }

    private IEnumerator InitializeOversizedRoom() {
        // This is total bullshit, but it works somehow...
        cam.SetActive(true);
        yield return new WaitForSeconds(2);
        cam.SetActive(false);
    }

    private void DeactivateRoom() {
        isActive = false;
        DeactivateBlocks();
        playerOutside.GetComponent<PlayerStateHandler>().OnRoomEnter();
        playerOutside = null;
        playersInside.Clear();
        // Debug.Log("Deactivate Camera");
        cam.SetActive(false);
        RemoveExitArrow();
    }

    private bool IsFullyInside(Collider2D other) {
        return roomBoundsWithGap.Contains(other.bounds.min) && roomBoundsWithGap.Contains(other.bounds.max);
    }

    private Direction ExitDirection(Collider2D other) {
        Vector3 direction = other.transform.position - transform.position;
        if (direction.x >= room.size.x / 2) {
            // Debug.Log("right");
            return Direction.Right;
        }
        if (direction.x <= -room.size.x / 2) {
            // Debug.Log("left");
            return Direction.Left;
        }
        if (direction.y >= room.size.y / 2) {
            // Debug.Log("up");
            return Direction.Up;
        }
        if (direction.y <= -room.size.y / 2) {
            // Debug.Log("down");
            return Direction.Down;
        }
        return Direction.Vanished;
    }

    private Vector2 DirectionToVector2(Direction direction) {
        return direction switch {
            Direction.Right => Vector2.right,
            Direction.Left => Vector2.left,
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            _ => Vector2.zero
        };
    }

    private void ActivateBlocks(Direction exclude) {
        foreach (Direction key in blocks.Keys) {
            if (key != exclude) {
                blocks[key].SetActive(true);
            }
        }
    }

    private void DeactivateBlocks() {
        foreach (Direction key in blocks.Keys) {
            blocks[key].SetActive(false);
        }
    }

    private void SetExitArrow(PlayerInput leavingPlayer, Direction exitDirection) {
        exitArrow.color = leavingPlayer.CompareTag("Felix") ? Color.blue : Color.red;
        Vector3 snappedPlayerPosition = new Vector3(
            MathF.Round(leavingPlayer.transform.position.x),
            MathF.Round(leavingPlayer.transform.position.y),
            0
        );
        Vector3 offset = Vector3.zero;
        int rotation = 0;
        switch (exitDirection) {
            case Direction.Right:
                offset = Vector3.left;
                rotation = -90;
                break;
            case Direction.Left:
                offset = Vector3.right;
                rotation = 90;
                break;
            case Direction.Up:
                offset = Vector3.down;
                rotation = 0;
                break;
            case Direction.Down:
                offset = Vector3.up;
                rotation = 180;
                break;
        }
        exitArrow.transform.position = room.bounds.ClosestPoint(snappedPlayerPosition) + offset;
        exitArrow.transform.eulerAngles = new Vector3(0, 0, rotation);
        exitArrow.enabled = true;
    }

    private void RemoveExitArrow() {
        exitArrow.enabled = false;
    }

    public void ResetWithDeath() {
        resettables.ForEach(resettable => resettable.ResetWithDeath());
    }

    public void ResetWithRoom() {
        resettables.ForEach(resettable => resettable.ResetWithRoom());
    }

    public void SetSpawnPoint(CharacterName character, Vector3 position) {
        spawnPoints[character] = position;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, room.size);
    }
}
