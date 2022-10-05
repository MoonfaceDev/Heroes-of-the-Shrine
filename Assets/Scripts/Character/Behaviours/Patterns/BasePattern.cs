using System;

public abstract class BasePattern : CharacterBehaviour
{
    public event Action onStart;
    public event Action onStop;

    public bool active
    {
        get => _active;
        private set
        {
            _active = value;
        }
    }

    private bool _active;

    public virtual void StartPattern()
    {
        active = true;
        onStart?.Invoke();
    }

    public virtual void StopPattern()
    {
        active = false;
        onStop?.Invoke();
    }
}
