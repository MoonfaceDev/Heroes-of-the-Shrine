public interface IEffect : IPlayableBehaviour
{
    public bool Active { get; }
    public float GetProgress();
}

public abstract class BaseEffect<T> : PlayableBehaviour<T>, IEffect where T : ICommand
{
    public bool Active
    {
        get => active;
        protected set
        {
            active = value;
            Animator.SetBool(GetType().Name, active);
        }
    }

    public override bool Playing => Active;

    private bool active;

    public abstract float GetProgress();
}