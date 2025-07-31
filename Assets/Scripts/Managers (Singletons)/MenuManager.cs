/*References PlayerManager singleton!*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour {

    // Assign in Inspector
    [Header("Menu Prefabs")]
    public Canvas canvasPrefab;
    public EventSystem eventSystemPrefab;   //configured with our InputActionMap
    public GameObject mainMenuPrefab;
    public GameObject settingsMenuPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject characterSelectionMenuPrefab;
    public GameObject controlsMenuPrefab;
    public GameObject creditsMenuPrefab;
    public GameObject audioMenuPrefab;
    public GameObject assistModeMenuPrefab;

    [Header("Menu Sounds")]
    public AudioClip menuMusic;
    public AudioClip menuPauseSound;
    public AudioClip menuResumeSound;
    public AudioClip menuSelectSound;
    public AudioClip menuConfirmSound;
    public AudioClip menuCancelSound;
    public AudioClip menuErrorSound;
    //

    public static MenuManager instance;
    public enum Menu{Main, Settings, Pause, CharacterSelection, Controls, Credits, Audio, AssistMode};

    public Menu currentMenu {get; private set;}
    private Dictionary<Menu, GameObject> menus;
    private Stack<Menu> previousMenus = new();
    public Canvas canvas {get; private set;}
    public float gameSpeed = 1;

    public void Awake() {
        // Debug.Log("Menu Manager: Awake");

        if (instance != null) {
            Debug.LogWarning("Attempting to create another Menu Manager");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);

        canvas = Instantiate(canvasPrefab, transform);
        canvas.gameObject.SetActive(false);

        EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem != null) {
            // There should only be one EventSystem per scene
            Destroy(eventSystem.gameObject);
        }
        Instantiate(eventSystemPrefab, transform);

        menus = new Dictionary<Menu, GameObject> {
            {Menu.Main, Instantiate(mainMenuPrefab, canvas.transform)},
            {Menu.Settings, Instantiate(settingsMenuPrefab, canvas.transform)},
            {Menu.Pause, Instantiate(pauseMenuPrefab, canvas.transform)},
            {Menu.CharacterSelection, Instantiate(characterSelectionMenuPrefab, canvas.transform)},
            {Menu.Controls, Instantiate(controlsMenuPrefab, canvas.transform)},
            {Menu.Credits, Instantiate(creditsMenuPrefab, canvas.transform)},
            {Menu.Audio, Instantiate(audioMenuPrefab, canvas.transform)},
            {Menu.AssistMode, Instantiate(assistModeMenuPrefab, canvas.transform)}
        };

        foreach (var menu in menus) {
            if (menu.Value.activeSelf) {
                Debug.Log("Save menu prefabs as inactive: " + menu.Key);
                menu.Value.SetActive(false);
            }
        }
    }

    public void OpenMenu(Menu newMenu) {
        // Debug.Log("Menu Manager: OpenMenu()");

        if (newMenu == Menu.Pause) {
            AudioManager.instance.PlaySoundEffect(menuPauseSound, 0.5f);
        }
        PauseGame();
        AudioManager.instance.PlayMenuMusic(menuMusic);
        PlayerManager.instance.SetActionMaps("UI");
        currentMenu = newMenu;
        canvas.gameObject.SetActive(true);
        CurrentSetActive(true);
    }

    public void CloseMenu() {
        // Debug.Log("Menu Manager: CloseMenu()");

        AudioManager.instance.StopMenuMusic();
        if (currentMenu == Menu.Pause) {
            StartCoroutine(CloseWithSound(menuResumeSound, 0.5f));
        } else if (currentMenu == Menu.CharacterSelection) {
            StartCoroutine(CloseWithSound(menuConfirmSound, 0.2f));
        }
        UnpauseGame();
        PlayerManager.instance.SetActionMaps("Player");
        previousMenus.Clear();
        CurrentSetActive(false);
        canvas.gameObject.SetActive(false);
    }

    public void NextMenu(Menu nextMenu, bool allowBack = true) {
        // Debug.Log($"Menu Manager: NextMenu({nextMenu})");

        AudioManager.instance.PlaySoundEffect(menuConfirmSound, 0.2f);
        CurrentSetActive(false);
        if (allowBack) { previousMenus.Push(currentMenu); }
        currentMenu = nextMenu;
        CurrentSetActive(true);
    }

    public void PreviousMenu() {
        // Debug.Log("Menu Manager: PreviousMenu()");

        if (previousMenus.Count == 0) { return; }
        AudioManager.instance.PlaySoundEffect(menuCancelSound, 0.2f);
        CurrentSetActive(false);
        currentMenu = previousMenus.Pop();
        CurrentSetActive(true);
    }

    public void OnButtonSelect(BaseEventData eventData) {
        // Only play sound if not triggered by mouse click, avoid double sound effect
        if (eventData is PointerEventData pointerEventData) {
            if (pointerEventData.pointerId == 1) return; // left mouse button
        }
        AudioManager.instance.PlaySoundEffect(menuSelectSound, 0.5f);
    }

    private void CurrentSetActive(bool status) {
        menus[currentMenu].SetActive(status);
    }

    private void PauseGame() {
        Time.timeScale = 0;
        AudioManager.instance.PauseBackgroundMusic();
    }

    private void UnpauseGame() {
        Time.timeScale = gameSpeed;
        AudioManager.instance.ResumeBackgroundMusic();
    }

    IEnumerator CloseWithSound(AudioClip closeSound, float volumeScale = 1) {
        AudioManager.instance.PlaySoundEffect(closeSound, volumeScale);
        yield return new WaitForSecondsRealtime(closeSound.length);
    }
}
