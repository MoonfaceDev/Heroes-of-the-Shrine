using System.Linq;
using UnityEngine;

public class ClosestEnemyBrainModule : BrainModule
{
    private const string ClosestEnemyDistanceParameter = "closestEnemyDistance";
    private static readonly int ClosestEnemyDistance = Animator.StringToHash(ClosestEnemyDistanceParameter);

    protected override void Update()
    {
        base.Update();

        var enemies = EntityManager.Instance.GetEntities(Tag.Enemy);

        var closestEnemy = enemies.Aggregate(enemies[0], (prev, next) =>
        {
            if (!prev) return next;
            if (!next || next == MovableEntity)
            {
                return prev;
            }

            var nextDistance = MovableEntity.GroundDistance(next.WorldPosition);
            var prevDistance = MovableEntity.GroundDistance(prev.WorldPosition);
            return nextDistance < prevDistance ? next : prev;
        });

        StateMachine.SetFloat(ClosestEnemyDistance, MovableEntity.GroundDistance(closestEnemy.WorldPosition));
    }

    public override string[] GetParameters()
    {
        return new[] { ClosestEnemyDistanceParameter };
    }
}