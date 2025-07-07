using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    // Assign in Inspector
    public Button selectedButton;
    //

    public void Start() {
        Debug.Log("Settings Menu: Start");
        selectedButton.Select();
    }

    public void OnEnable() {
        Debug.Log("Settings Menu: Enable");
        selectedButton.Select();
    }

    public void OpenAudioSettings() {
        MenuManager.instance.NextMenu(MenuManager.Menu.Audio);
    }

    public void OpenAssistModeMenu() {
        MenuManager.instance.NextMenu(MenuManager.Menu.AssistMode);
    }

    public void ShowControls() {
        MenuManager.instance.NextMenu(MenuManager.Menu.Controls);
    }

    public void Back() {
        MenuManager.instance.PreviousMenu();
    }
}
