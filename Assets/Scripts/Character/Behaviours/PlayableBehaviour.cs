using System;

public abstract class PlayableBehaviour : CharacterBehaviour
{
    public event Action OnPlay;
    public event Action OnStop;

    public abstract bool Playing
    {
        get;
    }

    public virtual bool CanPlay()
    {
        return Enabled;
    }

    public abstract void Stop();

    protected void InvokeOnPlay()
    {
        OnPlay?.Invoke();
    }

    protected void InvokeOnStop()
    {
        OnStop?.Invoke();
    }

    private void OnDestroy()
    {
        Stop();
    }
}
