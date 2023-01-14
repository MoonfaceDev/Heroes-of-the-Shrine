using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class WalkSoundEffect : BaseComponent
{
    public AudioSource walkAudioSource;

    private void Awake()
    {
        var walkBehaviour = GetComponent<WalkBehaviour>();
        var jumpBehaviour = GetComponent<JumpBehaviour>();

        walkBehaviour.PlayEvents.onPlay.AddListener(() =>
        {
            if (jumpBehaviour && jumpBehaviour.Playing) return;
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        });

        walkBehaviour.PlayEvents.onStop.AddListener(() => walkAudioSource.Stop());

        if (jumpBehaviour)
        {
            jumpBehaviour.PlayEvents.onPlay.AddListener(() => walkAudioSource.Stop());
            jumpBehaviour.PlayEvents.onStop.AddListener(() =>
            {
                if (walkBehaviour.Playing)
                {
                    walkAudioSource.Play();
                }
            });
        }

        Register(() => walkAudioSource.volume = AudioManager.Instance.soundEffectsAudioSource.volume);
    }
}