public class AbsoluteHitDetector : IHitDetector
{
    private readonly Hitbox hitbox;
    private readonly HitCallable hitCallable;

    public AbsoluteHitDetector(Hitbox hitbox, HitCallable hitCallable)
    {
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
    }

    public void Start()
    {
        foreach (Hitbox hit in hitbox.DetectHits())
        {
            hitCallable(hit);
        }
    }

    public void Stop()
    {}
}
