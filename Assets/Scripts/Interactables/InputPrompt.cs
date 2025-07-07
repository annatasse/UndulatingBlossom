using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPrompt : MonoBehaviour {

    // Assign is Inspector
    public int[] actionIndices; // index from Assets/TextMesh Pro/Resources/Sprite Assets/
    public bool useShared = true;
    public int[] actionIndicesXbox;
    public int[] actionIndicesPlayStation;
    public int[] actionIndicesKeyboard;
    public string prefix = "Press";
    public string suffix = "";
    public string separator = "+";
    public Vector3 offset;
    public GameObject canvas;
    public int textScale = 200;
    public int iconScale = 200;
    public bool allowFelix = true;
    public bool allowAnna = true;
    //

    private Image background;
    private TextMeshProUGUI displayText;
    private Dictionary<ControllerType, int[]> actions;

    private void Start() {
        background = canvas.GetComponentInChildren<Image>();
        displayText = canvas.GetComponentInChildren<TextMeshProUGUI>();

        if (displayText == null) {
            Debug.LogWarning("Input prompt canvas need to have a TextMeshProUGUI child");
        }

        actions = new Dictionary<ControllerType, int[]> {
            {ControllerType.XBox, actionIndicesXbox},
            {ControllerType.PlayStation, actionIndicesPlayStation},
            {ControllerType.Keyboard, actionIndicesKeyboard},
            {ControllerType.Other, actionIndicesXbox}
        };
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!(
            allowFelix && other.CompareTag("Felix") ||
            allowAnna && other.CompareTag("Anna")
        )) return;

        CharacterName character = other.CompareTag("Felix") ? CharacterName.Felix : CharacterName.Anna;
        ControllerType controllerType = PlayerManager.instance.GetControllerType(character);
        canvas.SetActive(true);
        displayText.text = CreateButtonPrompt(controllerType);
        background.transform.position = transform.position + offset;
        LayoutRebuilder.ForceRebuildLayoutImmediate(background.rectTransform);
        // Debug.Log("collision fired");
    }

    private void OnTriggerExit2D(Collider2D other) {
        canvas.SetActive(false);
        // Debug.Log("collision exit");
    }

    private string CreateButtonPrompt(ControllerType controllerType) {
        int[] indices = useShared ? actionIndices : actions[controllerType];
        string[] prompts = indices.Select(actionIndex => GetSpriteString(controllerType, actionIndex)).ToArray();
        return $"<size={textScale}%>{prefix} {string.Join(separator, prompts)} {suffix}</size>";
    }

    private string GetSpriteString(ControllerType controllerType, int actionIndex) {
        return controllerType switch {
            ControllerType.Keyboard => $"<size={iconScale}%><sprite=\"Keyboard\", index={actionIndex}></size>",
            ControllerType.XBox => $"<size={iconScale}%><sprite=\"XBox\", index={actionIndex}></size>",
            ControllerType.PlayStation => $"<size={iconScale}%><sprite=\"PlayStation\", index={actionIndex}></size>",
            _ => $"<size={iconScale}%><sprite=\"XBox\", index={actionIndex}></size>"
        };
    }
}
