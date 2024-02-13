using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

[Serializable]
public class SceneCollisionDetector : ICollisionDetector
{
    public Hitbox hitbox;
    
    public IEnumerable<Collision> Detect()
    {
        var hitboxes = Object.FindObjectsOfType<HittableHitbox>();
        var overlappingHitboxes = hitboxes.Where(other => other.Hitbox.OverlapHitbox(hitbox));
        return overlappingHitboxes.Select(other =>
        {
            var point = other.Hitbox.GetIntersectionCenter(hitbox);
            return new Collision(other, point);
        });
    }
}