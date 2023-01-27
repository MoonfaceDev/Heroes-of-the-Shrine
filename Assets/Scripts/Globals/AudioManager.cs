using UnityEngine;

public class AudioManager : BaseComponent
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectsAudioSource;

    public float MusicVolume
    {
        get => musicAudioSource.volume;
        set
        {
            musicAudioSource.volume = value;
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }
    
    public float SoundVolume
    {
        get => soundEffectsAudioSource.volume;
        set
        {
            soundEffectsAudioSource.volume = value;
            PlayerPrefs.SetFloat(SoundVolumeKey, value);
        }
    }

    private const string MusicVolumeKey = "musicVolume"; 
    private const string SoundVolumeKey = "soundVolume"; 

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        musicAudioSource.volume = PlayerPrefs.GetFloat(MusicVolumeKey, 1);
        soundEffectsAudioSource.volume = PlayerPrefs.GetFloat(SoundVolumeKey, 1);
    }

    public static void PlayBackground(AudioClip clip)
    {
        Instance.musicAudioSource.clip = clip;
        Instance.musicAudioSource.Play();
    }

    public static void StopBackground()
    {
        Instance.musicAudioSource.Stop();
    }

    public static void Play(AudioClip clip)
    {
        Instance.soundEffectsAudioSource.PlayOneShot(clip);
    }
}