using System;
using System.Linq;

public abstract class PlayableBehaviour : CharacterBehaviour
{
    public event Action onPlay;
    public event Action onStop;

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
        onPlay?.Invoke();
    }

    protected void InvokeOnStop()
    {
        onStop?.Invoke();
    }

    protected void StopBehaviours(params Type[] behaviours)
    {
        foreach (Type type in behaviours)
        {
            foreach (PlayableBehaviour behaviour in GetComponents(type))
            {
                behaviour.Stop();
            }
        }
    }

    protected bool IsPlaying(Type type)
    {
        PlayableBehaviour behaviour = GetComponent(type) as PlayableBehaviour;
        return behaviour && behaviour.Playing;
    }

    protected bool AllStopped(params Type[] types)
    {
        return types.Select(type => GetComponents(type)).All(behaviours => behaviours.All(behaviour => !(behaviour as PlayableBehaviour).Playing));
    }
}
