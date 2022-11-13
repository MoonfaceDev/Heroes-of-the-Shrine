using System;

public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    public event Action OnDetect;

    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly Action<IHittable> hitCallable;
    private readonly float interval;
    private readonly bool startImmediately;
    private EventListener detectPeriodicallyEvent;

    public PeriodicAbsoluteHitDetector(EventManager eventManager, Hitbox hitbox, Action<IHittable> hitCallable, float interval, bool startImmediately = true)
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
        OnDetect?.Invoke();
        var hittables = UnityEngine.Object.FindObjectsOfType<HittableHitbox>();
        foreach (var hittable in hittables)
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
