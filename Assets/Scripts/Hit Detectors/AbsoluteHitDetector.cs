using System;
using Object = UnityEngine.Object;

/// <summary>
/// An hit detector that detects hits only in the frame it is started
/// </summary>
[Serializable]
public class AbsoluteHitDetector : BaseHitDetector
{
    protected override void DoStartDetector(Action<HittableHitbox> hitCallable)
    {
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
    }
}