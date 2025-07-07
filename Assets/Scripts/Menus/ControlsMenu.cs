using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour {

    // Assign in Inspector
    public Button selectedButton;
    public Image xboxControlsImage;
    public Image playStationControlsImage;
    public TextMeshProUGUI keyboardControlsTextLeft;
    public TextMeshProUGUI keyboardControlsTextRight;
    public int textScale = 200;
    public int iconScale = 200;
    public int iconVOffset = 0;
    //

    private enum Layout { Xbox, PlayStation, Keyboard };
    private Layout currentLayout = Layout.Xbox;

    public void Start() {
        // Debug.Log("ControlsMenu: Start");
        ShowLayout(currentLayout);
        selectedButton.Select();
    }

    public void OnEnable() {
        // Debug.Log("ControlsMenu: Enable");
        Start();
    }

    private void ShowLayout(Layout layout) {
        if (layout == Layout.PlayStation) ShowPlayStationControls();
        else if (layout == Layout.Keyboard) ShowKeyboardControls();
        else ShowXboxControls();
    }

    public void SwitchLayouts() {
        // Debug.Log("Controls Menu: SwitchLayouts");
        AudioManager.instance.PlaySoundEffect(MenuManager.instance.menuConfirmSound, 0.2f);

        if (currentLayout == Layout.Xbox) {
            currentLayout = Layout.PlayStation;
            ShowPlayStationControls();
        } else if (currentLayout == Layout.PlayStation) {
            currentLayout = Layout.Keyboard;
            ShowKeyboardControls();
        } else {
            currentLayout = Layout.Xbox;
            ShowXboxControls();
        }
    }

    private void ClearControls() {
        // Debug.Log("Controls Menu: ClearControls");
        xboxControlsImage.gameObject.SetActive(false);
        playStationControlsImage.gameObject.SetActive(false);
        keyboardControlsTextLeft.text = "";
        keyboardControlsTextRight.text = "";
    }

    private void ShowXboxControls() {
        // Debug.Log("Controls Menu: ShowXboxControls");
        ClearControls();
        xboxControlsImage.gameObject.SetActive(true);
    }

    private void ShowPlayStationControls() {
        // Debug.Log("Controls Menu: ShowPlayStationControls");
        ClearControls();
        playStationControlsImage.gameObject.SetActive(true);
    }

    private void ShowKeyboardControls() {
        // Debug.Log("Controls Menu: ShowKeyboardControls");
        ClearControls();
        // keyboardControlsText.text = $"<size={iconScale}%><sprite=\"Keyboard Full\", index={5}></size>";
        // Keyboard Layout
        // Move - WASD/Arrow keys
        // Interact - E
        // Jump - Space
        // Dash - Shift / F
        // Use Ability - Q / X
        // Climb - Ctrl (Hold)
        // Pause - Esc
        // Respawn - R (Hold)
        // Confirm - Enter / F
        // Cancel - Esc

        // valid indices are 0-94
        Dictionary<string, int[]> keyboardLayoutLeft = new() {
            {"Move", new int[] {90, 13, 79, 29}},
            {"Move (Alt)", new int[] {16, 17, 18, 15}},
            {"Jump", new int[] {84}},
            {"Dash", new int[] {82, 37}},
            {"Interact", new int[] {31}},
        };

        Dictionary<string, int[]> keyboardLayoutRight = new() {
            {"Use Ability", new int[] {75, 92}},
            {"Climb (Hold)", new int[] {28}},
            {"Respawn (Hold)", new int[] {78}},
            {"Pause / Cancel", new int[] {36}},
            {"Confirm", new int[] {34, 37}},
        };

        string textLeft = GetKeyboardSpriteString(keyboardLayoutLeft);
        keyboardControlsTextLeft.text = textLeft;
        string textRight = GetKeyboardSpriteString(keyboardLayoutRight);
        keyboardControlsTextRight.text = textRight;
    }

    private string GetKeyboardSpriteString(Dictionary<string, int[]> layout) {
        string text = "";
        foreach (var kvp in layout) {
            text += $"<size={textScale}%>{kvp.Key}:</size>\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0"; // space
            foreach (int actionIndex in kvp.Value) {
                text += $"<voffset={iconVOffset}><size={iconScale}%><sprite=\"Keyboard Full\", index={actionIndex}></size></voffset> ";
            }
            text += "\n";
        }
        return text;
    }

    public void Back() {
        // Debug.Log("ControlsMenu: Back");
        MenuManager.instance.PreviousMenu();
    }
}
