using System;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRelativeHitDetector : BaseHitDetector
{
    public float interval;
    public bool startImmediately = true;
    
    private string detectionListener;

    protected override void DoStartDetector(Action<IHittable> hitCallable)
    {
        var hitTimes = new Dictionary<HittableBehaviour, float>();
        detectionListener = Register(() => DetectHits(hitCallable, hitTimes));
    }

    private void DetectHits(Action<IHittable> hitCallable, IDictionary<HittableBehaviour, float> hitTimes)
    {
        var hittables = FindObjectsOfType<HittableHitbox>();
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
        Unregister(detectionListener);
    }
}
