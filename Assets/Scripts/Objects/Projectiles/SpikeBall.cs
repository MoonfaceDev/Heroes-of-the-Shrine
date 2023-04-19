using ExtEvents;
using UnityEngine;

public class SpikeBall : EntityBehaviour
{
    public Animator animator;
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
        latchHitDetector.StartDetector(hittable =>
        {
            latchHitDetector.StopDetector();
            var hitWorldPosition = Entity.WorldPosition;
            Entity.parent = hittable.Character.Entity;
            Entity.position = Vector3.zero + hitWorldPosition.y * Vector3.up + latchZ * Vector3.forward;
            hittable.Hit(latchHitExecutor,
                new Hit { source = source, victim = hittable, direction = Mathf.RoundToInt(Mathf.Sign(velocity.x)) }
            );
            MovableEntity.velocity = Vector3.zero;
            animator.SetTrigger($"{GetType().Name}-latch");
            onLatch.Invoke();
        }, source.AttackManager.hittableTags);
    }

    public void Explode()
    {
        MovableEntity.velocity = Vector3.zero;
        latchHitDetector.StopDetector();
        animator.SetTrigger($"{GetType().Name}-explode");
        Destroy(gameObject, explodeAnimationDuration);
        onExplode.Invoke();

        StartTimeout(() => explosionHitDetector.StartDetector(hittable =>
        {
            if (hittable.Character.Entity == Entity.parent)
            {
                hittable.Hit(explosionSourceHitExecutor, new Hit { source = sourceAttack, victim = hittable });
            }
            else
            {
                hittable.Hit(explosionHitExecutor,
                    new Hit
                    {
                        source = sourceAttack,
                        victim = hittable,
                        direction = Mathf.RoundToInt(
                            Mathf.Sign(hittable.Entity.WorldPosition.x - Entity.WorldPosition.x))
                    }
                );
            }
        }, sourceAttack.AttackManager.hittableTags), explosionStartDelay);
    }
}