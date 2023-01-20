public interface IEffect : IPlayableBehaviour
{
    /// <returns>Progress of the effect from 0 to 1. Can be used by UI elements to display the progress</returns>
    public float GetProgress();
}

/// <summary>
/// Base class for effects that character can receive
/// </summary>
/// <typeparam name="T">Type of play command</typeparam>
public abstract class BaseEffect<T> : PlayableBehaviour<T>, IEffect where T : ICommand
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

    public override bool Playing => Active;

    public abstract float GetProgress();
}