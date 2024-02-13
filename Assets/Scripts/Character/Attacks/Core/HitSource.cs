using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class HitSource
{
    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    public void Start(BaseAttack source, [CanBeNull] Action<Collision> onHit = null)
    {
        hitDetector.StartDetector(collision =>
        {
            var victim = collision.Other;

            if (!source.AttackManager.hittableTags.Intersect(victim.RelatedEntity.tags).Any()) return;
            if (!victim.CanGetHit()) return;

            victim.Hit(hitExecutor, new Hit(collision, source, source.Entity.WorldRotation));
            onHit?.Invoke(collision);
        });
    }

    public void Stop()
    {
        hitDetector.StopDetector();
    }
}