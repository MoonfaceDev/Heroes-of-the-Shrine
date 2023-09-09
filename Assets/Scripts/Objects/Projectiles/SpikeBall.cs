using ExtEvents;
using UnityEngine;

public class SpikeBall : EntityBehaviour
{
    public Animator animator;
    public ExtEvent onFire;
    [Header("Latch")] public SingleHitDetector latchHitDetector;
    public ChainHitExecutor latchHitExecutor;
    public float latchZ;
    public ExtEvent onLatch;
    [Header("Explosion")] public AbsoluteHitDetector explosionHitDetector;
    public ChainHitExecutor explosionSourceHitExecutor;
    public ChainHitExecutor explosionHitExecutor;
    public float explosionStartDelay;
    public float explodeAnimationDuration;
    public ExtEvent onExplode;

    private MovableEntity MovableEntity => (MovableEntity)Entity;
    private BaseAttack sourceAttack;

    public void Fire(Vector3 velocity, BaseAttack source)
    {
        sourceAttack = source;
        MovableEntity.velocity = velocity;
        latchHitDetector.StartDetector(collision =>
        {
            if (!source.AttackManager.CanHit(collision.Other)) return;
            latchHitDetector.StopDetector();
            Latch(collision, Mathf.Sign(velocity.x), source);
        });
        onFire.Invoke();
    }

    public void Latch(Collision collision, float hitDirection, BaseAttack source)
    {
        sourceAttack = source;
        Entity.parent = collision.Other.RelatedEntity;
        Entity.position = collision.Point.y * Vector3.up + latchZ * Vector3.forward;
        collision.Other.Hit(latchHitExecutor, new Hit(collision, source, Mathf.RoundToInt(hitDirection)));
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
        latchHitDetector.StopDetector();
        animator.SetTrigger($"{GetType().Name}-explode");
        Destroy(gameObject, explodeAnimationDuration);
        onExplode.Invoke();

        eventManager.StartTimeout(() => explosionHitDetector.StartDetector(collision =>
        {
            if (!sourceAttack.AttackManager.CanHit(collision.Other)) return;
            
            if (collision.Other.RelatedEntity == Entity.parent)
            {
                collision.Other.Hit(explosionSourceHitExecutor, new Hit(collision, sourceAttack));
            }
            else
            {
                collision.Other.Hit(explosionHitExecutor,
                    new Hit(collision, sourceAttack, collision.Other.RelatedEntity.WorldPosition - Entity.WorldPosition)
                );
            }
        }), explosionStartDelay);
    }
}