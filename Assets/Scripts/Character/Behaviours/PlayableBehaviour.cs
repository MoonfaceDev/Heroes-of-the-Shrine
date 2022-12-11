using UnityEngine.Events;

public abstract class PlayableBehaviour : CharacterBehaviour
{
    public UnityEvent onPlay;
    public UnityEvent onStop;

    public abstract bool Playing { get; }

    public virtual bool CanPlay()
    {
        return Enabled;
    }

    public abstract void Stop();

    private void OnDestroy()
    {
        Stop();
    }
}