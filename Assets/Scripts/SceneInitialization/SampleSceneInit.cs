using UnityEngine;

public class SampleSceneInit: MonoBehaviour {

    // Assign in Inspector
    public GameObject audioManagerPrefab;
    public GameObject menuManagerPrefab;
    public GameObject playerManagerPrefab;
    public Transform felixSpawnPoint;
    public Transform annaSpawnPoint;
    //

    private bool playersSpawned = false;

    public void Start() {
        // Debug.Log("Sample Scene Initializer: Start");

        // Ensure that all manager singletons are available
        if (AudioManager.instance == null) {
            Instantiate(audioManagerPrefab);
        }
        if (PlayerManager.instance == null) {
            Instantiate(playerManagerPrefab);
        }
        if (MenuManager.instance == null) {
            Instantiate(menuManagerPrefab);
            MenuManager.instance.OpenMenu(MenuManager.Menu.CharacterSelection);
        }
    }

    public void Update() {
        if (!playersSpawned && PlayerManager.instance.IsReady()) {
            PlayerManager.instance.SetPosition(CharacterName.Felix, felixSpawnPoint.transform.position);
            PlayerManager.instance.SetPosition(CharacterName.Anna, annaSpawnPoint.transform.position);
            playersSpawned = true;
        }
    }
}
