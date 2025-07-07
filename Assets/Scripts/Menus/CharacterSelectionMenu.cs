using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionMenu : MonoBehaviour {

    // Assign in Inspector
    [Header("Visible Components")]
    public TextMeshProUGUI title;
    public List<GameObject> characters;
    public List<GameObject> textsPlayer;
    public List<GameObject> players;

    [Header("Buttons")]
    public GameObject swapButton;
    public GameObject playButton;

    [Header("Controller Scheme Icons")]
    public Sprite keyboardIcon;
    public Sprite xboxIcon;
    public Sprite playstationIcon;
    //

    private bool allPlayersJoined;
    private bool isSwapped = false;

    public void Start() {
        allPlayersJoined = false;
        PlayerManager.instance.ResetPlayers();
        // Deactivate everything except for the title.
        characters.ForEach(character => character.SetActive(false));
        players.ForEach(player => player.SetActive(false));
        swapButton.SetActive(false);
        playButton.SetActive(false);
    }

    public void OnEnable() {
        title.text = "Waiting for both players...";
        if (isSwapped) {
            SwapPortraits();
        }
        Start();
    }

    public void Update() {
        if (allPlayersJoined) {
            return;
        }
        // Activate the character portraits as players join.
        if (PlayerManager.instance.GetPlayerCount() >= 1) {
            characters[0].SetActive(true);
            players[0].GetComponentInChildren<Image>().sprite = GetControllerIcon(CharacterName.Felix);
            players[0].SetActive(true);
        }
        if (PlayerManager.instance.GetPlayerCount() == 2) {
            characters[1].SetActive(true);
            players[1].GetComponentInChildren<Image>().sprite = GetControllerIcon(CharacterName.Anna);
            players[1].SetActive(true);

            title.text = "Character Selection";

            swapButton.SetActive(true);
            playButton.SetActive(true);
            playButton.GetComponent<Button>().Select();
            allPlayersJoined = true;
        }
    }

    public void Swap() {
        if (!PlayerManager.instance.IsReady()) {
            Debug.LogWarning("Character Selection Menu: swap button should be inactive");
            return;
        }
        PlayerManager.instance.SwapCharacters();
        SwapPortraits();
    }

    public void Play() {
        if (!PlayerManager.instance.IsReady()) {
            Debug.LogWarning("Character Selection Menu: play button should be inactive");
            return;
        }
        PlayerManager.instance.AssignRenderPrefabs();
        LevelManager.instance.AssignTargetGroup();
        MenuManager.instance.CloseMenu();
    }

    private void SwapPortraits() {
        foreach (GameObject character in characters) {
            Vector3 position = character.GetComponent<RectTransform>().anchoredPosition;
            position.x = -position.x;
            character.GetComponent<RectTransform>().anchoredPosition = position;
        }
        isSwapped = !isSwapped;
    }

    private Sprite GetControllerIcon(CharacterName character) {
        ControllerType controllerType = PlayerManager.instance.GetControllerType(character);
        return controllerType switch {
            ControllerType.Keyboard => keyboardIcon,
            ControllerType.XBox => xboxIcon,
            ControllerType.PlayStation => playstationIcon,
            _ => xboxIcon
        };
    }
}
