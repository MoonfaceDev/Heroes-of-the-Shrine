using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

internal class NoSpawnPointException : Exception
{
}

public class PossesAttack : SimpleAttack
{
    public int sourcesCount;
    public float spawnRadius;
    public PossesSource possesSource;
    public float warningDuration;
    public float sourceActiveDuration;

    private WalkableGrid walkableGrid;

    public override void Awake()
    {
        base.Awake();
        PreventWalking(true);

        walkableGrid = FindObjectOfType<WalkableGrid>();

        OnStartActive += () =>
        {
            var alreadySpawned = new List<Vector3>();
            for (var i = 0; i < sourcesCount; i++)
            {
                var newPossesSource = Instantiate(possesSource.gameObject);
                try
                {
                    var spawnPoint = GetSpawnPoint(alreadySpawned);
                    alreadySpawned.Add(spawnPoint);
                    newPossesSource.GetComponent<MovableObject>().WorldPosition = spawnPoint;
                    newPossesSource.GetComponent<PossesSource>().Activate(warningDuration, sourceActiveDuration);
                }
                catch (NoSpawnPointException)
                {
                    Debug.LogError("Spawn point not found after 10 tries");
                    break;
                }
            }
        };
    }

    private Vector3 GetSpawnPoint(IEnumerable<Vector3> alreadySpawned, int maxDepth = 10)
    {
        if (maxDepth == 0)
        {
            throw new NoSpawnPointException();
        }

        var player = CachedObjectsManager.Instance.GetObject<Character>("Player");
        var groundPlayerPosition = player.movableObject.GroundWorldPosition;
        var relativePoint = new Vector3(Random.value, 0, Random.value);
        var newPosition = groundPlayerPosition + relativePoint * spawnRadius;
        var enumerable = alreadySpawned.ToList();
        if (!IsValidPosition(enumerable, newPosition))
        {
            newPosition = GetSpawnPoint(enumerable, maxDepth - 1);
        }

        return newPosition;
    }

    private bool IsValidPosition(IEnumerable<Vector3> alreadySpawned, Vector3 newPosition)
    {
        return !walkableGrid.IsInside(newPosition)
               && alreadySpawned.All(existingPosition =>
                   Vector3.Distance(newPosition, existingPosition) > 2 * spawnRadius);
    }
}