public abstract class BaseHitDetector
{
    public abstract void Start();
    public abstract void Stop();

    protected bool OverlapHittable(HittableBehaviour hittable, Hitbox hitboxToCheck)
    {
        foreach (Hitbox hitbox in hittable.hitboxes)
        {
            if (hitboxToCheck.OverlapHitbox(hitbox, 0))
            {
                return true;
            }
        }
        return false;
    }
}
