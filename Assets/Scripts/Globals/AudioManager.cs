using UnityEngine;

/// <summary>
/// Singleton responsible for playing background music and SFX
/// </summary>
public class AudioManager : BaseComponent
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectsAudioSource;

    /// <value>
    /// Volume of background music (0-1)
    /// </value>
    public float MusicVolume
    {
        get => musicAudioSource.volume;
        set
        {
            musicAudioSource.volume = value;
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }
    
    /// <value>
    /// Volume of SFX (0-1)
    /// </value>
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

    /// <value>
    /// Instance of the singleton
    /// </value>
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

    /// <summary>
    /// Plays a new background music. If a previous clip was played, it will be replaced
    /// </summary>
    /// <param name="clip">Audio asset to play</param>
    public static void PlayBackground(AudioClip clip)
    {
        Instance.musicAudioSource.clip = clip;
        Instance.musicAudioSource.Play();
    }

    /// <summary>
    /// Stops background music. If it wasn't playing, then nothing happens
    /// </summary>
    public static void StopBackground()
    {
        Instance.musicAudioSource.Stop();
    }

    /// <summary>
    /// Plays a sound effect. Not affecting other sound effects or background music, meaning two clips can be played
    /// simultaneously
    /// </summary>
    /// <param name="clip">Audio asset to play</param>
    public static void Play(AudioClip clip)
    {
        Instance.soundEffectsAudioSource.PlayOneShot(clip);
    }
}