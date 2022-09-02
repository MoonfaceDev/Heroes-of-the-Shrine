using System.Collections.Generic;

public class SingleHitDetector : IHitDetector
{
    private readonly EventManager eventManager;
    private readonly Hitbox hitbox;
    private readonly HitCallable hitCallable;
    private readonly HashSet<Hitbox> alreadyHit;
    private EventListener eventListener;

    public SingleHitDetector(EventManager eventManager, Hitbox hitbox, HitCallable hitCallable)
    {
        this.eventManager = eventManager;
        this.hitbox = hitbox;
        this.hitCallable = hitCallable;
        alreadyHit = new();
    }

    public void Start()
    {
        eventListener = eventManager.Attach(
            () => true,
            () =>
            {
                foreach (Hitbox hit in hitbox.DetectHits())
                {
                    if (!alreadyHit.Contains(hit))
                    {
                        alreadyHit.Add(hit);
                        hitCallable(hit);
                    }
                }
            },
            single: false
        );
    }

    public void Stop()
    {
        eventManager.Detach(eventListener);
    }
}
