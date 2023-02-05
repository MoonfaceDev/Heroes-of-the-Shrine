using UnityEngine;

public interface IMovementBehaviour : IControlledBehaviour
{
}

public abstract class BaseMovementBehaviour<T> : PlayableBehaviour<T>, IMovementBehaviour
{
    public float cooldown;

    private float cooldownStartTime;

    protected override void Awake()
    {
        base.Awake();
        if (cooldown > 0)
        {
            cooldownStartTime = Time.time - cooldown;
            PlayEvents.onPlay += () => cooldownStartTime = Time.time;
        }
    }

    public override bool CanPlay(T command)
    {
        return base.CanPlay(command)
               && (cooldown <= 0 || Time.time - cooldownStartTime > cooldown);
    }
}