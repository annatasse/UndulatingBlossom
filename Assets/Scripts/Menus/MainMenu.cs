using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    // Assign in Inspector
    public Button selectedButton;
    public GameObject[] backgrounds;
    //

    private System.Random random = new System.Random();
    private int currentBackground;

    public void Start() {
        // Debug.Log("Main Menu: Start");
        ActivateMenu();
    }

    public void OnEnable() {
        // Debug.Log("Main Menu: Enable");
        ActivateMenu();
    }

    private void ActivateMenu() {
        selectedButton.Select();
        backgrounds[currentBackground].SetActive(false);
        currentBackground = random.Next(backgrounds.Length);
        backgrounds[currentBackground].SetActive(true);
    }

    public void Play() {
        // MenuManager.instance.CloseMenu();
        MenuManager.instance.NextMenu(MenuManager.Menu.CharacterSelection); // always go into character selection screen
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings() {
        MenuManager.instance.NextMenu(MenuManager.Menu.Settings);
    }

    public void OpenCredits() {
        MenuManager.instance.NextMenu(MenuManager.Menu.Credits);
    }

    public void Quit() {
        // Debug.Log("Quit Game");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
