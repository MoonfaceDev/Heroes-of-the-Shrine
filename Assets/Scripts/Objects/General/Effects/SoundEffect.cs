using UnityEngine;

public class SoundEffect : MonoBehaviour
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