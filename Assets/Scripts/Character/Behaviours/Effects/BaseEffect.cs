public abstract class BaseEffect : PlayableBehaviour
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
