using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour {

    // Assign in Inspector
    public Slider musicSlider;
    public Slider soundEffectsSlider;
    public AudioClip testSoundEffect;
    //

    private float prevSoundEffectSliderValue;


    public void Start() {
        musicSlider.Select();
    }

    public void OnEnable() {
        musicSlider.Select();
    }

    public void SetMusicVolume() {
        AudioManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSoundEffectVolume() {
        AudioManager.instance.SetSoundEffectVolume(soundEffectsSlider.value);
    }

    public void PlayTestSoundEffect() {
        if (prevSoundEffectSliderValue == soundEffectsSlider.value) return;
        if (EventSystem.current.currentSelectedGameObject != soundEffectsSlider.gameObject) return;
        AudioManager.instance.PlaySoundEffect(testSoundEffect);
        prevSoundEffectSliderValue = soundEffectsSlider.value;
    }

    public void PlayTestSoundEffectGamepad(BaseEventData eventData) {
        AxisEventData axisData = eventData as AxisEventData;
        if (prevSoundEffectSliderValue == soundEffectsSlider.maxValue && axisData.moveDir == MoveDirection.Right) return;
        PlayTestSoundEffect();
    }

    public void Back() {
        MenuManager.instance.PreviousMenu();
    }
}
