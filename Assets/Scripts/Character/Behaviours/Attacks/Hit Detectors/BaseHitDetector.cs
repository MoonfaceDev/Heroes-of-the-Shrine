using System.Linq;

public abstract class BaseHitDetector
{
    public abstract void Start();
    public abstract void Stop();

    protected static bool OverlapHittable(HittableBehaviour hittable, Hitbox hitboxToCheck)
    {
        return hittable.hitboxes.Any(hitbox => hitboxToCheck.OverlapHitbox(hitbox));
    }
}
