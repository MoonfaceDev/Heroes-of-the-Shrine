public abstract class BaseEffect : PlayableBehaviour
{
    public bool active
    {
        get => _active;
        protected set
        {
            _active = value;
            animator.SetBool(GetType().Name, _active);
        }
    }

    public override bool Playing => active;

    private bool _active;

    public abstract float GetProgress();
}
