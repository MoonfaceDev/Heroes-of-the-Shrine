using System;
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
                Instance.PlayBackground(backgroundMusic);
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

    public void PlayBackground(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}