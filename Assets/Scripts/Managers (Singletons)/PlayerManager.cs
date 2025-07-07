/*
--------------- Player Manager ----------------
Handles all player inputs and keeps track of which character each player is playing.

The PlayerManager Object is a singleton and persists across scenes. It can be accessed by other scripts via PlayerManager.instance.
It should only be created in the first scene (CharacterSelectionScene).
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour {

    // Assign in Inspector
    public GameObject felixRenderPrefab;
    public GameObject annaRenderPrefab;
    //

    public static PlayerManager instance;

    private List<Player> players;
    private Dictionary<CharacterName, Vector3> prevPositions = new();

    // Singleton pattern
    private void Awake() {
        // Debug.Log("Player Manager: Awake");

        if (instance != null) {
            Debug.Log("Attempting to create another Player Manager");
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(instance);    // keep this instance across scenes
            players = new List<Player> {
                new Player(CharacterName.Felix),
                new Player(CharacterName.Anna)
            };
        }
    }

    // Unity Events //
    public void OnPlayerJoined(PlayerInput playerInput) {
        Player joinedPlayer = players.Find(player => player.playerInput && player.playerInput.playerIndex == playerInput.playerIndex);
        if (joinedPlayer != null) {
            Debug.Log("Player rejoined " + playerInput.playerIndex);
            return;
        }

        Debug.Log("Player joined " + playerInput.playerIndex);

        // if (players.Count >= 2) {
        if (IsReady()) {
            Debug.LogWarning("Only two players are supported.");
            Destroy(playerInput.gameObject);
            return;
        }

        // Maybe add a check which character is assigned already.
        // But I think this would only be relevant if one player changed their input device mid game.
        // CharacterName assignedCharacter = players.Count == 0 ? CharacterName.Felix : CharacterName.Anna;
        CharacterName assignedCharacter = GetPlayerCount() == 0 ? CharacterName.Felix : CharacterName.Anna;
        playerInput.transform.SetParent(transform);
        playerInput.SwitchCurrentActionMap("UI");
        // players.Add(new Player(playerInput, assignedCharacter));
        players.Find(player => player.character == assignedCharacter).SetPlayerInput(playerInput);
        if (prevPositions.ContainsKey(assignedCharacter)) {
            SetPosition(assignedCharacter, prevPositions[assignedCharacter]);
        }
        playerInput.gameObject.tag = assignedCharacter.ToString();
    }

    // This method doesn't get triggered by disconnecting, but when a PlayerInput GameObject is destroyed.
    // This would be relevant if a player changes their input device mid game. (i.e Playstation to XBox controller)
    public void OnPlayerLeft(PlayerInput playerInput) {
        Debug.LogWarning("Player left: " + playerInput.playerIndex);
        // Player removedPlayer = players.Find(player => player.playerIndex == playerInput.playerIndex);
        // players.Remove(removedPlayer);
    }

    // Regular Methods //
    public void SwapCharacters() {
        if (players.Count < 2) {
            Debug.Log("Not enough players have joined!");
            return;
        }
        (players[1].character, players[0].character) = (players[0].character, players[1].character);

        (players[1].playerInput.gameObject.tag, players[0].playerInput.gameObject.tag)
            = (players[0].playerInput.gameObject.tag, players[1].playerInput.gameObject.tag);

        (players[1].playerInput.transform.position, players[0].playerInput.transform.position)
            = (players[0].playerInput.transform.position, players[1].playerInput.transform.position);
    }

    public void AssignRenderPrefabs() {
        for (int i=0; i < players.Count; i++) {
            // Get data from PlayerManager
            PlayerInput playerInput = players[i].playerInput;
            CharacterName assignedCharacter = players[i].character;

            GameObject characterRenderer = assignedCharacter == CharacterName.Felix ? Instantiate(felixRenderPrefab) : Instantiate(annaRenderPrefab);
            // Add sprite renderer to player object
            characterRenderer.transform.SetParent(playerInput.gameObject.transform, false);
            characterRenderer.transform.localPosition = Vector3.zero;
        }
    }

    public void SetActionMaps(string actionMap) {
        if (actionMap != "UI" && actionMap != "Player") {
            Debug.LogWarning("Requested action map not in ['UI','Player']:" + actionMap);
        }
        players.ForEach(player => {if (player.playerInput != null) player.playerInput.SwitchCurrentActionMap(actionMap);});
    }

    public void SetPosition(CharacterName character, Vector3 position) {
        Player player = players.Find(player => player.character == character);
        player.playerInput.transform.position = position;
    }

    public void ResetPlayers() {
        foreach (Player player in players) {
            if (player.playerInput) {
                prevPositions[player.character] = player.playerInput.transform.position;
                Destroy(player.playerInput.gameObject);
                player.RemovePlayerInput();
            }
        }
        // players.Clear();
    }

    public bool IsReady() {
        // return players.Count() == 2;
        return players.All(player => player.playerInput != null);
    }

    public int GetPlayerCount() {
        // return players.Count();
        return players.Where(player => player.playerInput != null).Count();
    }

    public Transform GetPlayerTransform(CharacterName character) {
        return players.Find(player => player.character == character).playerInput.transform;
    }

    public ControllerType GetControllerType(CharacterName character) {
        return players.Find(player => player.character == character).controllerType;
    }

    public bool CanInfiniteDash(CharacterName character) {
        return players.Find(player => player.character == character).infiniteDash;
    }

    public bool IsInvincible(CharacterName character) {
        return players.Find(player => player.character == character).invincible;
    }

     public bool SetInfiniteDash(CharacterName character, bool value) {
        return players.Find(player => player.character == character).infiniteDash = value;
    }

    public bool SetInvincible(CharacterName character, bool value) {
        return players.Find(player => player.character == character).invincible = value;
    }

    // public void DisablePlayer(int playerIndex) {
    //             exitedPlayer.GetComponent<PlayerInput>().enabled = false;
    //     Player player = players.Find(player => player.playerInput.playerIndex == playerIndex);
    // }
}
