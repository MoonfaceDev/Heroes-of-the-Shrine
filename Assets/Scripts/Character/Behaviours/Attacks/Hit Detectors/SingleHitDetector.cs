using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[Serializable]
public class SingleHitDetector : BaseHitDetector
{
    private string detectionListener;

    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
        var alreadyHit = new HashSet<HittableHitbox>();
        detectionListener = EventManager.Instance.Register(() =>
        {
            var hittables = Object.FindObjectsOfType<HittableHitbox>();
            foreach (var hittable in hittables)
            {
                if (!alreadyHit.Contains(hittable) && hittable.Hitbox.OverlapHitbox(hitbox))
                {
                    alreadyHit.Add(hittable);
                    hitCallable(hittable);
                }
            }
        });
    }

    public override void StopDetector()
    {
        EventManager.Instance.Unregister(detectionListener);
    }
}