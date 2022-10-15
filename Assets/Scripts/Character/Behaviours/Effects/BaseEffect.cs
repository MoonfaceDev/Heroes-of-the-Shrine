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
    public event Action onDeactivate;

    private bool _active;

    public abstract void Deactivate();
    public abstract float GetProgress();

    protected void InvokeOnActivate()
    {
        onActivate?.Invoke();
    }

    protected void InvokeOnDeactivate()
    {
        onDeactivate?.Invoke();
    }
}
