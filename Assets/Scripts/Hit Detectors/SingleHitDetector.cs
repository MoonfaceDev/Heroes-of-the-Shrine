using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// An hit detector for which each object can be detected only once
/// </summary>
[Serializable]
public class SingleHitDetector : BaseHitDetector
{
    private string detectionListener;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        var alreadyHit = new HashSet<IHittable>();
        detectionListener = EventManager.Instance.Register(() =>
        {
            var hittables = Object.FindObjectsOfType<HittableHitbox>();
            foreach (var hittable in hittables)
            {
                if (alreadyHit.Contains(hittable) || !hittable.Hitbox.OverlapHitbox(hitbox)) continue;
                alreadyHit.Add(hittable);
                hitCallable(hittable);
            }
        });
    }

    public override void StopDetector()
    {
        EventManager.Instance.Unregister(detectionListener);
    }
}