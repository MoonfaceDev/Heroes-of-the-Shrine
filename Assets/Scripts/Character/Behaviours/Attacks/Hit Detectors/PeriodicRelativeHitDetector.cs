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
        hitTimes = new();
    }

    public override void Start()
    {
        detectPeriodicallyEvent = eventManager.Attach(() => true, () =>
        {
            DetectHits();
        });
    }

    private void DetectHits()
    {
        HittableBehaviour[] hittables = UnityEngine.Object.FindObjectsOfType<HittableBehaviour>();
        foreach (HittableBehaviour hittable in hittables)
        {
            if (OverlapHittable(hittable, hitbox))
            {
                if (!hitTimes.ContainsKey(hittable) && !startImmediately)
                {
                    hitTimes[hittable] = Time.time;
                    continue;
                }
                if (!hitTimes.ContainsKey(hittable) || Time.time - hitTimes[hittable] >= interval)
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
    }
}
