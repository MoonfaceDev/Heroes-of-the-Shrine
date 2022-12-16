using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip backgroundMusic;

    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
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
        Instance.audioSource.clip = clip;
        Instance.audioSource.Play();
    }

    public static void Play(AudioClip clip)
    {
        Instance.audioSource.PlayOneShot(clip);
    }
}