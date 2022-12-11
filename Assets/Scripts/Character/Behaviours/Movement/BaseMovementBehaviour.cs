using UnityEngine;

public abstract class BaseMovementBehaviour : PlayableBehaviour
{
    public float cooldown;

    private float cooldownStartTime;

    public override void Awake()
    {
        base.Awake();
        if (cooldown > 0)
        {
            cooldownStartTime = Time.time - cooldown;
            onPlay.AddListener(() => cooldownStartTime = Time.time);
        }
    }

    public override bool CanPlay()
    {
        return base.CanPlay()
            && AllStopped(typeof(ForcedBehaviour))
            && (cooldown <= 0 || Time.time - cooldownStartTime > cooldown);
    }
}
