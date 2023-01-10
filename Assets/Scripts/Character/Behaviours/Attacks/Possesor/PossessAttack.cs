using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PossessActivePhase : IAttackPhase
{
    public float warningDuration;
    public float sourceActiveDuration;
    public int waveCount = 1;

    public IEnumerator Play()
    {
        yield return new WaitForSeconds((warningDuration + sourceActiveDuration) * waveCount);
    }
}

[Serializable]
public class PossessAttackFlow : IAttackFlow
{
    public TimedAttackPhase anticipationPhase;
    public PossessActivePhase activePhase;
    public TimedAttackPhase recoveryPhase;
    public IAttackPhase AnticipationPhase => anticipationPhase;
    public IAttackPhase ActivePhase => activePhase;
    public IAttackPhase RecoveryPhase => recoveryPhase;
}

internal class NoSpawnPointException : Exception
{
}

public class PossessAttack : BaseAttack
{
    public PossessAttackFlow possessAttackFlow;
    public int sourcesCount;
    public float spawnRadius;
    public float minSourcesDistance;
    public PossessSource possessSource;
    public float effectDuration;
    public int sourceDamage;

    private WalkableGrid walkableGrid;

    public override void Awake()
    {
        base.Awake();
        PlayEvents.onPlay.AddListener(() =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
        });
        PlayEvents.onStop.AddListener(() => EnableBehaviours(typeof(WalkBehaviour)));
        walkableGrid = FindObjectOfType<WalkableGrid>();

        attackEvents.onStartActive.AddListener(() => StartWave(0));
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
                var newPossessSource = Instantiate(possessSource.gameObject,
                    MovableObject.ScreenCoordinates(spawnPoint), Quaternion.identity);
                newPossessSource.GetComponent<MovableObject>().WorldPosition = spawnPoint;
                newPossessSource.GetComponent<PossessSource>().Activate(
                    possessAttackFlow.activePhase.warningDuration,
                    possessAttackFlow.activePhase.sourceActiveDuration,
                    AttackManager.hittableTags,
                    effectDuration,
                    sourceDamage
                );
            }
            catch (NoSpawnPointException)
            {
                Debug.LogWarning("Spawn point not found after 20 tries");
                break;
            }
        }

        if (waveIndex + 1 < possessAttackFlow.activePhase.waveCount)
        {
            StartTimeout(() => StartWave(waveIndex + 1),
                possessAttackFlow.activePhase.warningDuration + possessAttackFlow.activePhase.sourceActiveDuration);
        }
    }

    private Vector3 GetSpawnPoint(List<Vector3> alreadySpawned, int maxDepth = 20)
    {
        if (maxDepth == 0)
        {
            throw new NoSpawnPointException();
        }

        var player = CachedObjectsManager.Instance.GetObject<Character>("Player");
        var groundPlayerPosition = player.movableObject.GroundWorldPosition;
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
               && CachedObjectsManager.Instance.GetObjects<Hitbox>("Barrier").ToArray()
                   .All(barrier => !barrier.IsInside(newPosition))
               && alreadySpawned.All(existingPosition =>
                   Vector3.Distance(newPosition, existingPosition) > 2 * minSourcesDistance);
    }

    protected override IAttackFlow AttackFlow => possessAttackFlow;
}