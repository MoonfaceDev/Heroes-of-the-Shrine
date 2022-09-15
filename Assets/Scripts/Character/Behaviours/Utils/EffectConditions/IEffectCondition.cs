using System;

public abstract class BaseEffectCondition
{
    public event Action onSet;

    public abstract float GetProgress();

    public abstract bool IsSet();
    
    public abstract void Set();

    protected void InvokeOnSet()
    {
        onSet?.Invoke();
    }
}
