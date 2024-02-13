using System;
using ExtEvents;
using UnityEngine;

/// <summary>
/// Contains behaviour's play and stop events
/// </summary>
[Serializable]
public class PlayEvents
{
    /// <value>
    /// Behaviour was played
    /// </value>
    [SerializeField] public ExtEvent onPlay;

    /// <value>
    /// Behaviour was stopped
    /// </value>
    [SerializeField] public ExtEvent onStop;
}

public interface IPlayableBehaviour
{
    /// <value>
    /// Behaviour's play and stop events
    /// </value>
    public PlayEvents PlayEvents { get; }

    /// <value>
    /// <c>true</c> if behaviour is currently playing
    /// </value>
    public bool Playing { get; }

    /// <value>
    /// If <c>true</c>, the behaviour cannot be played.
    /// </value>
    public bool Blocked { get; set; }

    /// <summary>
    /// Stops the behaviour.
    /// Calling it when behaviour is not playing will not do anything.
    /// </summary>
    public void Stop();
}

/// <summary>
/// Abstract variant of <see cref="CharacterBehaviour"/> for behaviours than can be played and stopped
/// </summary>
/// <typeparam name="T">Command type that <see cref="Play"/> method accepts</typeparam>
public abstract class PlayableBehaviour<T> : CharacterBehaviour, IPlayableBehaviour
{
    [SerializeField] private PlayEvents playEvents;

    /// <value>
    /// Behaviour's play and stop events
    /// </value>
    public PlayEvents PlayEvents => playEvents;

    /// <value>
    /// <c>true</c> if behaviour is currently playing
    /// </value>
    public abstract bool Playing { get; }

    /// <summary>
    /// If <c>true</c>, the behaviour cannot be played.
    /// Can be set to <c>true</c> multiple times, as it will change back to <c>false</c> only if it was set to <c>false</c> the same number of times.
    /// Should not affect an action that has already started.
    /// </summary>
    public bool Blocked
    {
        get => blockCount > 0;
        set
        {
            if (value)
            {
                blockCount++;
            }
            else
            {
                if (blockCount > 0)
                {
                    blockCount--;
                }
            }
        }
    }

    private int blockCount;

    /// <returns>True if behaviour can be played. Override to add more conditions</returns>
    public virtual bool CanPlay(T command)
    {
        return !Blocked;
    }

    /// <summary>
    /// Plays the behaviour. To customize the functionality, implement <see cref="DoPlay"/>
    /// </summary>
    /// <param name="command">Arguments of the behaviour</param>
    public void Play(T command)
    {
        if (!CanPlay(command))
        {
            return;
        }

        DoPlay(command);
        PlayEvents.onPlay.Invoke();
    }

    /// <summary>
    /// Executed when behaviour is played
    /// </summary>
    /// <param name="command">Arguments of the behaviour, same as those passed to <see cref="Play"/></param>
    protected abstract void DoPlay(T command);

    /// <summary>
    /// Stops the behaviour. Can be used safely also when <see cref="Playing"/> is <c>false</c>, as it will do nothing.
    /// </summary>
    public void Stop()
    {
        if (!Playing)
        {
            return;
        }

        DoStop();
        PlayEvents.onStop.Invoke();
    }

    /// <summary>
    /// Executed when behaviour is stopped
    /// </summary>
    protected abstract void DoStop();

    private void OnDestroy()
    {
        Stop();
    }
}