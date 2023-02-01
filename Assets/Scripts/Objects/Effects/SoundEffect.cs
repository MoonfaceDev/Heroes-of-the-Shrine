using UnityEngine;

/// <summary>
/// Helper component that calls <see cref="AudioManager.Play"/>.
/// Use it only when you cannot access the global <see cref="AudioBehaviour"/> directly.
/// </summary>
public class SoundEffect : BaseComponent
{
    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="clip">Audio asset to play</param>
    public void Play(AudioClip clip)
    {
        AudioManager.Play(clip);
    }
}