using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayOnce : MonoBehaviour
{
    private static readonly HashSet<string> _played = new();

    public UnityEvent firstEvent;

    public UnityEvent restEvent;

    public bool playOnAwake;

    [Readonly] [TextArea]
    public string warning = "Pay attention: game object name has to be unique in the game! (across all scenes)";

    public void Play()
    {
        if (!_played.Contains(name))
        {
            _played.Add(name);
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