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
    public Color normalColor;
    public Color selectedColor;
    //

    private bool _infiniteDashFelix;
    private bool _invincibilityFelix;
    private bool _infiniteDashAnna;
    private bool _invincibilityAnna;

    public void Start() {
        selectedButton.Select();
        _infiniteDashFelix = PlayerManager.instance.CanInfiniteDash(CharacterName.Felix);
        _invincibilityFelix = PlayerManager.instance.IsInvincible(CharacterName.Felix);
        _infiniteDashAnna = PlayerManager.instance.CanInfiniteDash(CharacterName.Anna);
        _invincibilityAnna = PlayerManager.instance.IsInvincible(CharacterName.Anna);

        infiniteDashFelix.text = _infiniteDashFelix ? "On" : "Off";
        invincibiliyFelix.text = _invincibilityFelix ? "On" : "Off";
        infiniteDashAnna.text = _infiniteDashAnna ? "On" : "Off";
        invincibiliyAnna.text = _invincibilityAnna ? "On" : "Off";
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
        _invincibilityFelix = !_invincibilityFelix;
        invincibiliyFelix.text = _invincibilityFelix ? "On" : "Off";
        PlayerManager.instance.SetInvincible(CharacterName.Felix, _invincibilityFelix);
    }

    public void ToggleInvincibilityAnna() {
        _invincibilityAnna = !_invincibilityAnna;
        invincibiliyAnna.text = _invincibilityAnna ? "On" : "Off";
        PlayerManager.instance.SetInvincible(CharacterName.Anna, _invincibilityAnna);
    }

        public void ToggleInfiniteDashFelix() {
        _infiniteDashFelix = !_infiniteDashFelix;
        infiniteDashFelix.text = _infiniteDashFelix ? "On" : "Off";
        PlayerManager.instance.SetInfiniteDash(CharacterName.Felix, _infiniteDashFelix);
    }

    public void ToggleInfiniteDashAnna() {
        _infiniteDashAnna = !_infiniteDashAnna;
        infiniteDashAnna.text = _infiniteDashAnna ? "On" : "Off";
        PlayerManager.instance.SetInfiniteDash(CharacterName.Anna, _infiniteDashAnna);
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
