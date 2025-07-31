using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssistModeMenu : MonoBehaviour {

    // Assign in Inspector
    public Button selectedButton;
    public TextMeshProUGUI infiniteDashFelix;
    public TextMeshProUGUI invincibiliyFelix;
    public TextMeshProUGUI infiniteDashAnna;
    public TextMeshProUGUI invincibiliyAnna;
    public TextMeshProUGUI gameSpeedText;
    public Color normalColor;
    public Color selectedColor;
    //

    private bool isInfiniteDashFelix;
    private bool isInvincibilityFelix;
    private bool isInfiniteDashAnna;
    private bool isInvincibilityAnna;
    private float _gameSpeed;

    public void Start() {
        selectedButton.Select();
        isInfiniteDashFelix = PlayerManager.instance.CanInfiniteDash(CharacterName.Felix);
        isInvincibilityFelix = PlayerManager.instance.IsInvincible(CharacterName.Felix);
        isInfiniteDashAnna = PlayerManager.instance.CanInfiniteDash(CharacterName.Anna);
        isInvincibilityAnna = PlayerManager.instance.IsInvincible(CharacterName.Anna);

        infiniteDashFelix.text = isInfiniteDashFelix ? "On" : "Off";
        invincibiliyFelix.text = isInvincibilityFelix ? "On" : "Off";
        infiniteDashAnna.text = isInfiniteDashAnna ? "On" : "Off";
        invincibiliyAnna.text = isInvincibilityAnna ? "On" : "Off";

        _gameSpeed = MenuManager.instance.gameSpeed;
    }

    public void OnEnable() {
        selectedButton.Select();
        selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    public void OnDisable() {
        // Avoid buttons staying "selected" after closing the menu
        infiniteDashFelix.color = normalColor;
        invincibiliyFelix.color = normalColor;
        infiniteDashAnna.color = normalColor;
        invincibiliyAnna.color = normalColor;
    }

    public void ToggleInvincibilityFelix() {
        isInvincibilityFelix = !isInvincibilityFelix;
        invincibiliyFelix.text = isInvincibilityFelix ? "On" : "Off";
        PlayerManager.instance.SetInvincible(CharacterName.Felix, isInvincibilityFelix);
    }

    public void ToggleInvincibilityAnna() {
        isInvincibilityAnna = !isInvincibilityAnna;
        invincibiliyAnna.text = isInvincibilityAnna ? "On" : "Off";
        PlayerManager.instance.SetInvincible(CharacterName.Anna, isInvincibilityAnna);
    }

        public void ToggleInfiniteDashFelix() {
        isInfiniteDashFelix = !isInfiniteDashFelix;
        infiniteDashFelix.text = isInfiniteDashFelix ? "On" : "Off";
        PlayerManager.instance.SetInfiniteDash(CharacterName.Felix, isInfiniteDashFelix);
    }

    public void ToggleInfiniteDashAnna() {
        isInfiniteDashAnna = !isInfiniteDashAnna;
        infiniteDashAnna.text = isInfiniteDashAnna ? "On" : "Off";
        PlayerManager.instance.SetInfiniteDash(CharacterName.Anna, isInfiniteDashAnna);
    }

    public void UpdateGameSpeed() {
        _gameSpeed += 0.1f;
        if (_gameSpeed > 1.51f) { // 1.50 gets skipped, maybe due to float precision
            _gameSpeed = 0.5f;
        }
        MenuManager.instance.gameSpeed = _gameSpeed;
        gameSpeedText.text = $"Game Speed: {_gameSpeed * 100:F0}%";
    }

    public void Back() {
        MenuManager.instance.PreviousMenu();
    }

    public void OnButtonSelected(BaseEventData eventData) {
        TextMeshProUGUI text = eventData.selectedObject.GetComponentInChildren<TextMeshProUGUI>();
        text.color = selectedColor;
    }

    public void OnButtonDeselected(BaseEventData eventData) {
        TextMeshProUGUI text = eventData.selectedObject.GetComponentInChildren<TextMeshProUGUI>();
        text.color = normalColor;
    }
}
