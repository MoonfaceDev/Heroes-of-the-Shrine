using System.Linq;
using UnityEditor;
using UnityEngine;

public class FollowPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;

    private static readonly float distanceFromOtherEnemies = 0.5f;

    private IModifier speedModifier;
    private EventListener otherEnemiesEvent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        if (!player)
        {
            return;
        }
        MovableObject target = player.GetComponent<MovableObject>();

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();

        WalkBehaviour walkBehaviour = animator.GetComponent<WalkBehaviour>();
        speedModifier = new MultiplierModifier(speedMultiplier);
        walkBehaviour.speed.AddModifier(speedModifier);

        WalkableGrid grid = FindObjectOfType<WalkableGrid>();
        float nodeRadius = grid.nodeRadius;

        Character[] otherEnemies = new Character[0];

        otherEnemiesEvent = EventManager.Instance.Attach(() => true, () =>
        {
            otherEnemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy").Where(enemy => enemy != followBehaviour.Character).ToArray();
        }, false);

        followBehaviour.Play(
            target,
            () =>
            {
                return otherEnemies.SelectMany(enemy => grid.GetCircle(enemy.movableObject.GroundWorldPosition, distanceFromOtherEnemies + nodeRadius)).ToArray();
            },
            (out Vector3 direction) =>
            {
                Vector3[] closeEnemyPositions = otherEnemies
                .Where(enemy => enemy.movableObject.GroundDistance(grid.NodeFromWorldPoint(followBehaviour.MovableObject.WorldPosition).position) < distanceFromOtherEnemies + nodeRadius)
                .Select(enemy => enemy.movableObject.WorldPosition - Vector3.up * enemy.movableObject.WorldPosition.y).ToArray();
                if (closeEnemyPositions.Length == 0)
                {
                    direction = Vector3.zero;
                    return false;
                }
                Vector3 center = closeEnemyPositions.Aggregate(Vector3.zero, (acc, next) => acc + next) / closeEnemyPositions.Length;
                direction = (followBehaviour.MovableObject.WorldPosition - center).normalized;
                return true;
            }
        );
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Detach(otherEnemiesEvent);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();
        followBehaviour.Stop();

        WalkBehaviour walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed.RemoveModifier(speedModifier);
    }
}
