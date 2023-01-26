using System.Collections.Generic;
using ExtEvents;
using UnityEngine;
using UnityEngine.Events;

public class PlayOnce : BaseComponent
{
    private static readonly HashSet<string> Played = new();

    public string uniqueId;

    [SerializeField] public ExtEvent firstEvent;

    [SerializeField] public ExtEvent restEvent;

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