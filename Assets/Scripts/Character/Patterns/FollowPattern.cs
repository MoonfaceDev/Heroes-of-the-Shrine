using System;
using System.Linq;
using UnityEngine;

public class FollowPattern : BasePattern
{
    public Tag targetTag;
    public float speedMultiplier;

    private const float DistanceFromOtherEnemies = 0.5f;

    private string otherEnemiesListener;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var player = EntityManager.Instance.GetEntity(targetTag);
        if (!player)
        {
            return;
        }

        var target = player.GetEntity();

        var followBehaviour = animator.GetEntity().GetBehaviour<FollowBehaviour>();

        var walkBehaviour = animator.GetEntity().GetBehaviour<WalkBehaviour>();
        walkBehaviour.speed *= speedMultiplier;

        var grid = FindObjectOfType<WalkableGrid>();
        var nodeRadius = grid.nodeRadius;

        var otherEnemies = Array.Empty<GameEntity>();

        otherEnemiesListener = EventManager.Instance.Register(() =>
        {
            otherEnemies = EntityManager.Instance.GetEntities(Tag.Enemy)
                .Where(enemy => enemy != followBehaviour.MovableEntity)
                .Where(enemy => enemy.GetBehaviour<HealthSystem>().Alive)
                .ToArray();
        });

        followBehaviour.Play(new FollowBehaviour.Command
        {
            target = (MovableEntity)target,
            getExcluded = () =>
            {
                return otherEnemies
                    .SelectMany(enemy => grid.GetCircle(
                        enemy.GroundWorldPosition,
                        DistanceFromOtherEnemies + nodeRadius))
                    .ToArray();
            },
            getOverrideDirection = (out Vector3 direction) =>
            {
                var closeEnemyPositions = otherEnemies
                    .Where(enemy =>
                        enemy.GroundDistance(
                            grid.NodeFromWorldPoint(followBehaviour.MovableEntity.WorldPosition).position
                        ) < DistanceFromOtherEnemies + nodeRadius)
                    .Select(enemy =>
                        enemy.WorldPosition - Vector3.up * enemy.WorldPosition.y)
                    .ToArray();
                if (closeEnemyPositions.Length == 0)
                {
                    direction = Vector3.zero;
                    return false;
                }

                var center = closeEnemyPositions
                    .Aggregate(Vector3.zero, (acc, next) => acc + next) / closeEnemyPositions.Length;
                direction = (followBehaviour.MovableEntity.WorldPosition - center).normalized;
                return true;
            }
        });
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Unregister(otherEnemiesListener);

        var followBehaviour = animator.GetEntity().GetBehaviour<FollowBehaviour>();
        followBehaviour.Stop();

        var walkBehaviour = animator.GetEntity().GetBehaviour<WalkBehaviour>();
        walkBehaviour.speed /= speedMultiplier;
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(otherEnemiesListener);
    }
}