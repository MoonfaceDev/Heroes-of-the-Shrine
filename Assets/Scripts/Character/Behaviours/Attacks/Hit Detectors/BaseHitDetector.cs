public abstract class BaseHitDetector
{
    public abstract void Start();
    public abstract void Stop();

    protected static bool OverlapHittable(HittableHitbox hittable, Hitbox hitboxToCheck)
    {
        return hittable.Hitbox.OverlapHitbox(hitboxToCheck);
    }
}
