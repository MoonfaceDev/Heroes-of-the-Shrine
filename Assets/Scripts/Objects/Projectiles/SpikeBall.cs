using System;
using ExtEvents;
using UnityEngine;

[Serializable]
public class SpikeBallHitExecutor : IHitExecutor
{
    public ChainHitExecutor sourceHitExecutor;
    public ChainHitExecutor explodeHitExecutor;
    
    public void Execute(Hit hit)
    {
        var isHittingSource = hit.Victim.RelatedEntity == hit.Source.Entity.parent;
        (isHittingSource ? sourceHitExecutor : explodeHitExecutor).Execute(hit);
    }
}

public class SpikeBall : EntityBehaviour
{
    public Animator animator;
    public ExtEvent onFire;
    
    [Header("Latch")] public HitSource latchHitSource;
    public float latchZ;
    public ExtEvent onLatch;
    
    [Header("Explosion")] public HitSource explosionHitSource;
    public float explosionStartDelay;
    public float explodeAnimationDuration;
    public ExtEvent onExplode;

    private MovableEntity MovableEntity => (MovableEntity)Entity;
    private BaseAttack sourceAttack;

    public void Fire(Vector3 velocity, BaseAttack source)
    {
        sourceAttack = source;
        MovableEntity.velocity = velocity;
        latchHitSource.Start(source, collision =>
        {
            latchHitSource.Stop();
            Latch(collision, source);
        });
        onFire.Invoke();
    }

    public void Latch(Collision collision, BaseAttack source)
    {
        sourceAttack = source;
        Entity.parent = collision.Other.RelatedEntity;
        Entity.position = collision.Point.y * Vector3.up + latchZ * Vector3.forward;
        MovableEntity.velocity = Vector3.zero;
        animator.SetTrigger($"{GetType().Name}-latch");
        onLatch.Invoke();
    }

    public void ExplodeAfter(float delay)
    {
        eventManager.StartTimeout(Explode, delay);
    }

    public void Explode()
    {
        MovableEntity.velocity = Vector3.zero;
        latchHitSource.Stop();
        animator.SetTrigger($"{GetType().Name}-explode");
        Destroy(gameObject, explodeAnimationDuration);
        onExplode.Invoke();

        eventManager.StartTimeout(() => explosionHitSource.Start(sourceAttack), explosionStartDelay);
    }
}