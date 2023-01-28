﻿using System;
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

        var autoWalkBehaviour = animator.GetComponent<AutoWalkBehaviour>();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed *= speedMultiplier;

        var grid = FindObjectOfType<WalkableGrid>();
        var nodeRadius = grid.nodeRadius;

        var otherEnemies = Array.Empty<GameEntity>();

        otherEnemiesListener = EventManager.Instance.Register(() =>
        {
            otherEnemies = EntityManager.Instance.GetEntities(Tag.Enemy)
                .Where(enemy => enemy != autoWalkBehaviour.MovableEntity)
                .Where(enemy => enemy.GetComponent<HealthSystem>().Alive)
                .ToArray();
        });

        autoWalkBehaviour.Play(new AutoWalkBehaviour.Command(
            destination,
            () =>
            {
                return otherEnemies.SelectMany(enemy =>
                        grid.GetCircle(enemy.GroundWorldPosition, DistanceFromOtherEnemies + nodeRadius))
                    .ToArray();
            }
        ));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        EventManager.Instance.Unregister(otherEnemiesListener);

        var autoWalkBehaviour = animator.GetComponent<AutoWalkBehaviour>();
        autoWalkBehaviour.Stop();

        var walkBehaviour = animator.GetComponent<WalkBehaviour>();
        walkBehaviour.speed /= speedMultiplier;
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(otherEnemiesListener);
    }
}