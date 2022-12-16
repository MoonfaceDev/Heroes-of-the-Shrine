using System;
using System.Linq;
using UnityEngine;

public class FollowPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private const float DistanceFromOtherEnemies = 0.5f;

    private IModifier speedModifier;
    private EventListener otherEnemiesEvent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var player = GameObject.FindGameObjectWithTag(targetTag);
        if (!player)
        {
            return;
        }

        var target = player.GetComponent<MovableObject>();

        var followBehaviour = animator.GetComponent<FollowBehaviour>();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        var grid = FindObjectOfType<WalkableGrid>();
        var nodeRadius = grid.nodeRadius;

        var otherEnemies = Array.Empty<Character>();

        otherEnemiesEvent = EventManager.Instance.Attach(() => true,
            () =>
            {
                otherEnemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy")
                    .Where(enemy => enemy != followBehaviour.Character)
                    .Where(enemy => enemy.GetComponent<HealthSystem>().Alive)
                    .ToArray();
            }, false);

        followBehaviour.Play(
            target,
            () =>
            {
                return otherEnemies
                    .SelectMany(enemy => grid.GetCircle(
                        enemy.movableObject.GroundWorldPosition,
                        DistanceFromOtherEnemies + nodeRadius))
                    .ToArray();
            },
            (out Vector3 direction) =>
            {
                var closeEnemyPositions = otherEnemies
                    .Where(enemy =>
                        enemy.movableObject.GroundDistance(
                            grid.NodeFromWorldPoint(followBehaviour.MovableObject.WorldPosition).position
                        ) < DistanceFromOtherEnemies + nodeRadius)
                    .Select(enemy =>
                        enemy.movableObject.WorldPosition - Vector3.up * enemy.movableObject.WorldPosition.y)
                    .ToArray();
                if (closeEnemyPositions.Length == 0)
                {
                    direction = Vector3.zero;
                    return false;
                }

                var center = closeEnemyPositions
                    .Aggregate(Vector3.zero, (acc, next) => acc + next) / closeEnemyPositions.Length;
                direction = (followBehaviour.MovableObject.WorldPosition - center).normalized;
                return true;
            }
        );
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Detach(otherEnemiesEvent);

        var followBehaviour = animator.GetComponent<FollowBehaviour>();
        followBehaviour.Stop();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed.RemoveModifier(speedModifier);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Detach(otherEnemiesEvent);
    }
}