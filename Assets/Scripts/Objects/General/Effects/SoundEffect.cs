using UnityEngine;

public class SoundEffect : BaseComponent
{
    public void Play(AudioClip clip)
    {
        AudioManager.Play(clip);
    }

    public void PlayBackground(AudioClip clip)
    {
        AudioManager.PlayBackground(clip);
    }
}