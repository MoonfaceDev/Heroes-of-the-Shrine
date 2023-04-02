using UnityEngine;

public class SpikeBall : EntityBehaviour
{
    public Animator animator;
    [Header("Latch")] public SingleHitDetector latchHitDetector;
    public ChainHitExecutor latchHitExecutor;
    [Header("Explosion")] public AbsoluteHitDetector explosionHitDetector;
    public ChainHitExecutor explosionSourceHitExecutor;
    public ChainHitExecutor explosionHitExecutor;
    public float explodeAnimationDuration;

    private MovableEntity MovableEntity => (MovableEntity)Entity;
    private BaseAttack sourceAttack;

    public void Fire(Vector3 velocity, BaseAttack source)
    {
        sourceAttack = source;
        MovableEntity.velocity = velocity;
        latchHitDetector.StartDetector(hittable =>
        {
            latchHitDetector.StopDetector();
            var targetY = Entity.WorldPosition.y;
            Entity.parent = hittable.Character.Entity;
            Entity.position = Entity.TransformToRelative(Entity.parent.WorldPosition + targetY * Vector3.up); 
            hittable.Hit(latchHitExecutor,
                new Hit { source = source, victim = hittable, direction = Mathf.RoundToInt(Mathf.Sign(velocity.x)) }
            );
            MovableEntity.velocity = Vector3.zero;
            animator.SetTrigger($"{GetType().Name}-latch");
        }, source.AttackManager.hittableTags);
    }

    public void Explode()
    {
        MovableEntity.velocity = Vector3.zero;
        latchHitDetector.StopDetector();
        animator.SetTrigger($"{GetType().Name}-explode");
        Destroy(gameObject, explodeAnimationDuration);

        explosionHitDetector.StartDetector(hittable =>
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
        }, sourceAttack.AttackManager.hittableTags);
    }
}