using System;

public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    public float interval;
    public bool startImmediately = true;
    
    public event Action OnDetect;

    private EventListener detectPeriodicallyEvent;

    protected override void DoStartDetector(Action<IHittable> hitCallable)
    {
        if (startImmediately)
        {
            DetectHits(hitCallable);
        }
        detectPeriodicallyEvent = EventManager.Instance.StartInterval(() => DetectHits(hitCallable), interval);
    }

    private void DetectHits(Action<IHittable> hitCallable)
    {
        OnDetect?.Invoke();
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
        EventManager.Instance.Detach(detectPeriodicallyEvent);
    }
}
