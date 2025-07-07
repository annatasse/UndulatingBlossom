using UnityEngine;

public class MainMenuInit : MonoBehaviour {

    // Assign in Inspector
    public GameObject audioManagerPrefab;
    public GameObject menuManagerPrefab;
    public GameObject playerManagerPrefab;
    //

    public void Start() {
        // Debug.Log("Main Menu Initializer: Start");

        // Ensure that all manager singletons are available
        if (AudioManager.instance == null) {
            Instantiate(audioManagerPrefab);
        }
        if (PlayerManager.instance == null) {
            Instantiate(playerManagerPrefab);
        }
        if (MenuManager.instance == null) {
            Instantiate(menuManagerPrefab);
        }

        MenuManager.instance.OpenMenu(MenuManager.Menu.Main);
    }
}
