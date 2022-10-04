using System;
using System.Collections.Generic;

public class SingleHitDetector : BaseHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly Action<HittableBehaviour> hitCallable;
    private readonly HashSet<HittableBehaviour> alreadyHit;
    private EventListener eventListener;

    public SingleHitDetector(EventManager eventManager, Hitbox hitbox, Action<HittableBehaviour> hitCallable)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        alreadyHit = new();
    }

    public override void Start()
    {
        eventListener = eventManager.Attach(
            () => true,
            () =>
            {
                HittableBehaviour[] hittables = UnityEngine.Object.FindObjectsOfType<HittableBehaviour>();
                foreach (HittableBehaviour hittable in hittables)
                {
                    if (!alreadyHit.Contains(hittable) && OverlapHittable(hittable, hitbox))
                    {
                        alreadyHit.Add(hittable);
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
