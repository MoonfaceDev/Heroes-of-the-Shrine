using System;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRelativeHitDetector : BaseHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly Action<HittableBehaviour> hitCallable;
    private readonly float interval;
    private readonly bool startImmediately;
    private EventListener detectPeriodicallyEvent;
    private readonly Dictionary<HittableBehaviour, float> hitTimes;

    public PeriodicRelativeHitDetector(EventManager eventManager, Hitbox hitbox, Action<HittableBehaviour> hitCallable, float interval, bool startImmediately = true)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        this.interval = interval;
        this.startImmediately = startImmediately;
        hitTimes = new Dictionary<HittableBehaviour, float>();
    }

    public override void Start()
    {
        detectPeriodicallyEvent = eventManager.Attach(() => true, DetectHits);
    }

    private void DetectHits()
    {
        var hittables = UnityEngine.Object.FindObjectsOfType<HittableBehaviour>();
        foreach (var hittable in hittables)
        {
            if (OverlapHittable(hittable, hitbox) && !hitTimes.ContainsKey(hittable))
            {
                if (!startImmediately)
                {
                    hitTimes[hittable] = Time.time;
                    continue;
                }
                if (Time.time - hitTimes[hittable] >= interval)
                {
                    hitTimes[hittable] = Time.time;
                    hitCallable(hittable);
                }
            }
        }
    }

    public override void Stop()
    {
        eventManager.Detach(detectPeriodicallyEvent);
        hitTimes.Clear();
    }
}
