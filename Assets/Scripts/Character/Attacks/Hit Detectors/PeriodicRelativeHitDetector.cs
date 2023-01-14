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
    public float interval;
    public bool startImmediately = true;

    private string detectionListener;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        var hitTimes = new Dictionary<HittableBehaviour, float>();
        detectionListener = EventManager.Instance.Register(() => DetectHits(hitCallable, hitTimes));
    }

    private void DetectHits(Action<HittableHitbox> hitCallable, IDictionary<HittableBehaviour, float> hitTimes)
    {
        var hittables = Object.FindObjectsOfType<HittableHitbox>();
        foreach (var hittable in hittables)
        {
            if (hittable.Hitbox.OverlapHitbox(hitbox) && !hitTimes.ContainsKey(hittable.hittableBehaviour))
            {
                if (!startImmediately)
                {
                    hitTimes[hittable.hittableBehaviour] = Time.time;
                    continue;
                }

                if (Time.time - hitTimes[hittable.hittableBehaviour] >= interval)
                {
                    hitTimes[hittable.hittableBehaviour] = Time.time;
                    hitCallable(hittable);
                }
            }
        }
    }

    public override void StopDetector()
    {
        EventManager.Instance.Unregister(detectionListener);
    }
}