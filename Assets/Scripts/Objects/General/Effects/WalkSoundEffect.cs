using UnityEngine;

public class WalkSoundEffect : MonoBehaviour
{
    public AudioSource walkAudioSource;
    
    private void Awake()
    {
        var walkBehaviour = GetComponent<WalkBehaviour>();
        var jumpBehaviour = GetComponent<JumpBehaviour>();
        
        walkBehaviour.onPlay.AddListener(() =>
        {
            if (jumpBehaviour && jumpBehaviour.Playing) return; 
            walkAudioSource.Play();
        });
        
        walkBehaviour.onStop.AddListener(() => walkAudioSource.Stop());

        if (jumpBehaviour)
        {
            jumpBehaviour.onPlay.AddListener(() => walkAudioSource.Stop());
            jumpBehaviour.onStop.AddListener(() =>
            {
                if (walkBehaviour.Playing)
                {
                    walkAudioSource.Play();
                }
            });
        }
    }
}