using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

internal class NoSpawnPointException : Exception
{
}

public class PossesAttack : BaseAttack
{
    public float anticipateDuration;
    public float recoveryDuration;
    public int sourcesCount;
    public float spawnRadius;
    public float minSourcesDistance;
    public PossesSource possesSource;
    public float warningDuration;
    public float sourceActiveDuration;

    private WalkableGrid walkableGrid;

    public override void Awake()
    {
        base.Awake();
        OnPlay += () =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
            MovableObject.velocity = Vector3.zero;
        };
        OnFinishRecovery += () => EnableBehaviours(typeof(WalkBehaviour));
        OnStop += () => EnableBehaviours(typeof(WalkBehaviour));
        walkableGrid = FindObjectOfType<WalkableGrid>();

        OnStartActive += () =>
        {
            var alreadySpawned = new List<Vector3>();
            for (var i = 0; i < sourcesCount; i++)
            {
                try
                {
                    var spawnPoint = GetSpawnPoint(alreadySpawned);
                    alreadySpawned.Add(spawnPoint);
                    var newPossesSource = Instantiate(possesSource.gameObject);
                    newPossesSource.GetComponent<MovableObject>().WorldPosition = spawnPoint;
                    newPossesSource.GetComponent<PossesSource>().Activate(warningDuration, sourceActiveDuration,
                        AttackManager.hittableTags);
                }
                catch (NoSpawnPointException)
                {
                    Debug.LogError("Spawn point not found after 10 tries");
                    break;
                }
            }
        };
    }

    private void Start()
    {
        Play();
    }

    private Vector3 GetSpawnPoint(List<Vector3> alreadySpawned, int maxDepth = 20)
    {
        if (maxDepth == 0)
        {
            throw new NoSpawnPointException();
        }

        var player = CachedObjectsManager.Instance.GetObject<Character>("Player");
        var groundPlayerPosition = player.movableObject.GroundWorldPosition;
        var relativePoint = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f) * 2 * spawnRadius;
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
               && alreadySpawned.All(existingPosition =>
                   Vector3.Distance(newPosition, existingPosition) > 2 * minSourcesDistance);
    }

    protected override IEnumerator AnticipateCoroutine()
    {
        yield return new WaitForSeconds(anticipateDuration);
    }

    protected override IEnumerator ActiveCoroutine()
    {
        yield return new WaitForSeconds(0);
    }

    protected override IEnumerator RecoveryCoroutine()
    {
        yield return new WaitForSeconds(recoveryDuration);
    }
}