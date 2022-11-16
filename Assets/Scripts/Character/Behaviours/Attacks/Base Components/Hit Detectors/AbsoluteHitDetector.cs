using System;

public class AbsoluteHitDetector : BaseHitDetector
{
    private readonly Hitbox hitbox;
    private readonly Action<IHittable> hitCallable;

    public AbsoluteHitDetector(Hitbox hitbox, Action<IHittable> hitCallable)
    {
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
    }

    public override void Start()
    {
        var hittables = UnityEngine.Object.FindObjectsOfType<HittableHitbox>();
        foreach (var hittable in hittables)
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
