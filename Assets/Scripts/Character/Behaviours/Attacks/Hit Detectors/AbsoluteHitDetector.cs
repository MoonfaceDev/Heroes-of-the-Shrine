using System;
using Object = UnityEngine.Object;

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