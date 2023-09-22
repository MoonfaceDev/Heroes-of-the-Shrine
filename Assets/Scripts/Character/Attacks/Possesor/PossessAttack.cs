using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PossessAttack : BaseAttack
{
    private class NoSpawnPointException : Exception
    {
    }

    [Serializable]
    public class AttackFlow
    {
        public float warningDuration;
        public float sourceActiveDuration;
        public int waveCount = 1;
    }

    public AttackFlow attackFlow;

    public GameObject possessSource;
    public int sourcesCount;
    public float spawnRadius;
    public float minSourcesDistance;

    private WalkableGrid walkableGrid;

    protected override void Awake()
    {
        base.Awake();
        walkableGrid = FindObjectOfType<WalkableGrid>();
    }

    protected override IEnumerator ActivePhase()
    {
        StartWave(0);
        yield return new WaitForSeconds((attackFlow.warningDuration + attackFlow.sourceActiveDuration) *
                                        attackFlow.waveCount);
    }

    private void StartWave(int waveIndex)
    {
        var alreadySpawned = new List<Vector3>();

        for (var i = 0; i < sourcesCount; i++)
        {
            try
            {
                var spawnPoint = GetSpawnPoint(alreadySpawned);
                alreadySpawned.Add(spawnPoint);
                var newPossessSource = GameEntity.Instantiate(possessSource, spawnPoint);
                newPossessSource.GetBehaviour<PossessSource>().Activate(
                    attackFlow.warningDuration,
                    attackFlow.sourceActiveDuration,
                    this
                );
            }
            catch (NoSpawnPointException)
            {
                Debug.LogWarning("Spawn point not found after 20 tries");
                break;
            }
        }

        if (waveIndex + 1 < attackFlow.waveCount)
        {
            eventManager.StartTimeout(() => StartWave(waveIndex + 1), attackFlow.warningDuration + attackFlow.sourceActiveDuration);
        }
    }

    private Vector3 GetSpawnPoint(List<Vector3> alreadySpawned, int maxDepth = 20)
    {
        if (maxDepth == 0)
        {
            throw new NoSpawnPointException();
        }

        var player = EntityManager.Instance.GetEntity(Tag.Player);
        var groundPlayerPosition = player.GroundWorldPosition;
        var relativePoint = 2 * spawnRadius * new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);
        var newPosition = groundPlayerPosition + relativePoint;
        if (!IsValidPosition(alreadySpawned, newPosition))
        {
            newPosition = GetSpawnPoint(alreadySpawned, maxDepth - 1);
        }

        return newPosition;
    }

    private bool IsValidPosition(List<Vector3> alreadySpawned, Vector3 newPosition)
    {
        return walkableGrid.IsInside(newPosition)
               && EntityManager.Instance.GetEntities(Tag.Barrier).ToArray()
                   .All(barrier => !barrier.GetBehaviour<Hitbox>().IsInside(newPosition))
               && alreadySpawned.All(existingPosition =>
                   Vector3.Distance(newPosition, existingPosition) > 2 * minSourcesDistance);
    }
}