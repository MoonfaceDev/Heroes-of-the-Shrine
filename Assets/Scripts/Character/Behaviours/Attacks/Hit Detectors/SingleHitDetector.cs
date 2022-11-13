using System;
using System.Collections.Generic;

public class SingleHitDetector : BaseHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly Action<IHittable> hitCallable;
    private readonly HashSet<HittableBehaviour> alreadyHit;
    private EventListener eventListener;

    public SingleHitDetector(EventManager eventManager, Hitbox hitbox, Action<IHittable> hitCallable)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        alreadyHit = new HashSet<HittableBehaviour>();
    }

    public override void Start()
    {
        eventListener = eventManager.Attach(
            () => true,
            () =>
            {
                var hittables = UnityEngine.Object.FindObjectsOfType<HittableHitbox>();
                foreach (var hittable in hittables)
                {
                    if (!alreadyHit.Contains(hittable.hittableBehaviour) && OverlapHittable(hittable, hitbox))
                    {
                        alreadyHit.Add(hittable.hittableBehaviour);
                        hitCallable(hittable);
                    }
                }
            },
            single: false
        );
    }

    public override void Stop()
    {
        eventManager.Detach(eventListener);
        alreadyHit.Clear();
    }
}
