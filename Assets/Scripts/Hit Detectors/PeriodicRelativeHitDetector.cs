using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// An hit detector that detects hits periodically, with a given interval, relative to the hit object
/// </summary>
[Serializable]
public class PeriodicRelativeHitDetector : BaseHitDetector
{
    /// <value>
    /// Interval between consecutive hits of a single hittable, in seconds
    /// </value>
    public float interval;

    private string detectionListener;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        var hitTimes = new Dictionary<IHittable, float>();
        detectionListener = GlobalEventManager.Instance.Register(() => DetectHits(hitCallable, hitTimes));
    }

    private void DetectHits(Action<HittableHitbox> hitCallable, IDictionary<IHittable, float> hitTimes)
    {
        var hittables = Object.FindObjectsOfType<HittableHitbox>();
        foreach (var hittable in hittables)
        {
            if (!hittable.Hitbox.OverlapHitbox(hitbox)) continue;
            if (hitTimes.ContainsKey(hittable) && !(Time.time - hitTimes[hittable] >= interval)) continue;
            hitTimes[hittable] = Time.time;
            hitCallable(hittable);
        }
    }

    public override void StopDetector()
    {
        GlobalEventManager.Instance.Unregister(detectionListener);
    }
}