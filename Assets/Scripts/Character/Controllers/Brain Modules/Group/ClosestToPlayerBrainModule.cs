using System.Linq;
using UnityEngine;

/// <summary>
/// State machine parameter telling if this is the closest enemy to the player
/// </summary>
public class ClosestToPlayerBrainModule : BrainModule
{
    private const string IsClosestToPlayerParameter = "isClosestToPlayer";
    private static readonly int IsClosestToPlayer = Animator.StringToHash(IsClosestToPlayerParameter);

    protected override void Update()
    {
        base.Update();

        var player = EntityManager.Instance.GetEntity(Tag.Player);
        var enemies = EntityManager.Instance.GetEntities(Tag.Enemy);

        var closestEnemy = enemies.Aggregate((prev, next) =>
        {
            if (!prev) return next;
            if (!next) return prev;
            var nextDistance = player.GroundDistance(next.WorldPosition);
            var prevDistance = player.GroundDistance(prev.WorldPosition);
            return nextDistance < prevDistance ? next : prev;
        });
        
        StateMachine.SetBool(IsClosestToPlayer, MovableEntity == closestEnemy);
    }

    public override string[] GetParameters()
    {
        return new[] { IsClosestToPlayerParameter };
    }
}