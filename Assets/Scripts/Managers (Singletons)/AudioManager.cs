using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    // Assign in Inspector
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource soundEffect;
    //

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);

        menuMusic = Instantiate(menuMusic, transform);
        menuMusic.volume = 0.2f;
        backgroundMusic = Instantiate(backgroundMusic, transform);
        soundEffect = Instantiate(soundEffect, transform);
    }

    public void PlayBackgroundMusic(AudioClip audioClip, float delay = 0) {
        if (audioClip == backgroundMusic.clip && backgroundMusic.isPlaying) { return; }

        backgroundMusic.clip = audioClip;
        if (!menuMusic.isPlaying) {
            backgroundMusic.PlayDelayed(delay);
        }
    }

    public void PauseBackgroundMusic() {
        if (backgroundMusic.isPlaying) {
            backgroundMusic.Pause();
        }
    }

    public void ResumeBackgroundMusic(float delay = 0) {
        if (!backgroundMusic.isPlaying && !menuMusic.isPlaying) {
            backgroundMusic.PlayDelayed(delay);
        }
    }

    public void PlayMenuMusic(AudioClip audioClip, float delay = 0) {
        if (audioClip == menuMusic.clip && menuMusic.isPlaying) { return; }
        menuMusic.clip = audioClip;
        menuMusic.PlayDelayed(delay);
    }

    public void StopMenuMusic() {
        menuMusic.Stop();
    }

    public void PlaySoundEffect(AudioClip audioClip, float volumeScale = 1) {
        soundEffect.PlayOneShot(audioClip, volumeScale);
    }

    public void SetMusicVolume(float volume) {
        menuMusic.volume = 0.2f * volume;
        backgroundMusic.volume = volume;
    }

    public void SetSoundEffectVolume(float volume) {
        soundEffect.volume = volume;
    }
}
