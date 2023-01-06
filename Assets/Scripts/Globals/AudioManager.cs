using UnityEngine;

public class AudioManager : BaseComponent
{
    public AudioSource musicAudioSource;
    public AudioSource soundEffectsAudioSource;
    public AudioClip backgroundMusic;
    
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            if (backgroundMusic)
            {
                PlayBackground(backgroundMusic);
            }

            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (backgroundMusic)
        {
            PlayBackground(backgroundMusic);
        }
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