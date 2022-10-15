using System;

public abstract class BaseEffect : CharacterBehaviour
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

    public event Action onActivate;
    public event Action onStop;

    private bool _active;

    public abstract float GetProgress();

    protected void InvokeOnActivate()
    {
        onActivate?.Invoke();
    }

    protected void InvokeOnStop()
    {
        onStop?.Invoke();
    }
}
