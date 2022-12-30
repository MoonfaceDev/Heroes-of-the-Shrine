using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayOnce : MonoBehaviour
{
    private static bool _isTransition = true;
    
    public UnityEvent firstEvent;
    public UnityEvent restEvent;

    public bool playOnAwake;

    public void Play()
    {
        if (_isTransition)
        {
            _isTransition = false;
            firstEvent.Invoke();
        }
        else
        {
            restEvent.Invoke();
        }
    }

    private void Awake()
    {
        if (playOnAwake)
        {
            Play();
        }
    }
}
