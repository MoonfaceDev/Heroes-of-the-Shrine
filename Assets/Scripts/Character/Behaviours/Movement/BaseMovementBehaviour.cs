using UnityEngine;

public abstract class BaseMovementBehaviour : PlayableBehaviour
{
    public float cooldown = 0;

    private float cooldownStartTime;

    public override void Awake()
    {
        base.Awake();
        if (cooldown > 0)
        {
            cooldownStartTime = Time.time - cooldown;
            OnStop += () => cooldownStartTime = Time.time;
        }
    }

    public override bool CanPlay()
    {
        return base.CanPlay()
            && AllStopped(typeof(KnockbackBehaviour), typeof(StunBehaviour)) 
            && (cooldown <= 0 || Time.time - cooldownStartTime > cooldown);
    }
}
