using UnityEngine;
using UnityEngine.InputSystem;

public class SinglePlayerSceneInit : MonoBehaviour {

    // Assign in Inspector
    public GameObject menuManagerPrefab;
    public GameObject playerManagerPrefab;

    public PlayerInput player;
    //

    public void Start() {
        Debug.Log("Sample Scene Initializer: Start");

        if (PlayerManager.instance == null) {
            Instantiate(playerManagerPrefab);
        }
        if (MenuManager.instance == null) {
            Instantiate(menuManagerPrefab);
        }

        // Cheese the player into the system xD
        player.enabled = true;
        player.SwitchCurrentActionMap("Player");
    }
}
