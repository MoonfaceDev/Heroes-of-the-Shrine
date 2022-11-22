using System;
using System.Collections.Generic;

public class SingleHitDetector : BaseHitDetector
{
    private EventListener eventListener;

    protected override void DoStartDetector(Action<IHittable> hitCallable)
    {
        var alreadyHit = new HashSet<HittableBehaviour>();
        eventListener = EventManager.Instance.Attach(
            () => true,
            () =>
            {
                var hittables = FindObjectsOfType<HittableHitbox>();
                foreach (var hittable in hittables)
                {
                    if (!alreadyHit.Contains(hittable.hittableBehaviour) && hittable.Hitbox.OverlapHitbox(hitbox))
                    {
                        alreadyHit.Add(hittable.hittableBehaviour);
                        hitCallable(hittable);
                    }
                }
            },
            single: false
        );
    }

    public override void StopDetector()
    {
        EventManager.Instance.Detach(eventListener);
    }
}