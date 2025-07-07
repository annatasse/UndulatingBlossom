using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour {

    // Assign in Inspector
    public GameObject mainCamera;
    public Light2D globalLight;
    public CinemachineTargetGroup targetGroup;

    [Header("Background Music")]
    public AudioClip overworldBGMusic;
    public AudioClip caveBGMusic;

    [Header("Sound Effects")]
    public AudioClip deathSound;
    //

    public static LevelManager instance;

    public Room currentRoom {get; private set;}
    public int mushroomsCollected {get; private set;}
    public int totalMushroomCount {get; private set;}
    public Dictionary<Shrine.Ability, bool> abilityUnlocked {get; private set;}
    public Dictionary<CharacterName, int> deathCounts {get; private set;}

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        mushroomsCollected = 0;
        abilityUnlocked = new() {
            {Shrine.Ability.Dash, false},
            {Shrine.Ability.SpawningPlatforms, false},
            {Shrine.Ability.SwitchingPlatforms, false}
        };
        deathCounts = new() {
            {CharacterName.Felix, 0},
            {CharacterName.Anna, 0}
        };
        // Deduct 3 because golden mushrooms don't count.
        totalMushroomCount = FindObjectsByType<Mushroom>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length - 3;
    }

    public void SetCurrentRoom(Room room) {
        if(currentRoom != null) {
            currentRoom.ResetWithRoom();
            PlayerManager.instance.GetPlayerTransform(CharacterName.Anna).GetComponent<SpawnPlatform>().ResetWithRoom();
            ReturnBackground(currentRoom);
        }
        currentRoom = room;
        AssignBackground(room);
        AudioManager.instance.PlayBackgroundMusic(currentRoom.isCave ? caveBGMusic : overworldBGMusic);
    }

    public void CollectMushroom() {
        mushroomsCollected++;
    }

    public void LockAbility(Shrine.Ability ability) {
        if (abilityUnlocked.ContainsKey(ability)) {
            abilityUnlocked[ability] = false;
        } else {
            Debug.LogWarning($"Attempted to lock an unknown ability: {ability}");
        }
    }

    public void UnlockAbility(Shrine.Ability ability) {
        if (abilityUnlocked.ContainsKey(ability)) {
            abilityUnlocked[ability] = true;
        } else {
            Debug.LogWarning($"Attempted to unlock an unknown ability: {ability}");
        }
    }

    public void PlayDeathAnimation(GameObject player) {
        player.GetComponentInChildren<AnimationController>().PlayDeathAnimation();
        AudioManager.instance.PlaySoundEffect(deathSound, 0.6f);
    }

    public void KillPlayer(GameObject player) {
        if (player.CompareTag("Felix")) {
            deathCounts[CharacterName.Felix]++;
        } else {
            deathCounts[CharacterName.Anna]++;
        }
        player.GetComponent<PlayerInput>().DeactivateInput();
        player.GetComponent<PlayerStateHandler>().Die();
        currentRoom.ResetWithDeath();
    }

    public void RespawnPlayer(GameObject player) {
        CharacterName character = player.CompareTag("Felix") ? CharacterName.Felix : CharacterName.Anna;
        player.transform.position = currentRoom.spawnPoints[character];
        player.GetComponent<PlayerInput>().ActivateInput();
        player.GetComponent<PlayerStateHandler>().Respawn();
    }

    public void AssignTargetGroup() {
        targetGroup.AddMember(PlayerManager.instance.GetPlayerTransform(CharacterName.Felix), 1, 0.5f);
        targetGroup.AddMember(PlayerManager.instance.GetPlayerTransform(CharacterName.Anna), 1, 0.5f);
    }

    public void AssignBackground(Room room) {
        // Debug.Log("assign background: " + room.gameObject.name);
        room.background.transform.SetParent(mainCamera.transform);
        room.background.transform.localPosition = new Vector3(0, 0, 1);
        room.background.GetComponent<Light2D>().enabled = true;
    }

    public void ReturnBackground(Room room) {
        // Debug.Log("return background: " + room.gameObject.name);
        room.background.transform.SetParent(currentRoom.transform, false);
        room.background.GetComponent<Light2D>().enabled = false;
    }
}
