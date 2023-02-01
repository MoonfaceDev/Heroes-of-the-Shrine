using System.Collections.Generic;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Splits calls to two events - one for first-time calls and the other for the rest
/// </summary>
public class PlayOnce : BaseComponent
{
    private static readonly HashSet<string> Played = new();

    /// <value>
    /// Unique ID of the component. Must be unique among all <see cref="PlayOnce"/> components.
    /// </value>
    public string uniqueId;

    /// <value>
    /// Invoked when <see cref="Play"/> is called for the first time in the game
    /// </value>
    [SerializeField] public ExtEvent firstEvent;

    /// <value>
    /// Invoked when <see cref="Play"/> is called if it is not the first time
    /// </value>
    [SerializeField] public ExtEvent restEvent;

    /// <value>
    /// If true, <see cref="Play"/> is called when scene is loaded
    /// </value>
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