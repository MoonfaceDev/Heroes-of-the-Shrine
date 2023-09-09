using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An hit detector for which each object can be detected only once
/// </summary>
[Serializable]
public class SingleHitDetector : BaseHitDetector
{
    public SceneCollisionDetector collisionDetector;

    private string detectionListener;

    public override void StartDetector(Action<Collision> listener)
    {
        var alreadyHit = new HashSet<IHittable>();
        detectionListener = GlobalEventManager.Instance.Register(() =>
        {
            var collisions = collisionDetector.Detect();
            var newCollisions = collisions.Where(collision => !alreadyHit.Contains(collision.Other));
            foreach (var collision in newCollisions)
            {
                alreadyHit.Add(collision.Other);
                listener(collision);
            }
        });
    }

    public override void StopDetector()
    {
        GlobalEventManager.Instance.Unregister(detectionListener);
    }
}