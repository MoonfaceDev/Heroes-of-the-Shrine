using System;

public class AbsoluteHitDetector : BaseHitDetector
{
    private readonly Hitbox hitbox;
    private readonly Action<HittableBehaviour> hitCallable;

    public AbsoluteHitDetector(Hitbox hitbox, Action<HittableBehaviour> hitCallable)
    {
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
    }

    public override void Start()
    {
        HittableBehaviour[] hittables = UnityEngine.Object.FindObjectsOfType<HittableBehaviour>();
        foreach (HittableBehaviour hittable in hittables)
        {
            if (OverlapHittable(hittable, hitbox))
            {
                hitCallable(hittable);
            }
        }
    }

    public override void Stop()
    {}
}
