using UnityEngine;

/// <summary>
/// Plays a sound effect when character is walking
/// </summary>
[RequireComponent(typeof(WalkBehaviour))]
public class WalkSoundEffect : BaseComponent
{
    /// <value>
    /// <see cref="AudioSource"/> only for this character's walk sound effects
    /// </value>
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

        Register(() => walkAudioSource.volume = AudioManager.Instance.SoundVolume);
    }
}