using System.Collections.Generic;
using UnityEngine;

public class PeriodicRelativeHitDetector : IHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly HitCallable hitCallable;
    private readonly float interval;
    private readonly bool startImmediately;
    private EventListener detectPeriodicallyEvent;
    private readonly Dictionary<Hitbox, float> hitTimes;

    public PeriodicRelativeHitDetector(EventManager eventManager, Hitbox hitbox, HitCallable hitCallable, float interval, bool startImmediately = true)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        this.interval = interval;
        this.startImmediately = startImmediately;
        hitTimes = new();
    }

    public void Start()
    {
        detectPeriodicallyEvent = eventManager.Attach(() => true, () =>
        {
            DetectHits();
        });
    }

    private void DetectHits()
    {
        foreach (Hitbox hit in hitbox.DetectHits())
        {
            if (!hitTimes.ContainsKey(hit) && !startImmediately)
            {
                hitTimes[hit] = Time.time;
                continue;
            }
            if (!hitTimes.ContainsKey(hit) || Time.time - hitTimes[hit] >= interval)
            {
                hitTimes[hit] = Time.time;
                hitCallable(hit);
            }
        }
    }

    public void Stop()
    {
        eventManager.Detach(detectPeriodicallyEvent);
    }
}
