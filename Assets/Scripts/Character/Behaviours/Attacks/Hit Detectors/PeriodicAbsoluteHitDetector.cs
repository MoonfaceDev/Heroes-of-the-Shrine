using System;

public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly Action<HittableBehaviour> hitCallable;
    private readonly float interval;
    private readonly bool startImmediately;
    private EventListener detectPeriodicallyEvent;

    public PeriodicAbsoluteHitDetector(EventManager eventManager, Hitbox hitbox, Action<HittableBehaviour> hitCallable, float interval, bool startImmediately = true)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        this.interval = interval;
        this.startImmediately = startImmediately;
    }

    public override void Start()
    {
        if (startImmediately)
        {
            DetectHits();
        }
        detectPeriodicallyEvent = eventManager.StartInterval(DetectHits, interval);
    }

    private void DetectHits()
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
    {
        eventManager.Detach(detectPeriodicallyEvent);
    }
}
