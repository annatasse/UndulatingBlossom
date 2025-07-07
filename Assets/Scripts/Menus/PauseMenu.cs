using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    // Assign in Inspector
    public Button selectedButton;
    public RectTransform mushroomStatsContainer;
    public TextMeshProUGUI mushroomCount;
    public TextMeshProUGUI deathCountFelix;
    public TextMeshProUGUI deathCountAnna;
    public Color goldenDeathCount;
    //

    public void Start() {
        ActivateMenu();
    }

    public void OnEnable() {
        ActivateMenu();
    }

    private void ActivateMenu() {
        selectedButton.Select();
        mushroomCount.color = MushroomCountColor();
        mushroomCount.text = $"{LevelManager.instance.mushroomsCollected}/{LevelManager.instance.totalMushroomCount}";
        deathCountFelix.color = DeathCountColor(CharacterName.Felix);
        deathCountFelix.text = LevelManager.instance.deathCounts[CharacterName.Felix].ToString();
        deathCountAnna.color = DeathCountColor(CharacterName.Anna);
        deathCountAnna.text = LevelManager.instance.deathCounts[CharacterName.Anna].ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(mushroomStatsContainer);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mushroomStatsContainer);
    }

    private Color DeathCountColor(CharacterName character) {
        return LevelManager.instance.deathCounts[character] == 0 ? goldenDeathCount : Color.white;
    }

    private Color MushroomCountColor() {
        return LevelManager.instance.mushroomsCollected >= LevelManager.instance.totalMushroomCount ? goldenDeathCount : Color.white;
    }

    public void Resume() {
        MenuManager.instance.CloseMenu();
    }

    public void OpenCharacterSelection() {
        MenuManager.instance.NextMenu(MenuManager.Menu.CharacterSelection);
    }

    public void OpenSettings() {
        MenuManager.instance.NextMenu(MenuManager.Menu.Settings);
    }

    public void GoToMainMenu() {
        // MenuManager.instance.CloseMenu();
        MenuManager.instance.NextMenu(MenuManager.Menu.Main, false);
        SceneManager.LoadScene("MainMenu");
    }
}
