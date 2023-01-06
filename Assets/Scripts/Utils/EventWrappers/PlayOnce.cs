using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayOnce : BaseComponent
{
    private static readonly HashSet<string> Played = new();

    public string uniqueId;

    public UnityEvent firstEvent;

    public UnityEvent restEvent;

    public bool playOnAwake;

    public void Play()
    {
        if (!Played.Contains(uniqueId))
        {
            Played.Add(uniqueId);
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