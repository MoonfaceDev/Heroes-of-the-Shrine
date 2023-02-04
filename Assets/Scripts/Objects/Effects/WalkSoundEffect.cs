using UnityEngine;

/// <summary>
/// Plays a sound effect when character is walking
/// </summary>
public class WalkSoundEffect : CharacterBehaviour
{
    /// <value>
    /// <see cref="AudioSource"/> only for this character's walk sound effects
    /// </value>
    public AudioSource walkAudioSource;

    protected override void Awake()
    {
        base.Awake();
        var walkBehaviour = GetBehaviour<WalkBehaviour>();
        var jumpBehaviour = GetBehaviour<JumpBehaviour>();

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