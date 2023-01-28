using UnityEngine;

/// <summary>
/// Base non-generic interface for an effect
/// </summary>
public interface IEffect : IPlayableBehaviour
{
    /// <returns>Progress of the effect from 0 to 1. Can be used by UI elements to display the progress</returns>
    public float GetProgress();
}

/// <summary>
/// Base class for effects that character can receive by getting hit
/// </summary>
/// <typeparam name="T">Type of play command</typeparam>
[RequireComponent(typeof(HittableBehaviour))]
public abstract class BaseEffect<T> : PlayableBehaviour<T>, IEffect
{
    /// <value>
    /// Is the effect currently active
    /// </value>
    public bool Active
    {
        get => active;
        protected set
        {
            active = value;
            Animator.SetBool(GetType().Name, active);
        }
    }

    private bool active;

    public override bool CanPlay(T command)
    {
        return base.CanPlay(command) && GetComponent<HittableBehaviour>().CanGetHit();
    }

    public override bool Playing => Active;
    
    /// <returns>Progress of the effect from 0 to 1. Can be used by UI elements to display the progress</returns>
    public abstract float GetProgress();
}