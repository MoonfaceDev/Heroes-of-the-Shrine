using UnityEngine;

[RequireComponent(typeof(WalkBehaviour))]
public class WalkSoundEffect : BaseComponent
{
    public AudioSource walkAudioSource;

    private void Awake()
    {
        var walkBehaviour = GetComponent<WalkBehaviour>();
        var jumpBehaviour = GetComponent<JumpBehaviour>();

        walkBehaviour.PlayEvents.onPlay += () =>
        {
            if (jumpBehaviour && jumpBehaviour.Playing) return;
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        };

        walkBehaviour.PlayEvents.onStop += () => walkAudioSource.Stop();

        if (jumpBehaviour)
        {
            jumpBehaviour.PlayEvents.onPlay += () => walkAudioSource.Stop();
            jumpBehaviour.PlayEvents.onStop += () =>
            {
                if (walkBehaviour.Playing)
                {
                    walkAudioSource.Play();
                }
            };
        }

        Register(() => walkAudioSource.volume = AudioManager.Instance.soundEffectsAudioSource.volume);
    }
}