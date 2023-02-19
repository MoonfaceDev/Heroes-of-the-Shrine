using System;
using Object = UnityEngine.Object;

/// <summary>
/// An hit detector that detects hits periodically, with a given interval
/// </summary>
[Serializable]
public class PeriodicAbsoluteHitDetector : BaseHitDetector
{
    /// <value>
    /// Interval between detections, in seconds
    /// </value>
    public float interval;
    
    /// <value>
    /// If true, a detection occurs immediately when <see cref="BaseHitDetector.StartDetector"/> is called. Otherwise,
    /// the first detection occurs after a <see cref="interval"/>.
    /// </value>
    public bool startImmediately = true;

    /// <value>
    /// Invoked when detection occurs
    /// </value>
    public event Action OnDetect;

    private string detectionInterval;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        detectionInterval = EventManager.Instance.StartInterval(() => DetectHits(hitCallable), interval);
        if (startImmediately)
        {
            DetectHits(hitCallable);
        }
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