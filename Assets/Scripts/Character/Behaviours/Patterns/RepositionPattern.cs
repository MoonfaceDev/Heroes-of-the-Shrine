using System;
using System.Linq;
using UnityEngine;

public class RepositionPattern : BasePattern
{
    public Vector3 destination;
    public float speedMultiplier;

    private const float DistanceFromOtherEnemies = 0.5f;

    private string otherEnemiesListener;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        var followBehaviour = animator.GetComponent<FollowBehaviour>();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed *= speedMultiplier;

        var grid = FindObjectOfType<WalkableGrid>();
        var nodeRadius = grid.nodeRadius;

        var otherEnemies = Array.Empty<Character>();

        otherEnemiesListener = EventManager.Instance.Register(() =>
        {
            otherEnemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy")
                .Where(enemy => enemy != followBehaviour.Character)
                .Where(enemy => enemy.GetComponent<HealthSystem>().Alive)
                .ToArray();
        });

        followBehaviour.Play(
            destination,
            () =>
            {
                return otherEnemies.SelectMany(enemy =>
                        grid.GetCircle(enemy.movableObject.GroundWorldPosition, DistanceFromOtherEnemies + nodeRadius))
                    .ToArray();
            }
        );
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Unregister(otherEnemiesListener);

        var followBehaviour = animator.GetComponent<FollowBehaviour>();
        followBehaviour.Stop();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed /= speedMultiplier;
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(otherEnemiesListener);
    }
}