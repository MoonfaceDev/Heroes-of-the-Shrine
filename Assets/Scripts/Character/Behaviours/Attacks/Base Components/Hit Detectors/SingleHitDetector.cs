using System;
using System.Collections.Generic;

public class SingleHitDetector : BaseHitDetector
{
    private string detectionListener;

    protected override void DoStartDetector(Action<IHittable> hitCallable)
    {
        var alreadyHit = new HashSet<HittableHitbox>();
        detectionListener = Register(() =>
        {
            var hittables = FindObjectsOfType<HittableHitbox>();
            foreach (var hittable in hittables)
            {
                if (!alreadyHit.Contains(hittable) && hittable.Hitbox.OverlapHitbox(hitbox))
                {
                    alreadyHit.Add(hittable);
                    hitCallable(hittable);
                }
            }
        });
    }

    public override void StopDetector()
    {
        Unregister(detectionListener);
    }
}