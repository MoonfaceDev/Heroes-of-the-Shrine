using System;
using Object = UnityEngine.Object;

[Serializable]
public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    public float interval;
    public bool startImmediately = true;

    public event Action OnDetect;

    private string detectionInterval;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        if (startImmediately)
        {
            DetectHits(hitCallable);
        }

        detectionInterval = EventManager.Instance.StartInterval(() => DetectHits(hitCallable), interval);
    }

    private void DetectHits(Action<HittableHitbox> hitCallable)
    {
        OnDetect?.Invoke();
        var hittables = Object.FindObjectsOfType<HittableHitbox>();
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
        EventManager.Instance.Unregister(detectionInterval);
    }
}