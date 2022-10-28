using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowPattern : BasePattern
{
    public string targetTag;
    public float speedMultiplier;
    public float distanceFromOtherEnemies = 1.5f;

    private EventListener otherEnemiesEvent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        if (!player)
        {
            return;
        }

        MovableObject target = player.GetComponent<MovableObject>();
        float nodeRadius = FindObjectOfType<WalkableGrid>().nodeRadius;

        Character[] otherEnemies = new Character[0];
        Vector3[] closeEnemyPositions = new Vector3[0];

        otherEnemiesEvent = EventManager.Instance.Attach(() => true, () =>
        {
            otherEnemies = CachedObjectsManager.Instance.GetObjects<Character>("Enemy").Where(enemy => enemy != followBehaviour.Character).ToArray();
            closeEnemyPositions = otherEnemies
            .Where(enemy => enemy.movableObject.GroundDistance(followBehaviour.MovableObject.position) < (distanceFromOtherEnemies + nodeRadius) * 1.5f)
            .Select(enemy => enemy.movableObject.position - Vector3.up * enemy.movableObject.position.y).ToArray();
        }, false);

        followBehaviour.Play(target, speedMultiplier, (Node node) =>
        {
            foreach (Character enemy in otherEnemies)
            {
                if (enemy.movableObject.GroundDistance(node.position) < distanceFromOtherEnemies + nodeRadius)
                {
                    return true;
                }
            }
            return false;
        }, 
        (out Vector3 direction) =>
        {
            if (closeEnemyPositions.Length == 0)
            {
                direction = Vector3.zero;
                return false;
            }
            Vector3 center = closeEnemyPositions.Aggregate(Vector3.zero, (acc, next) => acc + next) / closeEnemyPositions.Length;
            direction = (followBehaviour.MovableObject.position - center).normalized;
            return true;
        });
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Detach(otherEnemiesEvent);

        FollowBehaviour followBehaviour = animator.GetComponent<FollowBehaviour>();
        followBehaviour.Stop();
    }
}
