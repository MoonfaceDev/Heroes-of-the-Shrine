using System;

public class AbsoluteHitDetector : BaseHitDetector
{
    protected override void DoStartDetector(Action<IHittable> hitCallable)
    {
        var hittables = FindObjectsOfType<HittableHitbox>();
        foreach (var hittable in hittables)
        {
            if (hittable.Hitbox.OverlapHitbox(hitbox))
            {
                hitCallable(hittable);
            }
        }
    }

    public override void StopDetector()
    {
    }
}