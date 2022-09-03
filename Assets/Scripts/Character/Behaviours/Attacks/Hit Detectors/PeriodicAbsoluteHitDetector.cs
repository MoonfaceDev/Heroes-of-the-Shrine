using System.Collections;
using UnityEngine;

public class PeriodicAbsoluteHitDetector : IHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly HitCallable hitCallable;
    private readonly float interval;
    private readonly bool startImmediately;
    private Coroutine detectPeriodicallyCoroutine;

    public PeriodicAbsoluteHitDetector(EventManager eventManager, Hitbox hitbox, HitCallable hitCallable, float interval, bool startImmediately = true)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        this.interval = interval;
        this.startImmediately = startImmediately;
    }

    public void Start()
    {
        detectPeriodicallyCoroutine = eventManager.StartCoroutine(DetectPeriodically());
    }

    private void DetectHits()
    {
        foreach (Hitbox hit in hitbox.DetectHits())
        {
            hitCallable(hit);
        }
    }

    private IEnumerator DetectPeriodically()
    {
        if (startImmediately)
        {
            DetectHits();
        }
        while (true)
        {
            yield return new WaitForSeconds(interval);
            DetectHits();
        }
    }

    public void Stop()
    {
        eventManager.StopCoroutine(detectPeriodicallyCoroutine);
    }
}
