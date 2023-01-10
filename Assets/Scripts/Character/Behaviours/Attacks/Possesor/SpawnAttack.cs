using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnAttackFlow : IAttackFlow
{
    public TimedAttackPhase anticipationPhase;
    public EmptyAttackPhase activePhase;
    public TimedAttackPhase recoveryPhase;
    public IAttackPhase AnticipationPhase => anticipationPhase;
    public IAttackPhase ActivePhase => activePhase;
    public IAttackPhase RecoveryPhase => recoveryPhase;
}

public class SpawnAttack : BaseAttack
{
    public SpawnAttackFlow spawnAttackFlow;
    public int maxEnemyCount;
    public GameObject enemyPrefab;
    public Vector3[] spawnPoints;

    protected override IAttackFlow AttackFlow => spawnAttackFlow;

    public override bool CanPlay(BaseAttackCommand command)
    {
        return base.CanPlay(command) &&
               !((AttackManager.Anticipating || AttackManager.Active || AttackManager.HardRecovering) &&
                 !(instant && AttackManager.IsInterruptible()));
    }

    public override void Awake()
    {
        base.Awake();
        PlayEvents.onPlay.AddListener(() =>
        {
            DisableBehaviours(typeof(WalkBehaviour));
            StopBehaviours(typeof(WalkBehaviour));
        });
        PlayEvents.onStop.AddListener(() => EnableBehaviours(typeof(WalkBehaviour)));

        attackEvents.onStartActive.AddListener(SpawnGoblins);
    }

    private void SpawnGoblins()
    {
        var currentEnemyCount = CachedObjectsManager.Instance.GetObjects<Character>("Enemy").Length;
        var spawnCount = maxEnemyCount - currentEnemyCount;
        var selectedPoints = GetRandomSpawnPoints(spawnCount);
        for (var i = 0; i < maxEnemyCount - currentEnemyCount; i++)
        {
            var newEnemy = Instantiate(enemyPrefab, MovableObject.ScreenCoordinates(selectedPoints[i]),
                Quaternion.identity);
            newEnemy.GetComponent<MovableObject>().WorldPosition = selectedPoints[i];
            newEnemy.GetComponent<EnemyBrain>().Alarm();
        }
    }

    private Vector3[] GetRandomSpawnPoints(int count)
    {
        var availablePoints = spawnPoints.ToList();
        var selectedPoints = new Vector3[count];

        for (var i = 0; i < count; i++)
        {
            var randomIndex = Random.Range(0, availablePoints.Count);
            selectedPoints[i] = availablePoints[randomIndex];
            availablePoints.RemoveAt(randomIndex);
        }

        return selectedPoints;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var point in spawnPoints)
        {
            Gizmos.DrawWireSphere(MovableObject.ScreenCoordinates(point), 0.2f);
        }
    }
}