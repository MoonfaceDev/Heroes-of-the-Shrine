using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains behaviour's play and stop events
/// </summary>
[Serializable]
public class PlayEvents
{
    /// <value>
    /// Behaviour was played
    /// </value>
    public UnityEvent onPlay;

    /// <value>
    /// Behaviour was stopped
    /// </value>
    public UnityEvent onStop;
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

    /// <summary>
    /// Stops the behaviour.
    /// Calling it when behaviour is not playing will not do anything.
    /// </summary>
    public void Stop();
}

/// <summary>
/// Interface for specifying arguments for <see cref="PlayableBehaviour{T}.Play"/>
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Abstract variant of <see cref="CharacterBehaviour"/> for behaviours than can be played and stopped
/// </summary>
/// <typeparam name="T">Command type that <see cref="Play"/> method accepts</typeparam>
public abstract class PlayableBehaviour<T> : CharacterBehaviour, IPlayableBehaviour where T : ICommand
{
    [SerializeField] private PlayEvents playEvents;
    public PlayEvents PlayEvents => playEvents;

    public abstract bool Playing { get; }

    /// <returns>True if behaviour can be played. Override to add more conditions</returns>
    public virtual bool CanPlay(T command)
    {
        return Enabled;
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